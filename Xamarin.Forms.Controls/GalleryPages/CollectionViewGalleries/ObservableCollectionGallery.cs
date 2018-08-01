namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class ObservableCollectionGallery : ContentPage
	{
		public ObservableCollectionGallery ()
		{
			var desc = "Observable Collection Galleries";

			var descriptionLabel = new Label { Text = desc, Margin = new Thickness(2,2,2,2)};

			Title = "Simple DataTemplate Galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryBuilder.NavButton("Add/Remove Items", () => 
							new ObservableCodeCollectionViewGridGallery (), Navigation),
					}
				}
			};
		}
	}
}