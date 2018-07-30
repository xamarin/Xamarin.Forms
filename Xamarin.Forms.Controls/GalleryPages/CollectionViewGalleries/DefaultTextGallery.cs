namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class DefaultTextGallery : ContentPage
	{
		public DefaultTextGallery()
		{
			var desc = "No DataTemplates; just using the ToString() of the objects in the source.";

			var descriptionLabel = new Label { Text = desc, Margin = new Thickness(2,2,2,2)};

			Title = "Default Text Galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						// TODO hartez 2018-06-05 10:43 AM Need a gallery page which allows layout selection
						// so we can demonstrate switching between them
						descriptionLabel,
						GalleryBuilder.NavButton("Vertical List (Code)", () => 
							new TextCodeCollectionViewGallery(ListItemsLayout.VerticalList), Navigation),
						GalleryBuilder.NavButton("Horizontal List (Code)", () => 
							new TextCodeCollectionViewGallery(ListItemsLayout.HorizontalList), Navigation),
						GalleryBuilder.NavButton("Vertical Grid (Code)", () => 
							new TextCodeCollectionViewGridGallery(), Navigation),
						GalleryBuilder.NavButton("Horizontal Grid (Code)", () => 
							new TextCodeCollectionViewGridGallery(ItemsLayoutOrientation.Horizontal), Navigation),
					}
				}
			};
		}
	}

	internal class DataTemplateGallery : ContentPage
	{
		public DataTemplateGallery()
		{
			var desc = "Simple DataTemplate Galleries";

			var descriptionLabel = new Label { Text = desc, Margin = new Thickness(2,2,2,2)};

			Title = "Simple DataTemplate Galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryBuilder.NavButton("Vertical List (Code)", () => 
							new TemplateCodeCollectionViewGallery(ListItemsLayout.VerticalList), Navigation),
						GalleryBuilder.NavButton("Horizontal List (Code)", () => 
							new TemplateCodeCollectionViewGallery(ListItemsLayout.HorizontalList), Navigation),
						GalleryBuilder.NavButton("Vertical Grid (Code)", () => 
							new TemplateCodeCollectionViewGridGallery (), Navigation),
						GalleryBuilder.NavButton("Horizontal Grid (Code)", () => 
							new TemplateCodeCollectionViewGridGallery (ItemsLayoutOrientation.Horizontal), Navigation),
					}
				}
			};
		}
	}

	internal class ObservableCollectionGallery : ContentPage
	{
		public ObservableCollectionGallery ()
		{
			var desc = "Observable Collection Galleries";

			var descriptionLabel = new Label { Text = desc, Margin = new Thickness(2,2,2,2)};

			Title = "Simple DataTemplate Galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryBuilder.NavButton("Add/Remove Items", () => 
							new ObservableCodeCollectionViewGridGallery (), Navigation),
					}
				}
			};
		}
	}
}