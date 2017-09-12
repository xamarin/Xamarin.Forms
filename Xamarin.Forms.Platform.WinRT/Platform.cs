﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Xamarin.Forms.Internals;

#if WINDOWS_UWP
namespace Xamarin.Forms.Platform.UWP
#else
namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public abstract partial class Platform : IPlatform, INavigation, IToolbarProvider
	{
		internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer",
			typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer));

		public static IVisualElementRenderer GetRenderer(VisualElement element)
		{
			return (IVisualElementRenderer)element.GetValue(RendererProperty);
		}

		public static void SetRenderer(VisualElement element, IVisualElementRenderer value)
		{
			element.SetValue(RendererProperty, value);
			element.IsPlatformEnabled = value != null;
		}

		public static IVisualElementRenderer CreateRenderer(VisualElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			IVisualElementRenderer renderer = Registrar.Registered.GetHandler<IVisualElementRenderer>(element.GetType()) ??
			                                  new DefaultRenderer();
			renderer.SetElement(element);
			return renderer;
		}

		internal Platform(Windows.UI.Xaml.Controls.Page page)
		{
			if (page == null)
				throw new ArgumentNullException(nameof(page));

			_page = page;

			_container = new Canvas
			{
				Style = (Windows.UI.Xaml.Style)Windows.UI.Xaml.Application.Current.Resources["RootContainerStyle"]
			};

			_page.Content = _container;

			_container.SizeChanged += OnRendererSizeChanged;

			MessagingCenter.Subscribe(this, Page.BusySetSignalName, (Page sender, bool enabled) =>
			{
				Windows.UI.Xaml.Controls.ProgressBar indicator = GetBusyIndicator();
				indicator.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
			});

			_toolbarTracker.CollectionChanged += OnToolbarItemsChanged;

			MessagingCenter.Subscribe<Page, AlertArguments>(this, Page.AlertSignalName, OnPageAlert);
			MessagingCenter.Subscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName, OnPageActionSheet);

			UpdateBounds();

#if WINDOWS_UWP
			InitializeStatusBar();
#endif
		}

		internal void SetPage(Page newRoot)
		{
			if (newRoot == null)
				throw new ArgumentNullException(nameof(newRoot));

			_navModel.Clear();

			_navModel.Push(newRoot, null);
			SetCurrent(newRoot, true);
			Application.Current.NavigationProxy.Inner = this;
		}

		public IReadOnlyList<Page> NavigationStack
		{
			get { return _navModel.Tree.Last(); }
		}

		public IReadOnlyList<Page> ModalStack
		{
			get { return _navModel.Modals.ToList(); }
		}

		Task INavigation.PushAsync(Page root)
		{
			return ((INavigation)this).PushAsync(root, true);
		}

		Task<Page> INavigation.PopAsync()
		{
			return ((INavigation)this).PopAsync(true);
		}

		Task INavigation.PopToRootAsync()
		{
			return ((INavigation)this).PopToRootAsync(true);
		}

		Task INavigation.PushAsync(Page root, bool animated)
		{
			throw new InvalidOperationException("PushAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		Task<Page> INavigation.PopAsync(bool animated)
		{
			throw new InvalidOperationException("PopAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		Task INavigation.PopToRootAsync(bool animated)
		{
			throw new InvalidOperationException(
				"PopToRootAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		void INavigation.RemovePage(Page page)
		{
			throw new InvalidOperationException("RemovePage is not supported globally on Windows, please use a NavigationPage.");
		}

		void INavigation.InsertPageBefore(Page page, Page before)
		{
			throw new InvalidOperationException(
				"InsertPageBefore is not supported globally on Windows, please use a NavigationPage.");
		}

		Task INavigation.PushModalAsync(Page page)
		{
			return ((INavigation)this).PushModalAsync(page, true);
		}

		Task<Page> INavigation.PopModalAsync()
		{
			return ((INavigation)this).PopModalAsync(true);
		}

		Task INavigation.PushModalAsync(Page page, bool animated)
		{
			if (page == null)
				throw new ArgumentNullException(nameof(page));

			var tcs = new TaskCompletionSource<bool>();
			_navModel.PushModal(page);
			SetCurrent(page, completedCallback: () => tcs.SetResult(true));
			return tcs.Task;
		}

		Task<Page> INavigation.PopModalAsync(bool animated)
		{
			var tcs = new TaskCompletionSource<Page>();
			Page result = _navModel.PopModal();
			SetCurrent(_navModel.CurrentPage, true, () => tcs.SetResult(result));
			return tcs.Task;
		}

		SizeRequest IPlatform.GetNativeSize(VisualElement element, double widthConstraint, double heightConstraint)
		{
			// Hack around the fact that Canvas ignores the child constraints.
			// It is entirely possible using Canvas as our base class is not wise.
			// FIXME: This should not be an if statement. Probably need to define an interface here.
			if (widthConstraint > 0 && heightConstraint > 0)
			{
				IVisualElementRenderer elementRenderer = GetRenderer(element);
				if (elementRenderer != null)
					return elementRenderer.GetDesiredSize(widthConstraint, heightConstraint);
			}

			return new SizeRequest();
		}

		internal virtual Rectangle ContainerBounds
		{
			get { return _bounds; }
		}

		internal void UpdatePageSizes()
		{
			Rectangle bounds = ContainerBounds;
			if (bounds.IsEmpty)
				return;
			foreach (Page root in _navModel.Roots)
			{
                // Don't resize every page, it will let previous page visible in new design page display mode
                if (root == _currentPage)
                {
                    if (root.Width != bounds.Width || root.Height != bounds.Height)
                        root.Layout(bounds);
                    IVisualElementRenderer renderer = GetRenderer(root);
                    if (renderer != null)
                    {
                        if (renderer.ContainerElement.Width != _container.ActualWidth)
                            renderer.ContainerElement.Width = _container.ActualWidth;
                        if (renderer.ContainerElement.Height != _container.ActualHeight)
                        renderer.ContainerElement.Height = _container.ActualHeight;
                    }
                }
			}
		}

		Rectangle _bounds;
		readonly Canvas _container;
		readonly Windows.UI.Xaml.Controls.Page _page;
		Windows.UI.Xaml.Controls.ProgressBar _busyIndicator;
		Page _currentPage;
		readonly NavigationModel _navModel = new NavigationModel();
		readonly ToolbarTracker _toolbarTracker = new ToolbarTracker();
		readonly FileImageSourcePathConverter _fileImageSourcePathConverter = new FileImageSourcePathConverter();


		Windows.UI.Xaml.Controls.ProgressBar GetBusyIndicator()
		{
			if (_busyIndicator == null)
			{
				_busyIndicator = new Windows.UI.Xaml.Controls.ProgressBar
				{
					IsIndeterminate = true,
					Visibility = Visibility.Collapsed,
					VerticalAlignment = VerticalAlignment.Top
				};

				Canvas.SetZIndex(_busyIndicator, 1);
				_container.Children.Add(_busyIndicator);
			}

			return _busyIndicator;
		}

		internal bool BackButtonPressed()
		{
			if (_currentActionSheet != null)
			{
				CancelActionSheet();
				return true;
			}

			Page lastRoot = _navModel.Roots.Last();

			bool handled = lastRoot.SendBackButtonPressed();

			if (!handled && _navModel.Tree.Count > 1)
			{
				Page removed = _navModel.PopModal();
				if (removed != null)
				{
					SetCurrent(_navModel.CurrentPage, true);
					handled = true;
				}
			}

			return handled;
		}

		void CancelActionSheet()
		{
			if (_currentActionSheet == null)
				return;

			_actionSheetOptions.SetResult(null);
			_actionSheetOptions = null;
			_currentActionSheet.IsOpen = false;
			_currentActionSheet = null;
		}

		void OnRendererSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			UpdateBounds();
			UpdatePageSizes();
		}

		async void SetCurrent(Page newPage, bool popping = false, Action completedCallback = null)
        {
            if (newPage == _currentPage)
                return;

            newPage.Platform = this;

            // The new changes of this method, for : App.MainPag = existPage;

            if (_currentPage != null)
            {
                Page previousPage = _currentPage;
                IVisualElementRenderer previousRenderer = GetRenderer(previousPage);


                // Don't clean if RetainsRenderer is true
                if (popping && !previousPage.GetRetainsRendererValue())
                {
                    previousPage.Cleanup();
                    _container.Children.Remove(previousRenderer.ContainerElement);
                }
                else
                {
                    // We should just hide it, not remove or add again, that cause much performance
                    //_container.Children.Remove(previousRenderer.ContainerElement);
                    previousRenderer.ContainerElement.Visibility = Visibility.Collapsed;
                }
            }


			newPage.Layout(ContainerBounds);
            IVisualElementRenderer pageRenderer = newPage.GetOrCreateRenderer();

            if (null != _currentPage)
                ((IPageController)_currentPage)?.SendDisappearing();

            // Only if the page are new, we should add it, once only(avoid performance)
            if (_container.Children.Any( x => x == pageRenderer.ContainerElement))
                pageRenderer.ContainerElement.Visibility = Visibility.Visible;
            else
                _container.Children.Add(pageRenderer.ContainerElement);

            if (_container.Children.Any(x => x == pageRenderer.ContainerElement))
                ((IPageController)newPage)?.SendAppearing();

            pageRenderer.ContainerElement.Width = _container.ActualWidth;
            pageRenderer.ContainerElement.Height = _container.ActualHeight;

			completedCallback?.Invoke();

            _currentPage = newPage;

            UpdateToolbarTracker();
#if WINDOWS_UWP
            UpdateToolbarTitle(newPage);
#endif
            UpdatePageSizes();

            // When App.MainPage navigate to exist navigation page, maybe in UWP Desktop App lose Back button on the top, That's why I add it
            if (pageRenderer is NavigationPageRenderer)
                ((NavigationPageRenderer)pageRenderer).UpdateBackButton();

            await UpdateToolbarItems();

        }

        

		Task<CommandBar> IToolbarProvider.GetCommandBarAsync()
		{
			return GetCommandBarAsync();
		}

		async void OnToolbarItemsChanged(object sender, EventArgs e)
		{
			await UpdateToolbarItems();
		}

		void UpdateToolbarTracker()
		{
			Page last = _navModel.Roots.Last();
			if (last != null)
				_toolbarTracker.Target = last;
		}

		ActionSheetArguments _actionSheetOptions;
		Popup _currentActionSheet;

		async void OnPageAlert(Page sender, AlertArguments options)
		{
			string content = options.Message ?? options.Title ?? string.Empty;

			MessageDialog dialog;
			if (options.Message == null || options.Title == null)
				dialog = new MessageDialog(content);
			else
				dialog = new MessageDialog(options.Message, options.Title);

			if (options.Accept != null)
			{
				dialog.Commands.Add(new UICommand(options.Accept));
				dialog.DefaultCommandIndex = 0;
			}

			if (options.Cancel != null)
			{
				dialog.Commands.Add(new UICommand(options.Cancel));
				dialog.CancelCommandIndex = (uint)dialog.Commands.Count - 1;
			}

			if (Device.IsInvokeRequired)
			{
				Device.BeginInvokeOnMainThread(async () =>
				{
					IUICommand command = await dialog.ShowAsyncQueue();
					options.SetResult(command.Label == options.Accept);
				});
			}
			else
			{
				IUICommand command = await dialog.ShowAsyncQueue();
				options.SetResult(command.Label == options.Accept);
			}
		}
	}

	// refer to http://stackoverflow.com/questions/29209954/multiple-messagedialog-app-crash for why this is used
	// in order to allow for multiple MessageDialogs, or a crash occurs otherwise
	public static class MessageDialogExtensions
	{
		static TaskCompletionSource<MessageDialog> _currentDialogShowRequest;

		public static async Task<IUICommand> ShowAsyncQueue(this MessageDialog dialog)
		{
			while (_currentDialogShowRequest != null)
			{
				await _currentDialogShowRequest.Task;
			}

			TaskCompletionSource<MessageDialog> request = _currentDialogShowRequest = new TaskCompletionSource<MessageDialog>();
			IUICommand result = await dialog.ShowAsync();
			_currentDialogShowRequest = null;
			request.SetResult(dialog);

			return result;
		}
	}
}