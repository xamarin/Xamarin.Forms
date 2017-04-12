﻿using System.Collections.ObjectModel;
using Windows.UI.Xaml;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public class PageRenderer : VisualElementRenderer<Page, FrameworkElement>
	{
		bool _disposed;

		bool _loaded;

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
                    // In the previous version not remove it before, This might be cause a add multiple even problem and cause some performance and memory or other problems wehen keep navigate exist page.
                    Loaded -= OnLoaded;
                    Unloaded -= OnUnloaded;
                    Loaded += OnLoaded;
					Unloaded += OnUnloaded;

					Tracker = new BackgroundTracker<FrameworkElement>(BackgroundProperty);
				}

				if (_loaded)
					e.NewElement.SendAppearing();
			}
		}

		void OnLoaded(object sender, RoutedEventArgs args)
		{
			var carouselPage = Element?.Parent as CarouselPage;
			if (carouselPage != null && carouselPage.Children[0] != Element)
			{
				return;
			}
			_loaded = true;
			Element?.SendAppearing();
		}

		void OnUnloaded(object sender, RoutedEventArgs args)
		{
			_loaded = false;
			Element?.SendDisappearing();
		}
	}
}