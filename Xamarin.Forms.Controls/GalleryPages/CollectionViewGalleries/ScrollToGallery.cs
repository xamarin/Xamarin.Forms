namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
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
						GalleryBuilder.NavButton("ScrollTo (Code, Horizontal List)", () =>
							new ScrollToCodeGallery(ListItemsLayout.HorizontalList), Navigation),
						GalleryBuilder.NavButton("ScrollTo (Code, Verticl List)", () =>
							new ScrollToCodeGallery(ListItemsLayout.VerticalList), Navigation),
						GalleryBuilder.NavButton("ScrollTo (Code, Horizontal Grid)", () =>
							new ScrollToCodeGallery(new GridItemsLayout(3, ItemsLayoutOrientation.Horizontal)), Navigation),
						GalleryBuilder.NavButton("ScrollTo (Code, Vertical Grid)", () =>
							new ScrollToCodeGallery(new GridItemsLayout(3, ItemsLayoutOrientation.Vertical)), Navigation),
					}
				}
			};
		}
	}
}