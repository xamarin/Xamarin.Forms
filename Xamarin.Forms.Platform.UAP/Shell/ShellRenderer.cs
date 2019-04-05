using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using UFrame = Windows.UI.Xaml.Controls.Frame;
using UGrid = Windows.UI.Xaml.Controls.Grid;
using URowDefinition = Windows.UI.Xaml.Controls.RowDefinition;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellRenderer : SplitView, IVisualElementRenderer, /*IShellContext, */IAppearanceObserver
	{
		public static readonly Color DefaultBackgroundColor = Color.FromRgb(33, 150, 243);
		public static readonly Color DefaultForegroundColor = Color.White;
		public static readonly Color DefaultTitleColor = Color.White;
		public static readonly Color DefaultUnselectedColor = Color.FromRgba(255, 255, 255, 180);

		//bool _disposed;
		ShellFlyoutRenderer _flyoutRenderer;
		UFrame _frameLayout;

		public ShellRenderer()
		{
		}

		#region IVisualElementRenderer

		event EventHandler<VisualElementChangedEventArgs> _elementChanged;

		event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged
		{
			add { _elementChanged += value; }
			remove { _elementChanged -= value; }
		}

		FrameworkElement IVisualElementRenderer.ContainerElement => this;

		VisualElement IVisualElementRenderer.Element => Element;

		//VisualElementTracker IVisualElementRenderer.Tracker => null;

		//AView IVisualElementRenderer.View => _flyoutRenderer.AndroidView;
		//ViewGroup IVisualElementRenderer.ViewGroup => _flyoutRenderer.AndroidView as ViewGroup;

		SizeRequest IVisualElementRenderer.GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var constraint = new Windows.Foundation.Size(widthConstraint, heightConstraint);

			double oldWidth = Width;
			double oldHeight = Height;

			Height = double.NaN;
			Width = double.NaN;

			Measure(constraint);
			var result = new Size(Math.Ceiling(DesiredSize.Width), Math.Ceiling(DesiredSize.Height));

			Width = oldWidth;
			Height = oldHeight;

			return new SizeRequest(result);
		}

		public UIElement GetNativeElement() => null;

		public void Dispose()
		{
			SetElement(null);
			//_disposed = true;
		}

		public void SetElement(VisualElement element)
		{
			if (Element != null)
				throw new NotSupportedException("Reuse of the Shell Renderer is not supported");
			Element = (Shell)element;
			Element.SizeChanged += OnElementSizeChanged;
			OnElementSet(Element);

			Element.PropertyChanged += OnElementPropertyChanged;
			_elementChanged?.Invoke(this, new VisualElementChangedEventArgs(null, Element));
		}

		#endregion IVisualElementRenderer

		protected internal Shell Element { get; private set; }

		void OnElementSizeChanged(object sender, EventArgs e)
		{
			//int width = (int)AndroidContext.ToPixels(Element.Width);
			//int height = (int)AndroidContext.ToPixels(Element.Height);
			//_flyoutRenderer.AndroidView.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
			//	MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.Exactly));
			//_flyoutRenderer.AndroidView.Layout(0, 0, width, height);
		}
		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Shell.CurrentItemProperty.PropertyName)
			{
				SwitchShellItem(_frameLayout, Element.CurrentItem);
			}
		}

		UGrid _navigationBar;
		UGrid _tabBar;
		TextBlock _title;
		protected virtual void OnElementSet(Shell shell)
		{
			_flyoutRenderer = CreateShellFlyoutRenderer();
			_frameLayout = new UFrame();
			//Content area
			var content = new UGrid();
			content.RowDefinitions.Add(new URowDefinition() { Height = new Windows.UI.Xaml.GridLength() });
			content.RowDefinitions.Add(new URowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
			content.RowDefinitions.Add(new URowDefinition() { Height = new Windows.UI.Xaml.GridLength() });
			content.Children.Add(_frameLayout);
			UGrid.SetRow(_frameLayout, 1);
			//Navigation bar
			_navigationBar = new UGrid() { Background = new SolidColorBrush(Colors.Red) };
			_navigationBar.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
			_navigationBar.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
			_navigationBar.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
			var burgerButton = new Windows.UI.Xaml.Controls.Button()
			{
				Content = new TextBlock()
				{
					FontFamily = new FontFamily("Segoe MDL2 Assets"),
					FontSize = 20,
					Text = "",
					VerticalAlignment = VerticalAlignment.Center,
					Foreground = new SolidColorBrush() { Color = Windows.UI.Colors.White }
				},
				HorizontalContentAlignment = HorizontalAlignment.Center,
				VerticalContentAlignment = VerticalAlignment.Center,
				BorderThickness = new Windows.UI.Xaml.Thickness(0),
				Background = null,
				Width = 48,
				Height = 48,
				Margin = new Windows.UI.Xaml.Thickness(5, 5, 20, 5),
			};
			burgerButton.Click += (s, e) => Element.FlyoutIsPresented = !Element.FlyoutIsPresented;
			_navigationBar.Children.Add(burgerButton);
			_title = new TextBlock()
			{
				Text = "Browse",
				VerticalAlignment = VerticalAlignment.Center,
				Foreground = new SolidColorBrush() { Color = Windows.UI.Colors.White }
			};
			UGrid.SetColumn(_title, 1);
			_navigationBar.Children.Add(_title);
			content.Children.Add(_navigationBar);
			_tabBar = new UGrid()
			{
				Height = 40,
				Visibility = Visibility.Collapsed
			};
			UGrid.SetRow(_tabBar, 2);
			content.Children.Add(_tabBar);

			this.Content = content;

			_flyoutRenderer.AttachFlyout(this, _frameLayout);

			((IShellController)shell).AddAppearanceObserver(this, shell);

			SwitchShellItem(_frameLayout, shell.CurrentItem, false);
		}

		ShellItemRenderer _currentRenderer;

		protected virtual void SwitchShellItem(UFrame targetView, ShellItem newItem, bool animate = true)
		{
			var previousRenderer = _currentRenderer;
			_currentRenderer = CreateShellItemRenderer(newItem);
			_currentRenderer.ShellItem = newItem;
			//TODO: Do all this in the renderer
			_title.Text = newItem.CurrentItem?.Title;
			var page = ((IShellContentController)newItem?.CurrentItem?.CurrentItem).GetOrCreateContent();
			targetView.Content = page?.ToFrameworkElement();
			if(newItem.Items.Count > 1)
			{
				_tabBar.ColumnDefinitions.Clear();
				_tabBar.Children.Clear();
				_tabBar.Visibility = Visibility.Visible;
				for (int i = 0; i < newItem.Items.Count; i++)
				{
					var item = newItem.Items[i];
					_tabBar.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
					var tb = new TextBlock()
					{
						Text = item.Title,
						Foreground = new SolidColorBrush(Windows.UI.Colors.White),
						Margin = new Windows.UI.Xaml.Thickness(0, 7, 0, 5),
						HorizontalAlignment = HorizontalAlignment.Center
					};
					UGrid.SetColumn(tb, i);
					_tabBar.Children.Add(tb);
				}
			}
			else
			{
				_tabBar.Visibility = Visibility.Collapsed;
			}
			//TODO: If animate: Transition to new item
		}

		protected virtual ShellFlyoutRenderer CreateShellFlyoutRenderer()
		{
			return new ShellFlyoutRenderer(this);
		}

		protected virtual ShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
		{
			return new ShellItemRenderer(this);
		}

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			_tabBar.Background = _navigationBar.Background = new SolidColorBrush(appearance.TabBarBackgroundColor.ToWindowsColor());
			var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
			titleBar.BackgroundColor = titleBar.ButtonBackgroundColor = appearance.BackgroundColor.ToWindowsColor();
			titleBar.ForegroundColor = titleBar.ButtonForegroundColor = appearance.TitleColor.ToWindowsColor();
		}

		#endregion IAppearanceObserver
	}
}
