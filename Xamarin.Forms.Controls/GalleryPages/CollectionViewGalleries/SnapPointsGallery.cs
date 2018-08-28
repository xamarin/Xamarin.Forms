using System;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class SnapPointsGallery : ContentPage
	{
		public SnapPointsGallery()
		{
			var descriptionLabel =
				new Label { Text = "Snap Points Galleries", Margin = new Thickness(2, 2, 2, 2) };

			Title = "Snap Points Galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryBuilder.NavButton("Snap Points (Code, Horizontal List)", () =>
							new SnapPointsCodeGallery(ItemsLayoutOrientation.Horizontal), Navigation),
						GalleryBuilder.NavButton("Snap Points (Code, Vertical List)", () =>
							new SnapPointsCodeGallery(ItemsLayoutOrientation.Vertical), Navigation)
					}
				}
			};
		}
	}

	internal class ScrollToGallery : ContentPage
	{
		public ScrollToGallery()
		{
			var descriptionLabel =
				new Label { Text = "ScrollTo Galleries", Margin = new Thickness(2, 2, 2, 2) };

			Title = "ScrollTo Galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryBuilder.NavButton("Snap Points (Code, Horizontal List)", () =>
							new ScrollToCodeGallery(ListItemsLayout.HorizontalList), Navigation),
					}
				}
			};
		}
	}
}