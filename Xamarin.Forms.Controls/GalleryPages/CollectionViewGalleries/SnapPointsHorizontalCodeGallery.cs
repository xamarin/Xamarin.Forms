namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class SnapPointsCodeGallery : ContentPage
	{
		public SnapPointsCodeGallery(ItemsLayoutOrientation orientation)
		{
			Title = $"Snap Points (Code, {orientation} List)";

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

			var itemsLayout =
				new ListItemsLayout(orientation)
				{
					SnapPointsType = SnapPointsType.None,
					SnapPointsAlignment = SnapPointsAlignment.Start
				};

			var itemTemplate = ExampleTemplates.SnapPointsTemplate();

			var collectionView = new CollectionView
			{
				Margin = new Thickness(50,50,50,50),
				BackgroundColor = Color.Aquamarine,
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate,
			};

			

			var generator = new ItemsSourceGenerator(collectionView, initialItems: 50);

			var snapPointsTypeSelector = new EnumSelector<SnapPointsType>(() => itemsLayout.SnapPointsType,
				type => itemsLayout.SnapPointsType = type);

			var snapPointsAlignmentSelector = new EnumSelector<SnapPointsAlignment>(() => itemsLayout.SnapPointsAlignment,
				type => itemsLayout.SnapPointsAlignment = type);

			layout.Children.Add(generator);
			layout.Children.Add(snapPointsTypeSelector);
			layout.Children.Add(snapPointsAlignmentSelector);
			layout.Children.Add(collectionView);

			Grid.SetRow(snapPointsTypeSelector, 1);
			Grid.SetRow(snapPointsAlignmentSelector, 2);
			Grid.SetRow(collectionView, 3);

			Content = layout;

			generator.GenerateItems();
		}
	}
}