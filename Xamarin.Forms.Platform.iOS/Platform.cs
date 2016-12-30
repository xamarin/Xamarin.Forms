using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Forms.Platform.iOS
{
	public class Platform : BindableObject, IPlatform, INavigation, IDisposable
	{
		internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer),
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var view = bindable as VisualElement;
				if (view != null)
					view.IsPlatformEnabled = newvalue != null;
			});

		readonly int _alertPadding = 10;

		readonly List<Page> _modals;
		readonly PlatformRenderer _renderer;
		bool _animateModals = true;
		bool _appeared;

		bool _disposed;
		UIView _pageOverlayView;

		internal Platform()
		{
			_renderer = new PlatformRenderer(this)
			{
				View =
				{
					BackgroundColor = UIColor.White,
					ContentMode = UIViewContentMode.Redraw
				}
			};
			_modals = new List<Page>();

			var busyCount = 0;
			MessagingCenter.Subscribe(this, Page.BusySetSignalName, (Page sender, bool enabled) =>
			{
				if (!PageIsChildOfPlatform(sender))
					return;
				busyCount = Math.Max(0, enabled ? busyCount + 1 : busyCount - 1);
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = busyCount > 0;
			});

			MessagingCenter.Subscribe(this, Page.AlertSignalName, (Page sender, AlertArguments arguments) =>
			{
				if (!PageIsChildOfPlatform(sender))
					return;
				PresentAlert(arguments);
			});

			MessagingCenter.Subscribe(this, Page.ActionSheetSignalName, (Page sender, ActionSheetArguments arguments) =>
			{
				if (!PageIsChildOfPlatform(sender))
					return;

				var pageRoot = sender;
				while (!Application.IsApplicationOrNull(pageRoot.RealParent))
					pageRoot = (Page)pageRoot.RealParent;

				PresentActionSheet(arguments);
			});
		}

		internal UIViewController ViewController
		{
			get { return _renderer; }
		}

		internal Page Page { get; set; }

		void IDisposable.Dispose()
		{
			if (_disposed)
				return;
			_disposed = true;

			Page.DescendantRemoved -= HandleChildRemoved;
			MessagingCenter.Unsubscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName);
			MessagingCenter.Unsubscribe<Page, AlertArguments>(this, Page.AlertSignalName);
			MessagingCenter.Unsubscribe<Page, bool>(this, Page.BusySetSignalName);

			foreach (var modal in _modals)
			{
				_modals.Remove(modal);
				DisposeModelAndChildrenRenderers(modal);
			}

			DisposeModelAndChildrenRenderers(Page);

			_renderer.Dispose();
		}

		void INavigation.InsertPageBefore(Page page, Page before)
		{
			throw new InvalidOperationException("InsertPageBefore is not supported globally on iOS, please use a NavigationPage.");
		}

		IReadOnlyList<Page> INavigation.ModalStack
		{
			get { return _modals; }
		}

		IReadOnlyList<Page> INavigation.NavigationStack
		{
			get { return new List<Page>(); }
		}

		Task<Page> INavigation.PopAsync()
		{
			return ((INavigation)this).PopAsync(true);
		}

		Task<Page> INavigation.PopAsync(bool animated)
		{
			throw new InvalidOperationException("PopAsync is not supported globally on iOS, please use a NavigationPage.");
		}

		Task<Page> INavigation.PopModalAsync()
		{
			return ((INavigation)this).PopModalAsync(true);
		}

		async Task<Page> INavigation.PopModalAsync(bool animated)
		{
			var modal = _modals.Last();
			_modals.Remove(modal);
			modal.DescendantRemoved -= HandleChildRemoved;

			var controller = GetRenderer(modal) as UIViewController;

			if (_modals.Count >= 1 && controller != null)
				await controller.DismissViewControllerAsync(animated);
			else
				await _renderer.DismissViewControllerAsync(animated);

			DisposeModelAndChildrenRenderers(modal);

			return modal;
		}

		Task INavigation.PopToRootAsync()
		{
			return ((INavigation)this).PopToRootAsync(true);
		}

		Task INavigation.PopToRootAsync(bool animated)
		{
			throw new InvalidOperationException("PopToRootAsync is not supported globally on iOS, please use a NavigationPage.");
		}

		Task INavigation.PushAsync(Page root)
		{
			return ((INavigation)this).PushAsync(root, true);
		}

		Task INavigation.PushAsync(Page root, bool animated)
		{
			throw new InvalidOperationException("PushAsync is not supported globally on iOS, please use a NavigationPage.");
		}

		Task INavigation.PushModalAsync(Page modal)
		{
			return ((INavigation)this).PushModalAsync(modal, true);
		}

		Task INavigation.PushModalAsync(Page modal, bool animated)
		{
			_modals.Add(modal);
			modal.Platform = this;

			modal.DescendantRemoved += HandleChildRemoved;

			return PresentModal(modal, _animateModals && animated);
		}

		void INavigation.RemovePage(Page page)
		{
			throw new InvalidOperationException("RemovePage is not supported globally on iOS, please use a NavigationPage.");
		}

		SizeRequest IPlatform.GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			var renderView = GetRenderer(view);
			if (renderView == null || renderView.NativeView == null)
				return new SizeRequest(Size.Zero);

			return renderView.GetDesiredSize(widthConstraint, heightConstraint);
		}

		public static IVisualElementRenderer CreateRenderer(VisualElement element)
		{
			var t = element.GetType();
			var renderer = Registrar.Registered.GetHandler<IVisualElementRenderer>(t) ?? new DefaultRenderer();
			renderer.SetElement(element);
			return renderer;
		}

		public static IVisualElementRenderer GetRenderer(VisualElement bindable)
		{
			return (IVisualElementRenderer)bindable.GetValue(RendererProperty);
		}

		public static void SetRenderer(VisualElement bindable, IVisualElementRenderer value)
		{
			bindable.SetValue(RendererProperty, value);
		}

		protected override void OnBindingContextChanged()
		{
			SetInheritedBindingContext(Page, BindingContext);

			base.OnBindingContextChanged();
		}

		internal void DidAppear()
		{
			if (_appeared || _disposed)
				return;

			_appeared = true;
		}

		internal void DisposeModelAndChildrenRenderers(Element view)
		{
			IVisualElementRenderer renderer;

			foreach (VisualElement child in view.Descendants())
			{
				renderer = GetRenderer(child);
				child.ClearValue(RendererProperty);

				if (renderer != null)
				{
					renderer.NativeView.RemoveFromSuperview();
					renderer.ViewController?.Dispose();
					renderer.Dispose();
				}
			}

			renderer = GetRenderer((VisualElement)view);
			view.ClearValue(RendererProperty);
			if (renderer != null)
			{
				if (renderer.ViewController != null)
				{
					var modalWrapper = renderer.ViewController.ParentViewController as ModalWrapper;
					modalWrapper?.Dispose();

					renderer.ViewController.Dispose();
				}

				renderer.NativeView.RemoveFromSuperview();
				renderer.Dispose();
			}
		}

		internal void DisposeRendererAndChildren(IVisualElementRenderer rendererToRemove)
		{
			if (rendererToRemove == null)
				return;

			if (rendererToRemove.Element != null && GetRenderer(rendererToRemove.Element) == rendererToRemove)
				rendererToRemove.Element.ClearValue(RendererProperty);

			var subviews = rendererToRemove.NativeView.Subviews;
			for (var i = 0; i < subviews.Length; i++)
			{
				var childRenderer = subviews[i] as IVisualElementRenderer;
				if (childRenderer != null)
					DisposeRendererAndChildren(childRenderer);
			}

			rendererToRemove.NativeView.RemoveFromSuperview();
			rendererToRemove.ViewController?.Dispose();
			rendererToRemove.Dispose();
		}

		internal void LayoutSubviews()
		{
			if (Page == null)
				return;

			var rootRenderer = GetRenderer(Page);
			rootRenderer?.SetElementSize(new Size(_renderer.View.Bounds.Width, _renderer.View.Bounds.Height));

			if (_pageOverlayView != null && !ReferenceEquals(_pageOverlayView, _renderer.View.Subviews.LastOrDefault()))
				throw new InvalidOperationException("Overlay must be the topmost subview of MainPage.");
		}

		internal void SetPage(Page newRoot)
		{
			if (newRoot == null)
				return;
			if (Page != null)
				throw new NotImplementedException();

			Page = newRoot;
			Page.Platform = this;
			AddChild(Page);

			Page.DescendantRemoved += HandleChildRemoved;

			_animateModals = false;
			Application.Current.NavigationProxy.Inner = this;
			_animateModals = true;
		}

		void AddChild(VisualElement view)
		{
			if (!Application.IsApplicationOrNull(view.RealParent))
				Console.Error.WriteLine("Tried to add parented view to canvas directly");

			if (GetRenderer(view) == null)
			{
				var viewRenderer = CreateRenderer(view);
				SetRenderer(view, viewRenderer);

				_renderer.View.AddSubview(viewRenderer.NativeView);
				if (viewRenderer.ViewController != null)
					_renderer.AddChildViewController(viewRenderer.ViewController);
				viewRenderer.NativeView.Frame = new RectangleF(0, 0, _renderer.View.Bounds.Width, _renderer.View.Bounds.Height);
				viewRenderer.SetElementSize(new Size(_renderer.View.Bounds.Width, _renderer.View.Bounds.Height));
			}
			else
				Console.Error.WriteLine("Potential view double add");
		}

		void HandleChildRemoved(object sender, ElementEventArgs e)
		{
			var view = e.Element;
			DisposeModelAndChildrenRenderers(view);
		}

		bool PageIsChildOfPlatform(Page page)
		{
			while (!Application.IsApplicationOrNull(page.RealParent))
				page = (Page)page.RealParent;

			return Page == page || _modals.Contains(page);
		}

		// Creates a UIAlertAction which includes a call to hide the presenting UIWindow at the end
		UIAlertAction CreateActionWithWindowHide(string text, UIAlertActionStyle style, Action setResult, UIWindow window)
		{
			return UIAlertAction.Create(text, style,
					a =>
					{
						setResult();
						window.Hidden = true;
					});
		}

		void PresentAlert(AlertArguments arguments)
		{
			var window = new UIWindow { BackgroundColor = Color.Transparent.ToUIColor() };

			var alert = UIAlertController.Create(arguments.Title, arguments.Message, UIAlertControllerStyle.Alert);
			var oldFrame = alert.View.Frame;
			alert.View.Frame = new RectangleF(oldFrame.X, oldFrame.Y, oldFrame.Width, oldFrame.Height - _alertPadding * 2);

			if (arguments.Cancel != null)
			{
				alert.AddAction(CreateActionWithWindowHide(arguments.Cancel, UIAlertActionStyle.Cancel,
					() => arguments.SetResult(false), window));
			}

			if (arguments.Accept != null)
			{
				alert.AddAction(CreateActionWithWindowHide(arguments.Accept, UIAlertActionStyle.Default,
					() => arguments.SetResult(true), window));
			}

			PresentPopUp(window, alert);
		}

		void PresentActionSheet(ActionSheetArguments arguments)
		{
			var alert = UIAlertController.Create(arguments.Title, null, UIAlertControllerStyle.ActionSheet);
			var window = new UIWindow { BackgroundColor = Color.Transparent.ToUIColor() };

			if (arguments.Cancel != null)
			{
				alert.AddAction(CreateActionWithWindowHide(arguments.Cancel, UIAlertActionStyle.Cancel, () => arguments.SetResult(arguments.Cancel), window));
			}

			if (arguments.Destruction != null)
			{
				alert.AddAction(CreateActionWithWindowHide(arguments.Destruction, UIAlertActionStyle.Destructive, () => arguments.SetResult(arguments.Destruction), window));
			}

			foreach (var label in arguments.Buttons)
			{
				if (label == null)
					continue;

				var blabel = label;

				alert.AddAction(CreateActionWithWindowHide(blabel, UIAlertActionStyle.Default, () => arguments.SetResult(blabel), window));
			}

			PresentPopUp(window, alert, arguments);
		}

		static void PresentPopUp(UIWindow window, UIAlertController alert, ActionSheetArguments arguments = null)
		{
			window.RootViewController = new UIViewController();
			window.RootViewController.View.BackgroundColor = Color.Transparent.ToUIColor();
			window.WindowLevel = UIWindowLevel.Alert + 1;
			window.MakeKeyAndVisible();

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad && arguments != null)
			{
				UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
				var observer = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification,
					n => { alert.PopoverPresentationController.SourceRect = window.RootViewController.View.Bounds; });

				arguments.Result.Task.ContinueWith(t =>
				{
					NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
					UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
				}, TaskScheduler.FromCurrentSynchronizationContext());

				alert.PopoverPresentationController.SourceView = window.RootViewController.View;
				alert.PopoverPresentationController.SourceRect = window.RootViewController.View.Bounds;
				alert.PopoverPresentationController.PermittedArrowDirections = 0; // No arrow
			}

			if(!Forms.IsiOS9OrNewer)
			{
				// For iOS 8, we need to explicitly set the size of the window
				window.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			}

			window.RootViewController.PresentViewController(alert, true, null);
		}

		async Task PresentModal(Page modal, bool animated)
		{
			AddPageOverlay();
			while (!_appeared)
			{
				await Task.Delay(5);
			}

			var modalRenderer = GetRenderer(modal);
			if (modalRenderer == null)
			{
				modalRenderer = CreateRenderer(modal);
				SetRenderer(modal, modalRenderer);
			}

			var wrapper = new ModalWrapper(modalRenderer);

			if (_modals.Count > 1)
			{
				var topPage = _modals[_modals.Count - 2];
				var controller = GetRenderer(topPage) as UIViewController;
				if (controller != null)
				{
					await controller.PresentViewControllerAsync(wrapper, animated);
					while (!wrapper.Appeared)
					{
						await Task.Delay(5);
					}
					return;
				}

				Console.Error.WriteLine("Presenting topmost modal had no renderer");
			}

			// After presentation is done, wait until the wrapper has appeared.
			await _renderer.PresentViewControllerAsync(wrapper, animated);
			while (!wrapper.Appeared)
			{
				await Task.Delay(5);
			}
			RemovePageOverlay();
		}

		void AddPageOverlay()
		{
			var color = Application.Current.OnThisPlatform().PageOverlayColorWhenPushingModal();

			if (color == null || _pageOverlayView != null || _renderer.View.Subviews.Contains(_pageOverlayView))
				return;

			_pageOverlayView = new UIView
			{
				Frame = _renderer.View.Bounds,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				BackgroundColor = color.Value.ToUIColor()
			};

			_renderer.View.AddSubview(_pageOverlayView);
			_renderer.View.BringSubviewToFront(_pageOverlayView);
		}

		void RemovePageOverlay()
		{
			if (_pageOverlayView == null)
				return;

			_pageOverlayView.RemoveFromSuperview();
			_pageOverlayView.Dispose();
			_pageOverlayView = null;
		}

		internal class DefaultRenderer : VisualElementRenderer<VisualElement>
		{
		}
	}
}