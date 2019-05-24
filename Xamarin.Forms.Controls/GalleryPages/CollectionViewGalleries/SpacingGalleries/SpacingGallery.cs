namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.SpacingGalleries
{
	internal class SpacingGallery : ContentPage
	{
		public SpacingGallery(ItemsLayoutOrientation orientation)
		{
			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star }
				}
			};

			var instructions = new Label
			{
				Text = "Tap the buttons in each item to increase/decrease the amount of text. The items should expand and contract to accommodate the text."
			};

			var itemTemplate = ExampleTemplates.SpacingTemplate();

			var collectionView = new CollectionView
			{
				ItemsLayout = new ListItemsLayout(orientation),
				ItemTemplate = itemTemplate,
				AutomationId = "collectionview",
				Margin = 10
			};

			var generator = new ItemsSourceGenerator(collectionView, initialItems: 20);
			var spacingModifier = new SpacingModifier(collectionView, "Update Spacing");

			layout.Children.Add(generator);
			layout.Children.Add(instructions);
			layout.Children.Add(spacingModifier);
			layout.Children.Add(collectionView);

			Grid.SetRow(instructions, 1);
			Grid.SetRow(spacingModifier, 2);
			Grid.SetRow(collectionView, 3);

			Content = layout;

			generator.GenerateItems();
		}
	}
}
