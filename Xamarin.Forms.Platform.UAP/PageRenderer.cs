using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Xamarin.Forms.Platform.UWP
{
	public class PageRenderer : VisualElementRenderer<Page, FrameworkElement>
	{
		bool _disposed;

		bool _loaded;

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			// Pages need an automation peer so we can interact with them in automated tests
			return new FrameworkElementAutomationPeer(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing || _disposed)
				return;

			_disposed = true;

			if (Element != null)
			{
				ReadOnlyCollection<Element> children = ((IElementController)Element).LogicalChildren;
				for (var i = 0; i < children.Count; i++)
				{
					var visualChild = children[i] as VisualElement;
					visualChild?.Cleanup();
				}
				Element?.SendDisappearing();
			}

			base.Dispose();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			e.OldElement?.SendDisappearing();

			if (e.NewElement != null)
			{
				if (e.OldElement == null)
				{
					Loaded += OnLoaded;
					Tracker = new BackgroundTracker<FrameworkElement>(BackgroundProperty);
				}

				if (!string.IsNullOrEmpty(Element.AutomationId))
				{
					SetAutomationId(Element.AutomationId);
				}

				UpdateStatusBarColor();
				UpdateStatusBarStyle();

				if (_loaded)
					e.NewElement.SendAppearing();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == Page.StatusBarColorProperty.PropertyName)
				UpdateStatusBarColor();
			else if (e.PropertyName == Page.StatusBarStyleProperty.PropertyName)
				UpdateStatusBarStyle();
		}

		void OnLoaded(object sender, RoutedEventArgs args)
		{
			var carouselPage = Element?.Parent as CarouselPage;
			if (carouselPage != null && carouselPage.Children[0] != Element)
			{
				return;
			}
			_loaded = true;
			Unloaded += OnUnloaded;
			Element?.SendAppearing();
		}

		void OnUnloaded(object sender, RoutedEventArgs args)
		{
			Unloaded -= OnUnloaded;
			_loaded = false;
			Element?.SendDisappearing();
		}

		void UpdateStatusBarColor()
		{
			if (!Element.IsAppeared)
				return;

			if (Element.StatusBarColor == Color.Default)
				return;

			if (ApiInformation.IsTypePresent(typeof(StatusBar).FullName ?? string.Empty))
			{
				var statusBar = StatusBar.GetForCurrentView();
				if (statusBar != null)
				{
					statusBar.BackgroundColor = Element.StatusBarColor.ToWindowsColor();
				}
			}
			else
			{
				var titleBar = ApplicationView.GetForCurrentView()?.TitleBar;
				if (titleBar != null)
				{
					titleBar.BackgroundColor = Element.StatusBarColor.ToWindowsColor();
				}
			}
		}

		void UpdateStatusBarStyle()
		{
			if (!Element.IsAppeared)
				return;

			Color foregroundColor = Color.Default;
			switch (Element.StatusBarStyle)
			{
				case StatusBarStyle.LightContent:
					foregroundColor = Color.Black;
					break;
				case StatusBarStyle.DarkContent:
					foregroundColor = Color.White;
					break;
			}

			if (ApiInformation.IsTypePresent(typeof(StatusBar).FullName ?? string.Empty))
			{
				var statusBar = StatusBar.GetForCurrentView();
				if (statusBar != null)
				{
					statusBar.ForegroundColor = foregroundColor.ToWindowsColor();
				}
			}
			else
			{
				var titleBar = ApplicationView.GetForCurrentView()?.TitleBar;
				if (titleBar != null)
				{
					titleBar.ForegroundColor = foregroundColor.ToWindowsColor();
				}
			}
		}
	}
}