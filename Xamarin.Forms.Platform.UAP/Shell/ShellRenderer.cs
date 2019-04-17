using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellRenderer : NavigationView, IVisualElementRenderer, /*IShellContext, */IAppearanceObserver, IFlyoutBehaviorObserver
	{
		internal static readonly Color DefaultBackgroundColor = Color.FromRgb(33, 150, 243);
		internal static readonly Color DefaultForegroundColor = Color.White;
		internal static readonly Color DefaultTitleColor = Color.White;
		internal static readonly Color DefaultUnselectedColor = Color.FromRgba(255, 255, 255, 180);

		Windows.UI.Xaml.Controls.Frame _Frame;
		Windows.UI.Xaml.Controls.Panel _PaneHeaderArea;

		public ShellRenderer()
		{
			ItemInvoked += OnMenuItemInvoked;
			Content = _Frame = new Windows.UI.Xaml.Controls.Frame();
			BackRequested += OnBackRequested;
			IsBackEnabled = true;
			IsSettingsVisible = false;
			MenuItemTemplateSelector = new ShellPaneTemplateSelector();
			PaneClosing += (s, e) => { if (_PaneHeaderArea != null) _PaneHeaderArea.Visibility = Visibility.Collapsed; };
			PaneOpening += (s, e) => { if (_PaneHeaderArea != null) _PaneHeaderArea.Visibility = Visibility.Visible; };
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var topnavPad = GetTemplateChild("TopNavTopPadding") as UIElement;
			if (topnavPad != null)
				topnavPad.Visibility = Visibility.Collapsed;
			var toggleTopnavPad = GetTemplateChild("TogglePaneTopPadding") as UIElement;
			if (toggleTopnavPad != null)
				toggleTopnavPad.Visibility = Visibility.Collapsed;
			//Replace title area with Header area
			var paneTitle = GetTemplateChild("PaneTitleTextBlock") as FrameworkElement;
			if (paneTitle != null)
			{
				paneTitle.Loaded += (s, e) =>
				{
					if (paneTitle.Parent is Panel panel)
					{
						panel.Children.Clear();
						_PaneHeaderArea = panel;
						UpdatePaneHeader();
					}
				};
			}
		}
		void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			if (_Frame.Content is ShellItemRenderer r && r.CanGoBack)
			{
				r.GoBack();
			}
			else if (_Frame.CanGoBack)
			{
				_Frame.GoBack();
			}
			IsBackEnabled = _Frame.CanGoBack || _Frame.Content is ShellItemRenderer ren && ren.CanGoBack;
		}

		void OnMenuItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			var item = args.InvokedItemContainer?.DataContext as Element;
			if (item != null)
				((IShellController)Element).OnFlyoutItemSelected(item);
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

		internal Shell Shell => Element;

		void OnElementSizeChanged(object sender, EventArgs e)
		{
			InvalidateMeasure();
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Shell.CurrentItemProperty.PropertyName)
			{
				SwitchShellItem(Element.CurrentItem);
			}
		}

		protected virtual void OnElementSet(Shell shell)
		{
			((IShellController)Element).AddFlyoutBehaviorObserver(this);
			UpdatePaneHeader();
			MenuItemsSource = IterateItems();
			SwitchShellItem(shell.CurrentItem, false);
			((IShellController)shell).AddAppearanceObserver(this, shell);
		}

		IEnumerable<object> IterateItems()
		{
			foreach (var item in Element.Items)
				yield return item;
			if (Element.Items.Count > 0 && Element.MenuItems.Count > 0)
				yield return null; //Use null for a seperator
			foreach (var item in Element.MenuItems)
				yield return item;
		}

		void UpdatePaneHeader()
		{
			if (_PaneHeaderArea != null)
			{
				_PaneHeaderArea.Visibility = IsPaneOpen ? Visibility.Visible : Visibility.Collapsed;
				_PaneHeaderArea.ClearValue(FrameworkElement.HeightProperty);
				_PaneHeaderArea.Children.Clear();
				if (Element != null)
				{
					Windows.UI.Xaml.FrameworkElement header = null;
					_PaneHeaderArea.Children.Add(new ShellHeaderRenderer(Element));
					if (header != null)
						_PaneHeaderArea.Children.Add(header);
				}
			}
		}

		void SwitchShellItem(ShellItem newItem, bool animate = true)
		{
			if (_Frame.Content != null)
				IsBackEnabled = true;
			SelectedItem = newItem;
			_Frame.Navigate(typeof(ShellItemRenderer), ShellItemRenderer.CreateNavigationArgs(this, newItem), new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
		}

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			Windows.UI.Color backgroundColor;
			if (appearance != null && !appearance.BackgroundColor.IsDefault)
				backgroundColor = appearance.BackgroundColor.ToWindowsColor(); // #03A9F4
			else
				backgroundColor = Windows.UI.Color.FromArgb(255, 3, 169, 244); // #03A9F4
			var brush = new SolidColorBrush(backgroundColor);

			var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
			titleBar.BackgroundColor = titleBar.ButtonBackgroundColor = backgroundColor;
			titleBar.ForegroundColor = titleBar.ButtonForegroundColor = appearance.TitleColor.ToWindowsColor();

			if (_Frame.Content is IAppearanceObserver iao)
				iao.OnAppearanceChanged(appearance);
		}

		#endregion IAppearanceObserver

		void IFlyoutBehaviorObserver.OnFlyoutBehaviorChanged(FlyoutBehavior behavior)
		{			
			switch (behavior)
			{
				case FlyoutBehavior.Disabled:
					PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
					IsPaneOpen = false;
					break;

				case FlyoutBehavior.Flyout:
					PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
					IsPaneOpen = Shell.FlyoutIsPresented;
					break;

				case FlyoutBehavior.Locked:
					IsPaneOpen = true;
					PaneDisplayMode = NavigationViewPaneDisplayMode.Left;
					break;
			}
		}
	}
}
