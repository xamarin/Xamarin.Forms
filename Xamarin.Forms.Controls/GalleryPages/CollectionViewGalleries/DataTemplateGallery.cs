namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class DataTemplateGallery : ContentPage
	{
		public DataTemplateGallery()
		{
			var descriptionLabel =
				new Label { Text = "Simple DataTemplate Galleries", Margin = new Thickness(2, 2, 2, 2) };

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

	internal class SnapPointsGallery : ContentPage
	{
		public SnapPointsGallery()
		{
			var descriptionLabel =
				new Label { Text = "Snap Points Galleries", Margin = new Thickness(2, 2, 2, 2) };

			Title = "Snap Points Galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryBuilder.NavButton("Snap Points (Code, Horizontal List)", () => 
							new SnapPointsHorizontalCodeGallery(), Navigation),
					}
				}
			};
		}
	}

	internal class SnapPointsHorizontalCodeGallery : ContentPage
	{
		public SnapPointsHorizontalCodeGallery()
		{
			Title = "Snap Points (Code, Horizontal List)";

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
				new ListItemsLayout(ItemsLayoutOrientation.Horizontal)
				{
					SnapPointsType = SnapPointsType.None,
					SnapPointsAlignment = SnapPointsAlignment.Start
				};

			var itemTemplate = ExampleTemplates.SnapPointsTemplate();

			var collectionView = new CollectionView
			{
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate
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