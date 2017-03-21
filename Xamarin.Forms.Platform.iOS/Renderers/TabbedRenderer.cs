using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using UIKit;
using Xamarin.Forms.Internals;
using static Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page;
using PageUIStatusBarAnimation = Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIStatusBarAnimation;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Platform.iOS
{
	public class TabbedRenderer : UITabBarController, IVisualElementRenderer, IEffectControlProvider
	{
		bool _barBackgroundColorWasSet;
		bool _barTextColorWasSet;
		UIColor _defaultBarTextColor;
		bool _defaultBarTextColorSet;
		UIColor _defaultBarColor;
		bool _defaultBarColorSet;
		bool _loaded;
		Size _queuedSize;

		Dictionary<int, TabSwipe> _tabSwipes;
		UIPanGestureRecognizer _uiPanGestureRecognizer;
		CGPoint _startLocation = CGPoint.Empty;
		string _direction = string.Empty;
		bool _shouldSelect = true, _shouldPan = true;
		const string Right = "Right", Left = "Left";

		IPageController PageController => Element as IPageController;
		IElementController ElementController => Element as IElementController;

		public override UIViewController SelectedViewController
		{
			get { return base.SelectedViewController; }
			set
			{
				base.SelectedViewController = value;
				UpdateCurrentPage();
			}
		}

		protected TabbedPage Tabbed
		{
			get { return (TabbedPage)Element; }
		}

		public VisualElement Element { get; private set; }

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
		}

		public UIView NativeView
		{
			get { return View; }
		}

		public void SetElement(VisualElement element)
		{
			var oldElement = Element;
			Element = element;

			FinishedCustomizingViewControllers += HandleFinishedCustomizingViewControllers;
			Tabbed.PropertyChanged += OnPropertyChanged;
			Tabbed.PagesChanged += OnPagesChanged;

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

			OnPagesChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

			if (element != null)
				element.SendViewInitialized(NativeView);

			//disable edit/reorder of tabs
			CustomizableViewControllers = null;

			UpdateBarBackgroundColor();
			UpdateBarTextColor();
			UpdateTabSwipes();

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
		}

		public void SetElementSize(Size size)
		{
			if (_loaded)
				Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
			else
				_queuedSize = size;
		}

		public UIViewController ViewController
		{
			get { return this; }
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);

			View.SetNeedsLayout();
		}

		public override void ViewDidAppear(bool animated)
		{
			PageController.SendAppearing();
			base.ViewDidAppear(animated);
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			PageController.SendDisappearing();
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			if (Element == null)
				return;

			if (!Element.Bounds.IsEmpty)
			{
				View.Frame = new System.Drawing.RectangleF((float)Element.X, (float)Element.Y, (float)Element.Width, (float)Element.Height);
			}

			var frame = View.Frame;
			var tabBarFrame = TabBar.Frame;
			PageController.ContainerArea = new Rectangle(0, 0, frame.Width, frame.Height - tabBarFrame.Height);

			if (!_queuedSize.IsZero)
			{
				Element.Layout(new Rectangle(Element.X, Element.Y, _queuedSize.Width, _queuedSize.Height));
				_queuedSize = Size.Zero;
			}

			_loaded = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				PageController.SendDisappearing();
				Tabbed.PropertyChanged -= OnPropertyChanged;
				Tabbed.PagesChanged -= OnPagesChanged;
				FinishedCustomizingViewControllers -= HandleFinishedCustomizingViewControllers;
			}

			base.Dispose(disposing);
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			var changed = ElementChanged;
			if (changed != null)
				changed(this, e);
		}

		UIViewController GetViewController(Page page)
		{
			var renderer = Platform.GetRenderer(page);
			if (renderer == null)
				return null;

			return renderer.ViewController;
		}

		void HandleFinishedCustomizingViewControllers(object sender, UITabBarCustomizeChangeEventArgs e)
		{
			if (e.Changed)
				UpdateChildrenOrderIndex(e.ViewControllers);
		}

		void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// Setting TabBarItem.Title in iOS 10 causes rendering bugs
			// Work around this by creating a new UITabBarItem on each change
			if (e.PropertyName == Page.TitleProperty.PropertyName && !Forms.IsiOS10OrNewer)
			{
				var page = (Page)sender;
				var renderer = Platform.GetRenderer(page);
				if (renderer == null)
					return;

				if (renderer.ViewController.TabBarItem != null)
					renderer.ViewController.TabBarItem.Title = page.Title;
			}
			else if (e.PropertyName == Page.IconProperty.PropertyName || e.PropertyName == Page.TitleProperty.PropertyName && Forms.IsiOS10OrNewer)
			{
				var page = (Page)sender;

				IVisualElementRenderer renderer = Platform.GetRenderer(page);

				if (renderer?.ViewController.TabBarItem == null)
					return;

				SetTabBarItem(renderer);
			}
		}

		void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			e.Apply((o, i, c) => SetupPage((Page)o, i), (o, i) => TeardownPage((Page)o, i), Reset);

			SetControllers();

			UIViewController controller = null;
			if (Tabbed.CurrentPage != null)
				controller = GetViewController(Tabbed.CurrentPage);
			if (controller != null && controller != base.SelectedViewController)
				base.SelectedViewController = controller;
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(TabbedPage.CurrentPage))
			{
				var current = Tabbed.CurrentPage;
				if (current == null)
					return;

				var controller = GetViewController(current);
				if (controller == null)
					return;

				SelectedViewController = controller;
			}
			else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
				UpdateBarBackgroundColor();
			else if (e.PropertyName == TabbedPage.BarTextColorProperty.PropertyName)
				UpdateBarTextColor();
			else if (e.PropertyName == PrefersStatusBarHiddenProperty.PropertyName)
				UpdatePrefersStatusBarHiddenOnPages();
			else if (e.PropertyName == PreferredStatusBarUpdateAnimationProperty.PropertyName)
				UpdateCurrentPagePreferredStatusBarUpdateAnimation();
			else if (e.PropertyName == PlatformConfiguration.iOSSpecific.TabbedPage.TabSwipesProperty.PropertyName)
				UpdateTabSwipes();
		}

		public override UIViewController ChildViewControllerForStatusBarHidden()
		{
			var current = Tabbed.CurrentPage;
			if (current == null)
				return null;

			return GetViewController(current);
		}

		void UpdateCurrentPagePreferredStatusBarUpdateAnimation()
		{
			PageUIStatusBarAnimation animation = ((Page)Element).OnThisPlatform().PreferredStatusBarUpdateAnimation();
			Tabbed.CurrentPage.OnThisPlatform().SetPreferredStatusBarUpdateAnimation(animation);
		}

		void UpdatePrefersStatusBarHiddenOnPages()
		{
			for (var i = 0; i < ViewControllers.Length; i++)
			{
				Tabbed.GetPageByIndex(i).OnThisPlatform().SetPrefersStatusBarHidden(Tabbed.OnThisPlatform().PrefersStatusBarHidden());
			}
		}

		void Reset()
		{
			var i = 0;
			foreach (var page in Tabbed.Children)
				SetupPage(page, i++);
		}

		void SetControllers()
		{
			var list = new List<UIViewController>();
			for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
			{
				var child = ElementController.LogicalChildren[i];
				var v = child as VisualElement;
				if (v == null)
					continue;
				if (Platform.GetRenderer(v) != null)
					list.Add(Platform.GetRenderer(v).ViewController);
			}
			ViewControllers = list.ToArray();
		}

		void SetupPage(Page page, int index)
		{
			IVisualElementRenderer renderer = Platform.GetRenderer(page);
			if (renderer == null)
			{
				renderer = Platform.CreateRenderer(page);
				Platform.SetRenderer(page, renderer);
			}

			page.PropertyChanged += OnPagePropertyChanged;

			SetTabBarItem(renderer);
		}

		void TeardownPage(Page page, int index)
		{
			page.PropertyChanged -= OnPagePropertyChanged;

			Platform.SetRenderer(page, null);
		}

		void UpdateBarBackgroundColor()
		{
			if (Tabbed == null || TabBar == null)
				return;

			var barBackgroundColor = Tabbed.BarBackgroundColor;
			var isDefaultColor = barBackgroundColor.IsDefault;

			if (isDefaultColor && !_barBackgroundColorWasSet)
				return;

			if (!_defaultBarColorSet)
			{
				_defaultBarColor = TabBar.BarTintColor;

				_defaultBarColorSet = true;
			}

			if (!isDefaultColor)
				_barBackgroundColorWasSet = true;
			
			TabBar.BarTintColor = isDefaultColor ? _defaultBarColor : barBackgroundColor.ToUIColor();
		}

		void UpdateBarTextColor()
		{
			if (Tabbed == null || TabBar == null || TabBar.Items == null)
				return;

			var barTextColor = Tabbed.BarTextColor;
			var isDefaultColor = barTextColor.IsDefault;

			if (isDefaultColor && !_barTextColorWasSet)
				return;

			if (!_defaultBarTextColorSet)
			{
				_defaultBarTextColor = TabBar.TintColor;
				_defaultBarTextColorSet = true;
			}

			if (!isDefaultColor)
				_barTextColorWasSet = true;

			var attributes = new UITextAttributes();

			if (isDefaultColor)
				attributes.TextColor = _defaultBarTextColor;
			else
				attributes.TextColor = barTextColor.ToUIColor();

			foreach (UITabBarItem item in TabBar.Items)
			{
				item.SetTitleTextAttributes(attributes, UIControlState.Normal);
			}

			// set TintColor for selected icon
			// setting the unselected icon tint is not supported by iOS
			TabBar.TintColor = isDefaultColor ? _defaultBarTextColor : barTextColor.ToUIColor();
		}

		void UpdateChildrenOrderIndex(UIViewController[] viewControllers)
		{
			for (var i = 0; i < viewControllers.Length; i++)
			{
				var originalIndex = -1;
				if (int.TryParse(viewControllers[i].TabBarItem.Tag.ToString(), out originalIndex))
				{
					var page = (Page)((IPageController)Tabbed).InternalChildren[originalIndex];
					TabbedPage.SetIndex(page, i);
				}
			}
		}

		void UpdateCurrentPage()
		{
			var count = ((IPageController)Tabbed).InternalChildren.Count;
			var index = (int)SelectedIndex;
			((TabbedPage)Element).CurrentPage = index >= 0 && index < count ? Tabbed.GetPageByIndex(index) : null;
		}

		void UpdateTabSwipes()
		{
			_tabSwipes = ((TabbedPage)Element).OnThisPlatform().TabSwipes();

			if (_tabSwipes == null || _tabSwipes.Count == 0)
			{
				ShouldSelectViewController = null;

				if (_uiPanGestureRecognizer == null)
					return;

				if(View.GestureRecognizers.Contains(_uiPanGestureRecognizer))
					View.GestureRecognizers.Remove(_uiPanGestureRecognizer);

				_uiPanGestureRecognizer.Dispose();
				_uiPanGestureRecognizer = null;
			}
			else
			{
				ShouldSelectViewController = (tabController, controller) =>
				{
					if (SelectedViewController == null || ReferenceEquals(controller, SelectedViewController))
						return true;

					if (!_shouldSelect)
						return false;

					_shouldSelect = false;
					_shouldPan = false;

					UIView fromView = SelectedViewController.View;
					UIView toView = controller.View;
					int newTabIndex = ViewController.ChildViewControllers.IndexOf(controller);
					CGRect viewSize = fromView.Frame;
					nfloat maxX = viewSize.Width;
					nfloat minX = -1 * viewSize.Width;
					SwipeConfig swipeConfig = GetSwipeConfig(SelectedIndex < newTabIndex) ?? new SwipeConfig();

					if (!fromView.Superview.Subviews.Contains(toView))
					{
						fromView.Superview.AddSubview(toView);
						toView.Frame = new CGRect(newTabIndex < SelectedIndex ? minX : maxX, viewSize.Y, viewSize.Width, viewSize.Height);
					}

					UIView.Animate(swipeConfig.Duration,
						() =>
						{
							fromView.Frame = new CGRect(newTabIndex < SelectedIndex ? maxX : minX, viewSize.Y, viewSize.Width, viewSize.Height);
							toView.Frame = new CGRect(0, viewSize.Y, viewSize.Width, viewSize.Height);
						},
						() =>
						{
							fromView.RemoveFromSuperview();
							SelectedIndex = newTabIndex;
							_shouldSelect = true;
							_shouldPan = true;
						}
					);
					
					return _shouldSelect;
				};

				if (_uiPanGestureRecognizer != null)
					return;

				_uiPanGestureRecognizer = new UIPanGestureRecognizer(HandlePanGesture);
				View.AddGestureRecognizer(_uiPanGestureRecognizer);
			}
		}

		SwipeConfig GetSwipeConfig(bool isLeftSwipe)
		{
			var swipeConfig = new SwipeConfig();
			if (_tabSwipes.ContainsKey((int)SelectedIndex))
				swipeConfig = isLeftSwipe ? _tabSwipes[(int)SelectedIndex].LeftSwipeConfig : _tabSwipes[(int)SelectedIndex].RightSwipeConfig;

			return swipeConfig;
		}

		void HandlePanGesture(UIPanGestureRecognizer gestureRecognizer)
		{
			// Handler seems to be getting hit even when the owner is null
			if (_uiPanGestureRecognizer == null || !_shouldPan)
				return;

			_shouldSelect = false;
			// Don't set _shouldPan to false here because we need the handler with each movement

			UIView fromView = SelectedViewController.View;
			CGRect viewSize = fromView.Frame;
			nfloat minX = -1 * viewSize.Width;
			nfloat maxX = viewSize.Width;
			CGPoint currentVelocity = gestureRecognizer.VelocityInView(NativeView);
			CGPoint currentLocation = gestureRecognizer.TranslationInView(NativeView);
			nfloat dX = _startLocation.X - currentLocation.X;

			if (dX < 0)
			{
				SwipeConfig rightSwipeConfig = GetSwipeConfig(false);

				if (rightSwipeConfig == null || !rightSwipeConfig.IsEnabled)
				{
					_shouldSelect = true;
					return;
				}
			}
			else if (dX > 0)
			{
				SwipeConfig leftSwipeConfig = GetSwipeConfig(true);

				if (leftSwipeConfig == null || !leftSwipeConfig.IsEnabled)
				{
					_shouldSelect = true;
					return;
				}
			}

			switch (gestureRecognizer.State)
			{
				case UIGestureRecognizerState.Began:
					_startLocation = gestureRecognizer.TranslationInView(NativeView);
					break;
				case UIGestureRecognizerState.Changed:
					HandleGestureRecognizerStateChanged(fromView, dX, viewSize, maxX, minX, currentLocation);
					break;
				default:
					HandleGestureRecognizerStateOther(currentVelocity, fromView, viewSize, maxX, minX);
					break;
			}
		}

		void HandleGestureRecognizerStateChanged(UIView fromView, nfloat dX, CGRect viewSize, nfloat maxX, nfloat minX, CGPoint currentLocation)
		{
			if ((SelectedIndex == 0 && fromView.Frame.X - dX > 0) || (SelectedIndex == (Element as TabbedPage).Children.Count - 1 && fromView.Frame.X - dX < 0))
			{
				_shouldSelect = true;
				return;
			}

			if (_direction == string.Empty)
			{
				_direction = dX < 0 ? Right : _direction;
				_direction = dX > 0 ? Left : _direction;
			}

			UIView toView;
			switch (_direction)
			{
				case Right:
					toView = ViewControllers[SelectedIndex - 1].View;

					if (!fromView.Superview.Subviews.Contains(toView))
					{
						fromView.Superview.AddSubview(toView);
						toView.Frame = new CGRect(minX, viewSize.Y, viewSize.Width, viewSize.Height);
					}
					break;
				case Left:
					toView = ViewControllers[SelectedIndex + 1].View;

					if (!fromView.Superview.Subviews.Contains(toView))
					{
						fromView.Superview.AddSubview(toView);
						toView.Frame = new CGRect(maxX, viewSize.Y, viewSize.Width, viewSize.Height);
					}
					break;
				default:
					_shouldSelect = true;
					return;
			}

			fromView.Frame = new CGRect(((double)(fromView.Frame.X - dX)).Clamp(_direction == Right ? 0 : minX, _direction == Right ? maxX : 0), viewSize.Y, viewSize.Width, viewSize.Height);
			toView.Frame = new CGRect(((double)(toView.Frame.X - dX)).Clamp(_direction == Right ? minX : 0, _direction == Right ? 0 : maxX), viewSize.Y, viewSize.Width, viewSize.Height);

			_startLocation = currentLocation;
		}

		async void HandleGestureRecognizerStateOther(CGPoint currentVelocity, UIView fromView, CGRect viewSize, nfloat maxX, nfloat minX)
		{
			SwipeConfig rightSwipeConfig = GetSwipeConfig(false);
			SwipeConfig leftSwipeConfig = GetSwipeConfig(true);
			UIView toView = null;
			var shouldRunToCompletion = false;
			switch (_direction)
			{
				case Right:
					toView = ViewControllers[SelectedIndex - 1].View;
					shouldRunToCompletion = Math.Abs(currentVelocity.X) >= rightSwipeConfig.Velocity;
					break;
				case Left:
					toView = ViewControllers[SelectedIndex + 1].View;
					shouldRunToCompletion = Math.Abs(currentVelocity.X) >= leftSwipeConfig.Velocity;
					break;
			}

			if (shouldRunToCompletion)
			{
				_shouldPan = false;
				UIView.Animate(_direction == Right ? rightSwipeConfig.Duration : leftSwipeConfig.Duration,
					() =>
					{
						fromView.Frame = new CGRect(_direction == Right ? maxX : minX, viewSize.Y, viewSize.Width, viewSize.Height);
						toView.Frame = new CGRect(0, viewSize.Y, viewSize.Width, viewSize.Height);
					},
					() =>
					{
						fromView.RemoveFromSuperview();
						SelectedIndex = _direction == Right ? SelectedIndex - 1 : SelectedIndex + 1;
						_startLocation = CGPoint.Empty;
						_direction = string.Empty;
						_shouldSelect = true;
						_shouldPan = true;
					}
				);

				while (!_shouldPan)
					await Task.Delay(5);
			}
			else
				SnapViewIntoPosition(_direction == Right ? rightSwipeConfig : leftSwipeConfig, fromView, viewSize, maxX, minX);
		}

		void SnapViewIntoPosition(SwipeConfig swipeConfig, UIView fromView, CGRect viewSize, nfloat maxX, nfloat minX)
		{
			if (fromView.Frame.X < 0)
			{
				UIView toView = ViewControllers[SelectedIndex + 1].View;

				if (fromView.Frame.X <= minX / 2)
					AnimateSnapViewIntoPosition(swipeConfig, fromView, toView, viewSize, minX, 0, false, false);
				else
					AnimateSnapViewIntoPosition(swipeConfig, fromView, toView, viewSize, 0, maxX, true);
			}
			else if (fromView.Frame.X > 0)
			{
				UIView toView = ViewControllers[SelectedIndex - 1].View;

				if (fromView.Frame.X >= maxX / 2)
					AnimateSnapViewIntoPosition(swipeConfig, fromView, toView, viewSize, maxX, 0, false);
				else
					AnimateSnapViewIntoPosition(swipeConfig, fromView, toView, viewSize, 0, minX, true);
			}

			_startLocation = CGPoint.Empty;
			_direction = string.Empty;
			_shouldSelect = true;
		}

		async void AnimateSnapViewIntoPosition(SwipeConfig swipeConfig, UIView fromView, UIView toView, CGRect viewSize, nfloat fromViewX, nfloat toViewX, bool shouldRemoveToViewInstead, bool shouldDecrementSelectedIndex = true)
		{
			var didFinish = false;

			UIView.Animate(swipeConfig.Duration,
				() =>
				{
					fromView.Frame = new CGRect(fromViewX, viewSize.Y, viewSize.Width, viewSize.Height);
					toView.Frame = new CGRect(toViewX, viewSize.Y, viewSize.Width, viewSize.Height);
				},
				() =>
				{
					if(!shouldRemoveToViewInstead)
						fromView.RemoveFromSuperview();
					else
						toView.RemoveFromSuperview();

					if (!shouldRemoveToViewInstead)
						SelectedIndex = shouldDecrementSelectedIndex ? SelectedIndex - 1 : SelectedIndex + 1;

					didFinish = true;
				}
			);

			while (!didFinish)
				await Task.Delay(5);
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			VisualElementRenderer<VisualElement>.RegisterEffect(effect, View);
		}

		void SetTabBarItem(IVisualElementRenderer renderer)
		{
			var page = renderer.Element as Page;
			if(page == null)
				throw new InvalidCastException($"{nameof(renderer)} must be a {nameof(Page)} renderer.");

			var icons = GetIcon(page);
			renderer.ViewController.TabBarItem = new UITabBarItem(page.Title, icons?.Item1, icons?.Item2)
			{
				Tag = Tabbed.Children.IndexOf(page),
				AccessibilityIdentifier = page.AutomationId
			};
			icons?.Item1?.Dispose();
			icons?.Item2?.Dispose();
		}
		
		/// <summary>
		/// Get the icon for the tab bar item of this page
		/// </summary>
		/// <returns>
		/// A tuple containing as item1: the unselected version of the icon, item2: the selected version of the icon (item2 can be null),
		/// or null if no icon should be set.
		/// </returns>
		protected virtual Tuple<UIImage, UIImage> GetIcon(Page page)
		{
		    if (!string.IsNullOrEmpty(page.Icon))
		    {
		        var icon = new UIImage(page.Icon);
		        return Tuple.Create(icon, (UIImage)null);
		    }
		
		    return null;
		}
	}
}