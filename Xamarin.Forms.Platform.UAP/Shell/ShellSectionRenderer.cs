using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	// Renders the actual page area where the contents gets rendered, as well as set of optional top-bar menu items and search box.
	internal class ShellSectionRenderer : Windows.UI.Xaml.Controls.NavigationView, IAppearanceObserver
	{
		Windows.UI.Xaml.Controls.Frame Frame { get; }
		Page Page;
		ShellContent CurrentContent;
		ShellSection ShellSection;

		public ShellSectionRenderer()
		{
			MenuItemTemplate = (Windows.UI.Xaml.DataTemplate)Windows.UI.Xaml.Application.Current.Resources["ShellSectionMenuItemTemplate"];
			IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
			IsSettingsVisible = false;
			AlwaysShowHeader = false;
			PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
			ItemInvoked += MenuItemInvoked;

			AutoSuggestBox = new Windows.UI.Xaml.Controls.AutoSuggestBox() { Width = 300 };
			AutoSuggestBox.TextChanged += SearchBox_TextChanged;
			AutoSuggestBox.QuerySubmitted += SearchBox_QuerySubmitted;
			AutoSuggestBox.SuggestionChosen += SearchBox_SuggestionChosen;

			Frame = new Windows.UI.Xaml.Controls.Frame();
			Content = Frame;
			this.SizeChanged += ShellSectionRenderer_SizeChanged;
			Resources["NavigationViewTopPaneBackground"] = new Windows.UI.Xaml.Media.SolidColorBrush(ShellRenderer.DefaultBackgroundColor);
			Resources["TopNavigationViewItemForeground"] = new Windows.UI.Xaml.Media.SolidColorBrush(ShellRenderer.DefaultForegroundColor);
			Resources["TopNavigationViewItemForegroundSelected"] = new Windows.UI.Xaml.Media.SolidColorBrush(ShellRenderer.DefaultForegroundColor);
			Resources["NavigationViewSelectionIndicatorForeground"] = new Windows.UI.Xaml.Media.SolidColorBrush(ShellRenderer.DefaultForegroundColor);
		}

		void ShellSectionRenderer_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
		{
			Page.ContainerArea = new Rectangle(0, 0, e.NewSize.Width, e.NewSize.Height);
		}

		void MenuItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			var shellContent = args.InvokedItemContainer?.DataContext as ShellContent;
			var shellItem = ShellSection.RealParent as ShellItem;
			var result = ((IShellController)shellItem.RealParent).ProposeNavigation(ShellNavigationSource.Pop, shellItem, ShellSection, shellContent, null, true);
			if (result)
			{
				ShellSection.SetValueFromRenderer(ShellSection.CurrentItemProperty, shellContent);
			}
		}

		internal void NavigateToShellSection(ShellSection section, Page page, bool animate = true)
		{
			if(ShellSection != null)
			{
				ShellSection.PropertyChanged -= OnShellSectionPropertyChanged;
				ShellSection = null;
				MenuItemsSource = null;
			}
			ShellSection = section;
			ShellSection.PropertyChanged += OnShellSectionPropertyChanged;
			SelectedItem = null;
			IsPaneVisible = section.Items.Count > 1;
			MenuItemsSource = section.Items;
			SelectedItem = section.CurrentItem;
			NavigateToContent(section.CurrentItem, animate);
		}

		void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ShellSection.CurrentItemProperty.PropertyName)
			{
				NavigateToContent(ShellSection.CurrentItem);
			}
		}

		internal void NavigateToContent(ShellContent shellContent, bool animate = true)
		{
			if (CurrentContent != null && Page != null)
				((IShellContentController)CurrentContent).RecyclePage(Page);
			CurrentContent = shellContent;
			if (shellContent != null)
			{
				Page = ((IShellContentController)shellContent).GetOrCreateContent();
				Frame.Navigate((ContentPage)Page);
				UpdateSearchHandler(Shell.GetSearchHandler(Page));
			}
		}

		#region Search

		SearchHandler _currentSearchHandler;

		void UpdateSearchHandler(SearchHandler searchHandler)
		{
			if (_currentSearchHandler != null)
			{
				_currentSearchHandler.PropertyChanged -= SearchHandler_PropertyChanged;
			}
			_currentSearchHandler = searchHandler;
			if (AutoSuggestBox == null)
				return;
			if (searchHandler != null)
			{
				searchHandler.PropertyChanged += SearchHandler_PropertyChanged;
				AutoSuggestBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
				AutoSuggestBox.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
				AutoSuggestBox.PlaceholderText = searchHandler.Placeholder;
				AutoSuggestBox.IsEnabled = searchHandler.IsSearchEnabled;
				AutoSuggestBox.ItemsSource = _currentSearchHandler.ItemsSource;
				IsPaneVisible = true;
			}
			else
			{
				IsPaneVisible = ShellSection.Items.Count > 1;
				AutoSuggestBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			}
		}

		void SearchHandler_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (AutoSuggestBox == null)
				return;
			if (e.PropertyName == SearchHandler.PlaceholderProperty.PropertyName)
			{
				AutoSuggestBox.PlaceholderText = _currentSearchHandler.Placeholder;
			}
			else if (e.PropertyName == SearchHandler.IsSearchEnabledProperty.PropertyName)
			{
				AutoSuggestBox.IsEnabled = _currentSearchHandler.IsSearchEnabled;
			}
			else if (e.PropertyName == SearchHandler.ItemsSourceProperty.PropertyName)
			{
				AutoSuggestBox.ItemsSource = _currentSearchHandler.ItemsSource;
			}
			else if (e.PropertyName == SearchHandler.QueryProperty.PropertyName)
			{
				AutoSuggestBox.Text = _currentSearchHandler.Query;
			}
		}

		void SearchBox_TextChanged(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxTextChangedEventArgs args)
		{
			if (args.Reason != Windows.UI.Xaml.Controls.AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
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

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance) => UpdateAppearance(appearance);

		void UpdateAppearance(ShellAppearance appearance)
		{
			var tabBarBackgroundColor = ShellRenderer.DefaultBackgroundColor;
			var tabBarForegroundColor = ShellRenderer.DefaultForegroundColor;
			if (appearance != null)
			{
				var a = (IShellAppearanceElement)appearance;
				tabBarBackgroundColor = a.EffectiveTabBarBackgroundColor.ToWindowsColor();
				tabBarForegroundColor = a.EffectiveTabBarForegroundColor.ToWindowsColor();
			}

			UpdateBrushColor("NavigationViewTopPaneBackground", tabBarBackgroundColor);
			UpdateBrushColor("TopNavigationViewItemForeground", tabBarForegroundColor);
			UpdateBrushColor("TopNavigationViewItemForegroundSelected", tabBarForegroundColor);
			UpdateBrushColor("NavigationViewSelectionIndicatorForeground", tabBarForegroundColor);
		}

		void UpdateBrushColor(string resourceKey, Windows.UI.Color color)
		{
			if (Resources[resourceKey] is Windows.UI.Xaml.Media.SolidColorBrush sb)
				sb.Color = color;
		}

		#endregion
	}
}
