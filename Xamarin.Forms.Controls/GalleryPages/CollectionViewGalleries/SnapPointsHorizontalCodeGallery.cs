namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	// TODO hartez 2018/08/16 12:55:19 This needs a flow direction selector	
	// The CollectionViewAdapter needs to generate views (text or from Templates) with the correct flowdirection
	// The CollectionViewAdapter needs to respond to flow direction changes (for now, just reset it completely)
	// The StartSnapHelper is handling RTL incorrectly (rewinds all the way back)
	// Verify that the layouts handle flow direction changes reasonably
	// Still need an EndSnapHelper (or to make Start generic enough that we can have an EdgeSnapHelper)

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
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate,
			};

			var generator = new ItemsSourceGenerator(collectionView, initialItems: 50);

			var snapPointsTypeSelector = new EnumSelector<SnapPointsType>(() => itemsLayout.SnapPointsType,
				type => itemsLayout.SnapPointsType = type);

			var snapPointsAlignmentSelector = new EnumSelector<SnapPointsAlignment>(() => itemsLayout.SnapPointsAlignment,
				type => itemsLayout.SnapPointsAlignment = type);

			var flowDirectionSelector = new EnumSelector<FlowDirection>(() => layout.FlowDirection,
				type => layout.FlowDirection = type);

			layout.Children.Add(generator);
			layout.Children.Add(snapPointsTypeSelector);
			layout.Children.Add(snapPointsAlignmentSelector);
			layout.Children.Add(flowDirectionSelector);
			layout.Children.Add(collectionView);

			Grid.SetRow(snapPointsTypeSelector, 1);
			Grid.SetRow(snapPointsAlignmentSelector, 2);
			Grid.SetRow(flowDirectionSelector, 3);
			Grid.SetRow(collectionView, 4);

			Content = layout;

			generator.GenerateItems();
		}
	}
}