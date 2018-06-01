using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	public class CollectionViewGallery : ContentPage
	{
		public CollectionViewGallery()
		{
			Content = new StackLayout
			{
				Children =
				{
					GalleryBuilder.NavButton("Default Text Galleries", () => new DefaultTextGallery(), Navigation),
				}
			};
		}
	}

	public class DefaultTextGallery : ContentPage
	{
		public DefaultTextGallery()
		{
			var desc = "No DataTemplates; just using the ToString() of the objects in the source.";

			var descriptionLabel = new Label { Text = desc, Margin = new Thickness(2,2,2,2)};

			Title = "Disabled states galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryBuilder.NavButton("Vertical List (Code)", () => new TextCodeVerticalListGallery(), Navigation),
						GalleryBuilder.NavButton("Horizontal List (Code)", () => new TextCodeHorizontalListGallery(), Navigation),
					}
				}
			};
		}
	}

	public class TextCodeVerticalListGallery : ContentPage
	{
		public TextCodeVerticalListGallery()
		{
			var items = new List<string>();

			for (int n = 0; n < 1000; n++)
			{
				items.Add(DateTime.Now.AddDays(n).ToLongDateString());
			}

			var collectionView = new CollectionView { ItemsSource = items };

			// This the default
			//collectionView.ItemsLayout = ListItemsLayout.VerticalList; 

			Content = collectionView;
		}
	}

	public class TextCodeHorizontalListGallery : ContentPage
	{
		public TextCodeHorizontalListGallery()
		{
			var items = new List<string>();

			for (int n = 0; n < 1000; n++)
			{
				items.Add(DateTime.Now.AddDays(n).ToLongDateString());
			}

			var collectionView = new CollectionView
			{
				ItemsSource = items,
				ItemsLayout = ListItemsLayout.HorizontalList
			};

			Content = collectionView;
		}
	}
}
