using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Xamarin.Forms.Platform.UWP
{
	public class PageRenderer : VisualElementRenderer<Page, FrameworkElement>
	{
		bool _disposed;

		bool _loaded;
		UISettings _uiSettings = new UISettings();

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

			_uiSettings.ColorValuesChanged -= _uiSettings_ColorValuesChanged;
			_uiSettings = null;

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
					_uiSettings.ColorValuesChanged += _uiSettings_ColorValuesChanged;	
					Tracker = new BackgroundTracker<FrameworkElement>(BackgroundProperty);
				}

				if (!string.IsNullOrEmpty(Element.AutomationId))
				{
					SetAutomationId(Element.AutomationId);
				}

				if (_loaded)
					e.NewElement.SendAppearing();
			}
		}

		void _uiSettings_ColorValuesChanged(UISettings sender, object args)
		{
			Device.BeginInvokeOnMainThread(() => Element.Resources.Reload());
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
	}
}