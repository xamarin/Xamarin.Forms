using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.UWP
{
	internal class ShellItemRenderer : Windows.UI.Xaml.Controls.Page, IAppearanceObserver
	{
		Windows.UI.Xaml.Controls.Frame _Frame { get; }
		Windows.UI.Xaml.Controls.NavigationView View;
		Windows.UI.Xaml.Controls.TextBlock _Title;
		Windows.UI.Xaml.Controls.Border _BottomBarArea;
		Windows.UI.Xaml.Controls.Grid _BottomBar;
		Windows.UI.Xaml.Controls.Border _HeaderArea;
		Windows.UI.Xaml.Controls.AutoSuggestBox _SearchBox;
		internal ShellItem ShellItem { get; private set; }
		internal ShellRenderer ShellContext { get; private set; }

		public ShellItemRenderer()
		{
			var root = new Windows.UI.Xaml.Controls.Grid();
			root.RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(40, Windows.UI.Xaml.GridUnitType.Pixel) });
			root.RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
			root.RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
			_HeaderArea = new Windows.UI.Xaml.Controls.Border();

			_Title = new Windows.UI.Xaml.Controls.TextBlock() { Text = "Title area here",
				Padding = new Windows.UI.Xaml.Thickness(10),
				Style = Resources["SubtitleTextBlockStyle"] as Windows.UI.Xaml.Style, VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center };
			_HeaderArea.Child = _Title;
			root.Children.Add(_HeaderArea);

			_SearchBox = new Windows.UI.Xaml.Controls.AutoSuggestBox() { Width = 300 };
			_SearchBox.TextChanged += SearchBox_TextChanged;
			_SearchBox.QuerySubmitted += SearchBox_QuerySubmitted;
			_SearchBox.SuggestionChosen += SearchBox_SuggestionChosen;
			View = new Windows.UI.Xaml.Controls.NavigationView()
			{
				IsBackButtonVisible = Windows.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed,
				IsSettingsVisible = false,
				AlwaysShowHeader = false,
				PaneDisplayMode = Windows.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top,
				AutoSuggestBox = _SearchBox
			};
			View.ItemInvoked += MenuItemInvoked;

			var template = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><NavigationViewItem Content=""{Binding Title}""/></DataTemplate>";
			var datatemplate = (Windows.UI.Xaml.DataTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(template);
			View.MenuItemTemplate = datatemplate;
			View.Content = _Frame = new Windows.UI.Xaml.Controls.Frame();
			Windows.UI.Xaml.Controls.Grid.SetRow(View, 1);

			root.Children.Add(View);

			_BottomBar = new Windows.UI.Xaml.Controls.Grid() { HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center };
			_BottomBarArea = new Windows.UI.Xaml.Controls.Border() { Child = _BottomBar };
			Windows.UI.Xaml.Controls.Grid.SetRow(_BottomBarArea, 2);
			root.Children.Add(_BottomBarArea);

			Content = root;
		}

		internal bool CanGoBack => _Frame.CanGoBack || _Frame.Content is ShellSectionRenderer r && r.CanGoBack;

		internal void GoBack()
		{
			if (_Frame.Content is ShellSectionRenderer r && r.CanGoBack)
				r.GoBack();
			else
				_Frame.GoBack();
		}

		internal static object CreateNavigationArgs(ShellRenderer context, ShellItem shellItem) => new object[] { context, shellItem };

		protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			var args = e.Parameter as object[];
			ShellContext = args[0] as ShellRenderer;
			var newItem = args[1] as ShellItem;
			ShellItem = newItem;
			ShellSection = newItem.CurrentItem;
			ShellContext.DisplayModeChanged += ShellContext_DisplayModeChanged;
			UpdateHeaderInsets(ShellContext.DisplayMode);
			HookEvents(newItem);
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			ShellContext.DisplayModeChanged -= ShellContext_DisplayModeChanged;
			UnhookEvents(ShellItem);
			_Frame.Content = null;
			base.OnNavigatingFrom(e);
		}

		private void ShellContext_DisplayModeChanged(Windows.UI.Xaml.Controls.NavigationView sender, Windows.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
		{
			UpdateHeaderInsets(args.DisplayMode);
		}

		void UpdateHeaderInsets(Windows.UI.Xaml.Controls.NavigationViewDisplayMode displayMode)
		{ 
			var inset = (displayMode == Windows.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal) ? 100 : 0;
			_HeaderArea.Padding = new Windows.UI.Xaml.Thickness(inset, 0, 0, 0);
		}

		void UpdateBottomBar()
		{
			_BottomBar.Children.Clear();
			_BottomBar.ColumnDefinitions.Clear();
			if (ShellItem?.Items.Count > 1)
			{
				for (int i = 0; i < ShellItem.Items.Count; i++)
				{
					var section = ShellItem.Items[i];
					var btn = new Windows.UI.Xaml.Controls.AppBarButton()
					{
						Label = section.Title
					};
					if (section.Icon is FileImageSource fis)
						btn.Icon = new Windows.UI.Xaml.Controls.BitmapIcon() { UriSource = new Uri("ms-appx:///" + fis.File) };
					btn.Click += (s, e) => OnShellSectionClicked(section);
					_BottomBar.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
					Windows.UI.Xaml.Controls.Grid.SetColumn(btn, i);
					_BottomBar.Children.Add(btn);
				}
			}
		}

		void OnShellSectionClicked(ShellSection shellSection)
		{
			if (shellSection != null)
				((IShellItemController)ShellItem).ProposeSection(shellSection);
		}

		void MenuItemInvoked(Windows.UI.Xaml.Controls.NavigationView sender, Windows.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
		{
			var shellContent = args.InvokedItemContainer?.DataContext as ShellContent;
			var result = ((IShellController)ShellItem.RealParent).ProposeNavigation(ShellNavigationSource.Pop, ShellItem, ShellSection, shellContent, null, true);
			if (result)
			{
				ShellSection.SetValueFromRenderer(ShellSection.CurrentItemProperty, shellContent);
			}
		}

		protected virtual bool ChangeSection(ShellSection shellSection)
		{
			return ((IShellItemController)ShellItem).ProposeSection(shellSection);
		}

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance) => UpdateAppearance(appearance);
		void UpdateAppearance(ShellAppearance appearance)
		{
			var a = (IShellAppearanceElement)appearance;
			var tabBackground = new Windows.UI.Xaml.Media.SolidColorBrush(a.EffectiveTabBarBackgroundColor.ToWindowsColor());
			_BottomBarArea.Background = _HeaderArea.Background = tabBackground;
			_Title.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(appearance.TitleColor.ToWindowsColor());
			var tabbarForeground = new Windows.UI.Xaml.Media.SolidColorBrush(a.EffectiveTabBarForegroundColor.ToWindowsColor());
			foreach (var button in _BottomBar.Children.OfType<Windows.UI.Xaml.Controls.AppBarButton>())
				button.Foreground = tabbarForeground;

			View.Resources["NavigationViewTopPaneBackground"] = tabBackground;
			View.Resources["TopNavigationViewItemForeground"] = tabbarForeground;
			View.Resources["TopNavigationViewItemForegroundSelected"] = tabbarForeground;
			View.Resources["NavigationViewSelectionIndicatorForeground"] = tabbarForeground;
		}
		#endregion

		ShellSection _shellSection;
		protected ShellSection ShellSection
		{
			get => _shellSection;
			set
			{
				if (_shellSection == value)
					return;
				var oldValue = _shellSection;
				if (_shellSection != null)
				{
					((IShellSectionController)_shellSection).RemoveDisplayedPageObserver(this);
					View.MenuItemsSource = null;
				}

				_shellSection = value;
				if (value != null)
				{
					OnShellSectionChanged(oldValue, value);
					((IShellSectionController)ShellSection).AddDisplayedPageObserver(this, UpdateDisplayedPage);
				}
				UpdateBottomBar();
			}
		}

		void HookEvents(ShellItem shellItem)
		{
			shellItem.PropertyChanged += OnShellItemPropertyChanged;
			((INotifyCollectionChanged)shellItem.Items).CollectionChanged += OnShellItemsChanged;
			foreach (var shellSection in shellItem.Items)
			{
				HookChildEvents(shellSection);
			}
		}

		protected virtual void UnhookEvents(ShellItem shellItem)
		{
			if (shellItem != null)
			{
				foreach (var shellSection in shellItem.Items)
				{
					UnhookChildEvents(shellSection);
				}
				((INotifyCollectionChanged)shellItem.Items).CollectionChanged -= OnShellItemsChanged;
				ShellItem.PropertyChanged -= OnShellItemPropertyChanged;
				ShellSection = null;
				ShellItem = null;
			}
		}

		void HookChildEvents(ShellSection shellSection)
		{
			((IShellSectionController)shellSection).NavigationRequested += OnNavigationRequested;
			shellSection.PropertyChanged += OnShellSectionPropertyChanged;
		}

		protected virtual void UnhookChildEvents(ShellSection shellSection)
		{
			((IShellSectionController)shellSection).NavigationRequested -= OnNavigationRequested;
			shellSection.PropertyChanged -= OnShellSectionPropertyChanged;
		}
		protected virtual void OnShellItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ShellItem.CurrentItemProperty.PropertyName)
				ShellSection = ShellItem.CurrentItem;
		}

		void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ShellSection.CurrentItemProperty.PropertyName)
			{
				OnShellContentUpdated(ShellSection.CurrentItem);
			}
		}

		void OnShellContentUpdated(ShellContent shellContent)
		{
			//shellContent.Search
		}

		protected virtual void OnShellItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (ShellSection shellSection in e.OldItems)
					UnhookChildEvents(shellSection);
			}

			if (e.NewItems != null)
			{
				foreach (ShellSection shellSection in e.NewItems)
					HookChildEvents(shellSection);
			}
		}

		protected virtual void OnShellSectionChanged(ShellSection oldSection, ShellSection newSection)
		{
			SwitchSection(ShellNavigationSource.ShellSectionChanged, newSection, null, oldSection != null);
		}

		void SwitchSection(ShellNavigationSource source, ShellSection section, Page page, bool animate = true)
		{
			Windows.UI.Xaml.Media.Animation.NavigationTransitionInfo info;
			if (animate)
				info = new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo();
			else
				info = new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo();
			_Frame.Navigate(typeof(ShellSectionRenderer), section, info);
			View.SelectedItem = null;
			View.IsPaneVisible = section.Items.Count > 1;
			View.MenuItemsSource = section.Items;
			View.SelectedItem = section.CurrentItem;
			OnShellContentUpdated(section.CurrentItem);
	    }

		Page DisplayedPage { get; set; }

		void UpdateDisplayedPage(Page page)
		{
			DisplayedPage = page;
			_Title.Text = page?.Title ?? ShellSection?.Title ?? "";
			UpdateSearchHandler(Shell.GetSearchHandler(page));
			
		}

		#region Search

		SearchHandler _currentSearchHandler;
		void UpdateSearchHandler(SearchHandler searchHandler)
		{
			if(_currentSearchHandler != null)
			{
				_currentSearchHandler.PropertyChanged -= SearchHandler_PropertyChanged;
			}
			_currentSearchHandler = searchHandler;
			if (searchHandler != null)
			{
				searchHandler.PropertyChanged += SearchHandler_PropertyChanged;
				_SearchBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
				_SearchBox.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
				_SearchBox.PlaceholderText = searchHandler.Placeholder;
				_SearchBox.IsEnabled = searchHandler.IsSearchEnabled;
				_SearchBox.ItemsSource = _currentSearchHandler.ItemsSource;
				View.IsPaneVisible = true;
			}
			else
			{
				View.IsPaneVisible = ShellSection.Items.Count > 1;
				_SearchBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			}
		}

		void SearchHandler_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == SearchHandler.PlaceholderProperty.PropertyName)
			{
				_SearchBox.PlaceholderText = _currentSearchHandler.Placeholder;
			}
			else if (e.PropertyName == SearchHandler.IsSearchEnabledProperty.PropertyName)
			{
				_SearchBox.IsEnabled = _currentSearchHandler.IsSearchEnabled;
			}
			else if (e.PropertyName == SearchHandler.ItemsSourceProperty.PropertyName)
			{
				_SearchBox.ItemsSource = _currentSearchHandler.ItemsSource;
			}
			else if (e.PropertyName == SearchHandler.QueryProperty.PropertyName)
			{
				_SearchBox.Text = _currentSearchHandler.Query;
			}
		}

		void SearchBox_TextChanged(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxTextChangedEventArgs args)
		{
			if(args.Reason != Windows.UI.Xaml.Controls.AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
				_currentSearchHandler.Query = sender.Text;
		}

		void SearchBox_SuggestionChosen(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			((ISearchHandlerController)_currentSearchHandler).ItemSelected(args.SelectedItem);
		}

		void SearchBox_QuerySubmitted(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			((ISearchHandlerController)_currentSearchHandler).QueryConfirmed();
		}

		#endregion Search
		void OnNavigationRequested(object sender, NavigationRequestedEventArgs e)
		{
			SwitchSection((ShellNavigationSource)e.RequestType, (ShellSection)sender, e.Page, e.Animated);	
		}
	}
}
