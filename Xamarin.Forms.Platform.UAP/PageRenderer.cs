using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Xamarin.Forms.Platform.UWP
{
	public class PageRenderer : VisualElementRenderer<Page, FrameworkElement>
	{
		bool _disposed;

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
					Tracker = new BackgroundTracker<FrameworkElement>(BackgroundProperty);
				}

				if (!string.IsNullOrEmpty(Element.AutomationId))
				{
					SetAutomationId(Element.AutomationId);
				}
			}
		}

		public override void OnLoading(FrameworkElement sender, object args)
		{
			base.OnLoading(sender, args);
			Element?.SendAppearing();
		}

		public override void OnLoaded(object sender, RoutedEventArgs args)
		{
			base.OnLoaded(sender, args);
			var carouselPage = Element?.Parent as CarouselPage;
			if (carouselPage != null && carouselPage.Children[0] != Element)
			{
				return;
			}
			Element?.SendAppeared();
		}

		public override void OnUnloaded(object sender, RoutedEventArgs args)
		{
			Element?.SendDisappeared();

			base.OnUnloaded(sender, args);
		}
	}
}