using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.UWP
{
	// Responsible for rendering the content title, as well as the bottom bar list of shell sections
	internal class ShellItemRenderer : Windows.UI.Xaml.Controls.Grid, IAppearanceObserver, IFlyoutBehaviorObserver
	{
		ShellSectionRenderer SectionRenderer { get; }
		Windows.UI.Xaml.Controls.TextBlock _Title;
		Windows.UI.Xaml.Controls.Border _BottomBarArea;
		Windows.UI.Xaml.Controls.Grid _BottomBar;
		Windows.UI.Xaml.Controls.Grid _HeaderArea;
		Windows.UI.Xaml.Controls.ItemsControl _Toolbar;

		internal ShellItem ShellItem { get; private set; }

		internal ShellRenderer ShellContext { get; private set; }

		public ShellItemRenderer()
		{
			RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(40, Windows.UI.Xaml.GridUnitType.Pixel) });
			RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
			RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });

			_Title = new Windows.UI.Xaml.Controls.TextBlock()
			{
				Style = Resources["SubtitleTextBlockStyle"] as Windows.UI.Xaml.Style,
				VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
				TextTrimming = Windows.UI.Xaml.TextTrimming.CharacterEllipsis,
				TextWrapping = Windows.UI.Xaml.TextWrapping.NoWrap
			};
			_HeaderArea = new Windows.UI.Xaml.Controls.Grid() { Padding = new Windows.UI.Xaml.Thickness(10,0,10,0) };
			_HeaderArea.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
			_HeaderArea.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
			_HeaderArea.Children.Add(_Title);
			Children.Add(_HeaderArea);

			_Toolbar = new Windows.UI.Xaml.Controls.ItemsControl()
			{
				ItemTemplate = Windows.UI.Xaml.Application.Current.Resources["ShellToolbarItemTemplate"] as Windows.UI.Xaml.DataTemplate,
				ItemsPanel = Windows.UI.Xaml.Application.Current.Resources["ShellToolbarItemsPanelTemplate"] as Windows.UI.Xaml.Controls.ItemsPanelTemplate,
			};
			SetColumn(_Toolbar, 1);
			_HeaderArea.Children.Add(_Toolbar);

			SectionRenderer = new ShellSectionRenderer();
			SetRow(SectionRenderer, 1);

			Children.Add(SectionRenderer);

			_BottomBar = new Windows.UI.Xaml.Controls.Grid() { HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center };
			_BottomBarArea = new Windows.UI.Xaml.Controls.Border() { Child = _BottomBar };
			SetRow(_BottomBarArea, 2);
			Children.Add(_BottomBarArea);
		}

		internal void SetShellContext(ShellRenderer context)
		{
			if (ShellContext != null)
			{
				((IShellController)ShellContext.Shell).RemoveAppearanceObserver(this);
				((IShellController)ShellContext.Shell).RemoveFlyoutBehaviorObserver(this);
			}
			ShellContext = context;
			if (ShellContext != null)
			{
				((IShellController)ShellContext.Shell).AddFlyoutBehaviorObserver(this);
				((IShellController)ShellContext.Shell).AddAppearanceObserver(this, ShellContext.Shell);
				UpdateHeaderInsets();
			}
		}

		internal void NavigateToShellItem(ShellItem newItem, bool animate)
		{
			UnhookEvents(ShellItem);
			ShellItem = newItem;
			ShellSection = newItem.CurrentItem;
			HookEvents(newItem);
		}

		internal void UpdateHeaderInsets()
		{
			double inset = 10;
			if (ShellContext.IsPaneToggleButtonVisible)
				inset += 45;
			if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.NavigationView", "IsBackButtonVisible"))
			{
			if (ShellContext.IsBackButtonVisible != Windows.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed)
				inset += 45;
			}
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
						Label = section.Title, Width = double.NaN, MinWidth = 68, MaxWidth = 200
					};
					if (section.Icon is FileImageSource fis)
						btn.Icon = new Windows.UI.Xaml.Controls.BitmapIcon() { UriSource = new Uri("ms-appx:///" + fis.File) };
					btn.Click += (s, e) => OnShellSectionClicked(section);
					_BottomBar.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
					SetColumn(btn, i);
					_BottomBar.Children.Add(btn);
				}
			}
		}

		void OnShellSectionClicked(ShellSection shellSection)
		{
			if (shellSection != null)
				((IShellItemController)ShellItem).ProposeSection(shellSection);
		}

		protected virtual bool ChangeSection(ShellSection shellSection)
		{
			return ((IShellItemController)ShellItem).ProposeSection(shellSection);
		}

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance) => UpdateAppearance(appearance);
		void UpdateAppearance(ShellAppearance appearance)
		{
			var tabBarBackgroundColor = ShellRenderer.DefaultBackgroundColor;
			var tabBarForegroundColor = ShellRenderer.DefaultForegroundColor;
			var titleColor = ShellRenderer.DefaultTitleColor;
			if (appearance != null)
			{
				var a = (IShellAppearanceElement)appearance;
				tabBarBackgroundColor = a.EffectiveTabBarBackgroundColor.ToWindowsColor();
				tabBarForegroundColor = a.EffectiveTabBarForegroundColor.ToWindowsColor();
				if(!appearance.TitleColor.IsDefault)
					titleColor = appearance.TitleColor.ToWindowsColor();
			}
			_BottomBarArea.Background = _HeaderArea.Background = 
				new Windows.UI.Xaml.Media.SolidColorBrush(tabBarBackgroundColor);
			_Title.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(titleColor);
			var tabbarForeground = new Windows.UI.Xaml.Media.SolidColorBrush(tabBarForegroundColor);
			foreach (var button in _BottomBar.Children.OfType<Windows.UI.Xaml.Controls.AppBarButton>())
				button.Foreground = tabbarForeground;
			if (SectionRenderer is IAppearanceObserver iao)
				iao.OnAppearanceChanged(appearance);
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
		}

		protected virtual void UnhookChildEvents(ShellSection shellSection)
		{
			((IShellSectionController)shellSection).NavigationRequested -= OnNavigationRequested;
		}

		protected virtual void OnShellItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ShellItem.CurrentItemProperty.PropertyName)
				ShellSection = ShellItem.CurrentItem;
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
			UpdateBottomBar();
		}

		protected virtual void OnShellSectionChanged(ShellSection oldSection, ShellSection newSection)
		{
			SwitchSection(ShellNavigationSource.ShellSectionChanged, newSection, null, oldSection != null);
		}

		void SwitchSection(ShellNavigationSource source, ShellSection section, Page page, bool animate = true)
		{
			SectionRenderer.NavigateToShellSection(section, page, animate);			
	    }

		Page DisplayedPage { get; set; }

		void UpdateDisplayedPage(Page page)
		{
			if(DisplayedPage != null)
			{
				DisplayedPage.PropertyChanged -= OnPagePropertyChanged;
			}
			DisplayedPage = page;
			if (DisplayedPage != null)
			{
				DisplayedPage.PropertyChanged += OnPagePropertyChanged;
			}
			UpdateBottomBarVisibility();
			UpdatePageTitle();
			UpdateToolbar();
		}

		private void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == Shell.TabBarIsVisibleProperty.PropertyName)
			{
				UpdateBottomBarVisibility();
			}
			else if (e.PropertyName == Page.TitleProperty.PropertyName)
			{
				UpdatePageTitle();
			}
		}

		private void UpdatePageTitle()
		{
			_Title.Text = DisplayedPage?.Title ?? ShellSection?.Title ?? "";
		}

		private void UpdateBottomBarVisibility()
		{
			_BottomBar.Visibility = DisplayedPage == null || Shell.GetTabBarIsVisible(DisplayedPage) ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
		}

		private void UpdateToolbar()
		{
			_Toolbar.ItemsSource = DisplayedPage?.ToolbarItems;
		}

		void OnNavigationRequested(object sender, NavigationRequestedEventArgs e)
		{
			SwitchSection((ShellNavigationSource)e.RequestType, (ShellSection)sender, e.Page, e.Animated);	
		}

		void IFlyoutBehaviorObserver.OnFlyoutBehaviorChanged(FlyoutBehavior behavior)
		{
			UpdateHeaderInsets();
		}
	}
}
