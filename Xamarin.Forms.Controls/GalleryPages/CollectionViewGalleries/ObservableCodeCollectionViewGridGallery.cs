namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class ObservableCodeCollectionViewGridGallery : ContentPage
	{
		public ObservableCodeCollectionViewGridGallery(ItemsLayoutOrientation orientation = ItemsLayoutOrientation.Vertical)
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

			var itemsLayout = new GridItemsLayout(2, orientation);

			var itemTemplate = ExampleTemplates.PhotoTemplate();

			var collectionView = new CollectionView {ItemsLayout = itemsLayout, ItemTemplate = itemTemplate};

			var generator = new ItemsSourceGenerator(collectionView);
			var spanSetter = new SpanSetter(collectionView);
			var remover = new ItemRemover(collectionView);

			layout.Children.Add(generator);
			layout.Children.Add(spanSetter);
			Grid.SetRow(spanSetter, 1);

			layout.Children.Add(remover);
			Grid.SetRow(remover, 2);

			layout.Children.Add(collectionView);
			Grid.SetRow(collectionView, 3);

			Content = layout;

			spanSetter.UpdateSpan();
			generator.GenerateObservableCollection();
		}
	}
}