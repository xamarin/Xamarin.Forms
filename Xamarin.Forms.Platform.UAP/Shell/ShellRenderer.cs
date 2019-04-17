using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellRenderer : NavigationView, IVisualElementRenderer, IAppearanceObserver, IFlyoutBehaviorObserver
	{
		internal static readonly Windows.UI.Color DefaultBackgroundColor = Windows.UI.Color.FromArgb(255, 33, 150, 243);
		internal static readonly Windows.UI.Color DefaultForegroundColor = Windows.UI.Colors.White;
		internal static readonly Windows.UI.Color DefaultTitleColor = Windows.UI.Colors.White;
		internal static readonly Windows.UI.Color DefaultUnselectedColor = Windows.UI.Color.FromArgb(180, 255, 255, 255);

		ShellItemRenderer ItemRenderer { get; }

		public ShellRenderer()
		{
			IsBackEnabled = false;
			IsSettingsVisible = false;
			Content = ItemRenderer = new ShellItemRenderer();
			MenuItemTemplateSelector = new ShellPaneTemplateSelector();
			DisplayModeChanged += OnDisplayModeChanged;
			PaneClosing += (s, e) => OnPaneClosed();
			PaneOpening += (s, e) => OnPaneOpening();
			ItemInvoked += OnMenuItemInvoked;
		}

		void OnDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
		{
			bool isClosed = (args.DisplayMode == NavigationViewDisplayMode.Minimal);
			var elm = GetTemplateChild("TogglePaneTopPadding") as UIElement;
			if(elm != null)
				elm.Visibility = isClosed ? Visibility.Collapsed : Visibility.Visible;
			UpdatePaneButtonColor("TogglePaneButton", isClosed);
			UpdatePaneButtonColor("NavigationViewBackButton", isClosed);			
		}

		private void UpdatePaneButtonColor(string name, bool overrideColor)
		{
			var toggleButton = GetTemplateChild(name) as Control;
			if (toggleButton != null)
			{
				var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
				if (overrideColor)
					toggleButton.Foreground = new SolidColorBrush(titleBar.ButtonForegroundColor.Value);
				else
					toggleButton.ClearValue(Control.ForegroundProperty);
			}
		}

		void OnPaneOpening()
		{
			if (PaneCustomContent is FrameworkElement fe)
				fe.Visibility = Visibility.Visible;
			Shell.FlyoutIsPresented = true;
		}

		void OnPaneClosed()
		{
			if (PaneCustomContent is FrameworkElement fe)
				fe.Visibility = Visibility.Collapsed;
			Shell.FlyoutIsPresented = false;
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
			ItemRenderer.SetShellContext(this);
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
			else if (e.PropertyName == Shell.FlyoutIsPresentedProperty.PropertyName)
			{
				IsPaneOpen = Shell.FlyoutIsPresented;
			}
		}

		protected virtual void OnElementSet(Shell shell)
		{
			((IShellController)Element).AddFlyoutBehaviorObserver(this);
			var shr = new ShellHeaderRenderer(shell);
			PaneCustomContent = shr;
			
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

		void SwitchShellItem(ShellItem newItem, bool animate = true)
		{
			SelectedItem = newItem;
			ItemRenderer.NavigateToShellItem(newItem, animate);
		}

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			Windows.UI.Color backgroundColor = Windows.UI.Color.FromArgb(255, 3, 169, 244); // #03A9F4
			Windows.UI.Color foregroundColor = Windows.UI.Colors.Black;
			if (appearance != null)
			{
				if (!appearance.BackgroundColor.IsDefault)
					backgroundColor = appearance.BackgroundColor.ToWindowsColor();
				if (!appearance.TitleColor.IsDefault)
					foregroundColor = appearance.TitleColor.ToWindowsColor();
			}

			var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
			titleBar.BackgroundColor = titleBar.ButtonBackgroundColor = backgroundColor;
			titleBar.ForegroundColor = titleBar.ButtonForegroundColor = foregroundColor;
			if (DisplayMode == NavigationViewDisplayMode.Minimal)
			{
				UpdatePaneButtonColor("TogglePaneButton", true);
				UpdatePaneButtonColor("NavigationViewBackButton", true);
			}
		}

		#endregion IAppearanceObserver

		void IFlyoutBehaviorObserver.OnFlyoutBehaviorChanged(FlyoutBehavior behavior)
		{			
			switch (behavior)
			{
				case FlyoutBehavior.Disabled:
					IsPaneToggleButtonVisible = false;
					IsPaneOpen = false;
					break;

				case FlyoutBehavior.Flyout:
					IsPaneToggleButtonVisible = true;
					break;

				case FlyoutBehavior.Locked:
					IsPaneToggleButtonVisible = true;
					break;
			}
		}
	}
}
