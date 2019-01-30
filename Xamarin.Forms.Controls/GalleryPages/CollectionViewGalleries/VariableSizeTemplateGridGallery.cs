namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class VariableSizeTemplateGridGallery : ContentPage
	{
		public VariableSizeTemplateGridGallery(ItemsLayoutOrientation orientation = ItemsLayoutOrientation.Vertical)
		{
			var layout = new Grid
			{ 
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star }
				}
			};

			var itemsLayout = new GridItemsLayout(2, orientation);

			var itemTemplate = ExampleTemplates.VariableSizeTemplate();

			var collectionView = new CollectionView {ItemsLayout = itemsLayout, ItemTemplate = itemTemplate,
				ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem };

			var generator = new ItemsSourceGenerator(collectionView, 100);
			var sizingStrategySelector = new EnumSelector<ItemSizingStrategy>(() => collectionView.ItemSizingStrategy,
				mode => {
					// Setting the template again so the "first" item is small, making the transition between 
					// "MeasureFirstItem" and "MeasureAllItems" obvious
					collectionView.ItemTemplate = ExampleTemplates.VariableSizeTemplate();
					collectionView.ItemSizingStrategy = mode;
				});

			layout.Children.Add(generator);
			layout.Children.Add(sizingStrategySelector );
			Grid.SetRow(sizingStrategySelector , 1);
			layout.Children.Add(collectionView);
			Grid.SetRow(collectionView, 2);

			Content = layout;

			generator.GenerateItems();
		}
	}
}