using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Controls.GalleryPages.VisualStateManagerGalleries
{
	public class CollectionViewGallery : ContentPage
	{
		static Button GalleryNav(string galleryName, Func<ContentPage> gallery, INavigation nav)
		{
			var button = new Button { Text = $"{galleryName}" };
			button.Clicked += (sender, args) => { nav.PushAsync(gallery()); };
			return button;
		}

		public CollectionViewGallery()
		{
			Content = new StackLayout
			{
				Children =
				{
					GalleryNav("Default Text Galleries", () => new DefaultTextGallery(), Navigation),
				}
			};
		}
	}

	public class DefaultTextGallery : ContentPage
	{
		static Button GalleryNav(string control, Func<ContentPage> gallery, INavigation nav)
		{
			var button = new Button { Text = $"{control} Disabled States" };
			button.Clicked += (sender, args) => { nav.PushAsync(gallery()); };
			return button;
		}

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
						GalleryNav("Vertical List (Code)", () => new TextCodeVerticalListGallery(), Navigation),
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

			var collectionView = new CollectionView();

			collectionView.ItemsSource = items;
			
			// This the default
			//collectionView.ItemsLayout = ListItemsLayout.VerticalList; 

			Content = collectionView;
		}
	}
}
