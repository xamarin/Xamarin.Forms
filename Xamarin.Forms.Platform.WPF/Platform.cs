using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.WPF
{
	public class Platform : BindableObject, IPlatform, INavigation
	{
		internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer));
		
		readonly NavigationModel _navModel = new NavigationModel();

		readonly WPFPage _page;

		readonly Canvas _renderer;
		readonly ToolbarTracker _tracker = new ToolbarTracker();

		Page _currentDisplayedPage;
		CustomMessageBox _visibleMessageBox;

		internal Platform(WPFPage page)
		{
			_tracker.SeparateMasterDetail = true;

			page.BackKeyPress += OnBackKeyPress;
			_page = page;

			_renderer = new Canvas();
			_renderer.SizeChanged += RendererSizeChanged;
			
			SetProgressIndicator(false);

			var busyCount = 0;
			MessagingCenter.Subscribe(this, Page.BusySetSignalName, (Page sender, bool enabled) =>
			{
				busyCount = Math.Max(0, enabled ? busyCount + 1 : busyCount - 1);
				SetProgressIndicator(busyCount > 0);
			});

			MessagingCenter.Subscribe(this, Page.AlertSignalName, (Page sender, AlertArguments arguments) =>
			{
				var messageBox = new CustomMessageBox { Title = arguments.Title, Message = arguments.Message };
				if (arguments.Accept != null)
					messageBox.LeftButtonContent = arguments.Accept;
				messageBox.RightButtonContent = arguments.Cancel;
				messageBox.Show();
				_visibleMessageBox = messageBox;
				messageBox.Dismissed += (o, args) =>
				{
					arguments.SetResult(args.Result == CustomMessageBoxResult.LeftButton);
					_visibleMessageBox = null;
				};
			});

			MessagingCenter.Subscribe(this, Page.ActionSheetSignalName, (Page sender, ActionSheetArguments arguments) =>
			{
				var messageBox = new CustomMessageBox { Title = arguments.Title };

				var listBox = new ListBox { FontSize = 36, Margin = new System.Windows.Thickness(12) };
				var itemSource = new List<string>();

				if (!string.IsNullOrWhiteSpace(arguments.Destruction))
					itemSource.Add(arguments.Destruction);
				itemSource.AddRange(arguments.Buttons);
				if (!string.IsNullOrWhiteSpace(arguments.Cancel))
					itemSource.Add(arguments.Cancel);

				listBox.ItemsSource = itemSource.Select(s => new TextBlock { Text = s, Margin = new System.Windows.Thickness(0, 12, 0, 12) });
				messageBox.Content = listBox;

				listBox.SelectionChanged += (o, args) => messageBox.Dismiss();
				messageBox.Dismissed += (o, args) =>
				{
					string result = listBox.SelectedItem != null ? ((TextBlock)listBox.SelectedItem).Text : null;
					arguments.SetResult(result);
					_visibleMessageBox = null;
				};

				messageBox.Show();
				_visibleMessageBox = messageBox;
			});
		}

	    void SetProgressIndicator(bool isVisible)
	    {
	        //TODO: implement progress indicator;
	    }

	    internal Size Size
		{
			get { return new Size(_renderer.ActualWidth, _renderer.ActualHeight); }
		}

		Page Page { get; set; }

		void INavigation.InsertPageBefore(Page page, Page before)
		{
			_navModel.InsertPageBefore(page, before);
		}

		IReadOnlyList<Page> INavigation.ModalStack
		{
			get { return _navModel.Roots.ToList(); }
		}

		IReadOnlyList<Page> INavigation.NavigationStack
		{
			get { return _navModel.Tree.Last(); }
		}

		Task<Page> INavigation.PopAsync()
		{
			return ((INavigation)this).PopAsync(true);
		}

		Task<Page> INavigation.PopAsync(bool animated)
		{
			return Pop(Page, animated);
		}

		Task<Page> INavigation.PopModalAsync()
		{
			return ((INavigation)this).PopModalAsync(true);
		}

		Task<Page> INavigation.PopModalAsync(bool animated)
		{
			var tcs = new TaskCompletionSource<Page>();
			Page result = _navModel.PopModal();

			IReadOnlyList<Page> last = _navModel.Tree.Last();
			IEnumerable<Page> stack = last;
			if (last.Count > 1)
				stack = stack.Skip(1);

			Page navRoot = stack.First();
			Page current = _navModel.CurrentPage;
			if (current == navRoot)
				current = _navModel.Roots.Last(); // Navigation page itself, since nav root has a host

			SetCurrent(current, animated, true, () => tcs.SetResult(result));
			return tcs.Task;
		}

		Task INavigation.PopToRootAsync()
		{
			return ((INavigation)this).PopToRootAsync(true);
		}

		async Task INavigation.PopToRootAsync(bool animated)
		{
			await PopToRoot(Page, animated);
		}

		Task INavigation.PushAsync(Page root)
		{
			return ((INavigation)this).PushAsync(root, true);
		}

		Task INavigation.PushAsync(Page root, bool animated)
		{
			return Push(root, Page, animated);
		}

		Task INavigation.PushModalAsync(Page modal)
		{
			return ((INavigation)this).PushModalAsync(modal, true);
		}

		Task INavigation.PushModalAsync(Page modal, bool animated)
		{
			var tcs = new TaskCompletionSource<object>();
			_navModel.PushModal(modal);
			SetCurrent(_navModel.CurrentPage, animated, completedCallback: () => tcs.SetResult(null));
			return tcs.Task;
		}

		void INavigation.RemovePage(Page page)
		{
			RemovePage(page, true);
		}

		SizeRequest IPlatform.GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			// Hack around the fact that Canvas ignores the child constraints.
			// It is entirely possible using Canvas as our base class is not wise.
			// FIXME: This should not be an if statement. Probably need to define an interface here.
			if (widthConstraint > 0 && heightConstraint > 0 && GetRenderer(view) != null)
			{
				IVisualElementRenderer element = GetRenderer(view);
				return element.GetDesiredSize(widthConstraint, heightConstraint);
			}

			return new SizeRequest();
		}

		public static IVisualElementRenderer CreateRenderer(VisualElement element)
		{
			IVisualElementRenderer result = Registrar.Registered.GetHandler<IVisualElementRenderer>(element.GetType()) ?? new ViewRenderer();
			result.SetElement(element);
			return result;
		}

		public static IVisualElementRenderer GetRenderer(VisualElement self)
		{
			return (IVisualElementRenderer)self.GetValue(RendererProperty);
		}

		public static void SetRenderer(VisualElement self, IVisualElementRenderer renderer)
		{
			self.SetValue(RendererProperty, renderer);
			self.IsPlatformEnabled = renderer != null;
		}

		internal Canvas GetCanvas()
		{
			return _renderer;
		}

		internal async Task<Page> Pop(Page ancestor, bool animated)
		{
			Page result = _navModel.Pop(ancestor);

			Page navRoot = _navModel.Tree.Last().Skip(1).First();
			Page current = _navModel.CurrentPage;

			// The following code is a terrible horrible ugly hack that we are kind of stuck with for the time being
			// Effectively what can happen is a TabbedPage with many navigation page children needs to have all those children in the
			// nav stack. If you have multiple each of those roots needs to be skipped over.

			// In general the check for the NavigationPage will always hit if the check for the Skip(1) hits, but since that check
			// was always there it is left behind to ensure compatibility with previous behavior.
			bool replaceWithRoot = current == navRoot;
			var parent = current.Parent as NavigationPage;
			if (parent != null)
			{
				if (((IPageController)parent).InternalChildren[0] == current)
					replaceWithRoot = true;
			}

			if (replaceWithRoot)
				current = _navModel.Roots.Last(); // Navigation page itself, since nav root has a host

			await SetCurrent(current, animated, true);
			return result;
		}

		internal async Task PopToRoot(Page ancestor, bool animated)
		{
			_navModel.PopToRoot(ancestor);
			await SetCurrent(_navModel.CurrentPage, animated, true);
		}

		internal async Task PushCore(Page root, Page ancester, bool animated, bool realize = true)
		{
			_navModel.Push(root, ancester);
			if (realize)
				await SetCurrent(_navModel.CurrentPage, animated);

			if (root.NavigationProxy.Inner == null)
				root.NavigationProxy.Inner = this;
		}

		internal async void RemovePage(Page page, bool popCurrent)
		{
			if (popCurrent && _navModel.CurrentPage == page)
				await ((INavigation)this).PopAsync();
			else
				_navModel.RemovePage(page);
		}

		internal Task SetCurrent(Page page, bool animated, bool popping = false, Action completedCallback = null)
		{
			var tcs = new TaskCompletionSource<bool>();
			if (page == _currentDisplayedPage)
			{
				tcs.SetResult(true);
				return tcs.Task;
			}

			if (!animated)
				tcs.SetResult(true);

			page.Platform = this;

			if (GetRenderer(page) == null)
				SetRenderer(page, CreateRenderer(page));

			page.Layout(new Rectangle(0, 0, _renderer.ActualWidth, _renderer.ActualHeight));
			IVisualElementRenderer pageRenderer = GetRenderer(page);
			if (pageRenderer != null)
			{
				((FrameworkElement)pageRenderer.ContainerElement).Width = _renderer.ActualWidth;
				((FrameworkElement)pageRenderer.ContainerElement).Height = _renderer.ActualHeight;
			}

			Page current = _currentDisplayedPage;
			UIElement currentElement = null;
			if (current != null)
				currentElement = (UIElement)GetRenderer(current);

			//TODO: implement animation

			if (popping)
			{
				if (current != null)
				{
					_renderer.Children.Remove(currentElement);
				}

				var pageElement = (UIElement)GetRenderer(page);
				
				UpdateToolbarTracker();
				_renderer.Children.Add(pageElement);
				if (completedCallback != null)
					completedCallback();
			}
			else
			{
				if (current != null)
				{
					_renderer.Children.Remove(currentElement);
				}
				
				_renderer.Children.Add((UIElement)GetRenderer(page));
				UpdateToolbarTracker();
				if (completedCallback != null)
					completedCallback();
			}

			_currentDisplayedPage = page;

			return tcs.Task;
		}

		internal void SetPage(Page newRoot)
		{
			if (newRoot == null)
				return;

			Page = newRoot;
			_navModel.Clear();
			_navModel.PushModal(newRoot);
			SetCurrent(newRoot, false, true);

			((Application)newRoot.RealParent).NavigationProxy.Inner = this;
		}

		internal event EventHandler SizeChanged;

		void OnBackKeyPress(object sender, CancelEventArgs e)
		{
			if (_visibleMessageBox != null)
			{
				_visibleMessageBox.Dismiss();
				e.Cancel = true;
				return;
			}

			Page lastRoot = _navModel.Roots.Last();

			bool handled = lastRoot.SendBackButtonPressed();

			e.Cancel = handled;
		}

		Task Push(Page root, Page ancester, bool animated)
		{
			return PushCore(root, ancester, animated);
		}

		void RendererSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateFormSizes();
			EventHandler handler = SizeChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		void UpdateFormSizes()
		{
			foreach (Page f in _navModel.Roots)
			{
				f.Layout(new Rectangle(0, 0, _renderer.ActualWidth, _renderer.ActualHeight));
#pragma warning disable 618
				IVisualElementRenderer pageRenderer = f.GetRenderer();
#pragma warning restore 618
				if (pageRenderer != null)
				{
					((FrameworkElement)pageRenderer.ContainerElement).Width = _renderer.ActualWidth;
					((FrameworkElement)pageRenderer.ContainerElement).Height = _renderer.ActualHeight;
				}
			}
		}
		
		void UpdateToolbarTracker()
		{
			if (_navModel.Roots.Last() != null)
				_tracker.Target = _navModel.Roots.Last();
		}
		
	}
}