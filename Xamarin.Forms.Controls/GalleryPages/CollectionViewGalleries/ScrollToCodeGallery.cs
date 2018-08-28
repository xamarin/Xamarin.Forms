namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class ScrollToCodeGallery : ContentPage
	{
		public ScrollToCodeGallery(IItemsLayout itemsLayout)
		{
			Title = $"ScrollTo (Code, {itemsLayout})";

			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star }
				}
			};

			var itemTemplate = ExampleTemplates.SnapPointsTemplate();

			var collectionView = new CollectionView
			{
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate,
			};

			var generator = new ItemsSourceGenerator(collectionView, initialItems: 50);

			var scrollToControl = new ScrollToControl(collectionView);

			layout.Children.Add(generator);
			layout.Children.Add(scrollToControl);
			
			layout.Children.Add(collectionView);

			Grid.SetRow(scrollToControl, 1);
			Grid.SetRow(collectionView, 2);

			Content = layout;

			generator.GenerateItems();
		}
	}
}