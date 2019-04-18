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
		internal static readonly Windows.UI.Color DefaultBackgroundColor = Windows.UI.Color.FromArgb(255, 3, 169, 244);
		internal static readonly Windows.UI.Color DefaultForegroundColor = Windows.UI.Colors.White;
		internal static readonly Windows.UI.Color DefaultTitleColor = Windows.UI.Colors.White;
		internal static readonly Windows.UI.Color DefaultUnselectedColor = Windows.UI.Color.FromArgb(180, 255, 255, 255);

		ShellItemRenderer ItemRenderer { get; }

		public ShellRenderer()
		{
			if (!Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.NavigationView", "PaneDisplayMode"))
				throw new PlatformNotSupportedException("Windows 10 October 2018 (1809) update required");
			IsBackEnabled = false;
			IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
			IsSettingsVisible = false;
			PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
			IsPaneOpen = false;
			Content = ItemRenderer = new ShellItemRenderer();
			MenuItemTemplateSelector = new ShellFlyoutTemplateSelector();
			PaneClosing += (s, e) => OnPaneClosed();
			PaneOpening += (s, e) => OnPaneOpening();
			ItemInvoked += OnMenuItemInvoked;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			UpdatePaneButtonColor("TogglePaneButton", !IsPaneOpen);
			UpdatePaneButtonColor("NavigationViewBackButton", !IsPaneOpen);
		}

		void OnPaneOpening()
		{
			if (Shell != null)
				Shell.FlyoutIsPresented = true;
			UpdatePaneButtonColor("TogglePaneButton", false);
			UpdatePaneButtonColor("NavigationViewBackButton", false);
		}

		void OnPaneClosed()
		{
			if(Shell != null)
				Shell.FlyoutIsPresented = false;
			UpdatePaneButtonColor("TogglePaneButton", true);
			UpdatePaneButtonColor("NavigationViewBackButton", true);
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
			var shr = new ShellHeaderRenderer(shell);
			PaneCustomContent = shr;
			MenuItemsSource = IterateItems();
			SwitchShellItem(shell.CurrentItem, false);
			IsPaneOpen = Shell.FlyoutIsPresented;
			((IShellController)Element).AddFlyoutBehaviorObserver(this);
			((IShellController)shell).AddAppearanceObserver(this, shell);
		}

		IEnumerable<object> IterateItems()
		{
			var groups = ((IShellController)Shell).GenerateFlyoutGrouping();
			foreach(var group in groups)
			{
				if (group.Count > 0 && group != groups[0])
				{
					yield return null; // Creates a seperator
				}
				foreach(var item in group)
				{
					yield return item;
				}
			}
		}

		void SwitchShellItem(ShellItem newItem, bool animate = true)
		{
			SelectedItem = newItem;
			ItemRenderer.NavigateToShellItem(newItem, animate);
		}

		void UpdatePaneButtonColor(string name, bool overrideColor)
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

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			Windows.UI.Color backgroundColor = DefaultBackgroundColor;
			Windows.UI.Color titleColor = DefaultTitleColor;
			if (appearance != null)
			{
				if (!appearance.BackgroundColor.IsDefault)
					backgroundColor = appearance.BackgroundColor.ToWindowsColor();
				if (!appearance.TitleColor.IsDefault)
					titleColor = appearance.TitleColor.ToWindowsColor();
			}

			var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
			titleBar.BackgroundColor = titleBar.ButtonBackgroundColor = backgroundColor;
			titleBar.ForegroundColor = titleBar.ButtonForegroundColor = titleColor;
			UpdatePaneButtonColor("TogglePaneButton", !IsPaneOpen);
			UpdatePaneButtonColor("NavigationViewBackButton", !IsPaneOpen);
		}

		#endregion IAppearanceObserver

		void IFlyoutBehaviorObserver.OnFlyoutBehaviorChanged(FlyoutBehavior behavior)
		{			
			switch (behavior)
			{
				case FlyoutBehavior.Disabled:
					IsPaneToggleButtonVisible = false;
					IsPaneVisible = false;
					PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
					IsPaneOpen = false;
					break;

				case FlyoutBehavior.Flyout:
					IsPaneVisible = true;
					IsPaneToggleButtonVisible = true;
					bool shouldOpen = Shell.FlyoutIsPresented;
					PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal; //This will trigger opening the flyout
					IsPaneOpen = shouldOpen;
					break;

				case FlyoutBehavior.Locked:
					IsPaneVisible = true;
					IsPaneToggleButtonVisible = false;
					PaneDisplayMode = NavigationViewPaneDisplayMode.Left;
					break;
			}
		}
	}
}
