namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class ObservableCollectionResetGallery : ContentPage
	{
		ItemsSourceGenerator generator;


		public ObservableCollectionResetGallery()
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

			var itemsLayout = new GridItemsLayout(3, ItemsLayoutOrientation.Vertical) as IItemsLayout;

			var itemTemplate = ExampleTemplates.PhotoTemplate();

			var collectionView = new CollectionView { ItemsLayout = itemsLayout, ItemTemplate = itemTemplate };

			generator = new ItemsSourceGenerator(collectionView, 100, ItemsSourceType.MultiTestObservableCollection);

			layout.Children.Add(generator);

			var resetter = new Resetter(collectionView);
			layout.Children.Add(resetter);
			Grid.SetRow(resetter, 1);

			layout.Children.Add(collectionView);
			Grid.SetRow(collectionView, 2);

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