namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.SpacingGalleries
{
	internal class SpacingGallery : ContentPage
	{
		ItemsSourceGenerator generator;

		public SpacingGallery(IItemsLayout itemsLayout)
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
				Text = "Use the control below to update the spacing between items."
			};

			if (itemsLayout is GridItemsLayout)
			{
				instructions.Text += " Format is '[vertical], [horizontal]'";
			}

			var itemTemplate = ExampleTemplates.SpacingTemplate();

			var collectionView = new CollectionView
			{
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate,
				AutomationId = "collectionview",
				Margin = 10
			};

			generator = new ItemsSourceGenerator(collectionView, initialItems: 20);
			var spacingModifier = new SpacingModifier(collectionView.ItemsLayout, "Update_Spacing");

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

		protected override void OnAppearing()
		{
			base.OnAppearing();

			generator.SubscribeEvents();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			generator.UnsubscribeEvents();
		}
	}
}
