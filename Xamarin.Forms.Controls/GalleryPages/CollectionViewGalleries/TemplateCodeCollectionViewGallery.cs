namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class TemplateCodeCollectionViewGallery : ContentPage
	{
		public TemplateCodeCollectionViewGallery(IItemsLayout itemsLayout)
		{
			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star }
				}
			};

			// TODO hartez 2018/07/25 14:38:05 In order to reasonably test adding/removing single items, we need these items to be differentiatable	
			// So spend a bit making the image source bind to the data, set up the data so we can display a few different images, and add the dates as captions (and add index as a visible label so it's easy to see the changes below)
			// Then add some controls for adding/removing items at an index so we can test that with the adapter, collectionViewsource, etc.

			var itemTemplate = ExampleTemplates.PhotoTemplate();

			var collectionView = new CollectionView
			{
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate
			};

			var generator = new ItemsSourceGenerator(collectionView, initialItems: 2);

			layout.Children.Add(generator);
			layout.Children.Add(collectionView);

			Grid.SetRow(collectionView, 1);

			Content = layout;

			generator.GenerateItems();
		}
	}
}