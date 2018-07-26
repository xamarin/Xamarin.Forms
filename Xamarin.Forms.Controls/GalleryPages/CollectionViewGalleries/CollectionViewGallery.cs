namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	public class CollectionViewGallery : ContentPage
	{
		public CollectionViewGallery()
		{
			Content = new StackLayout
			{
				Children =
				{
					GalleryBuilder.NavButton("Default Text Galleries", () => new DefaultTextGallery(), Navigation),
					GalleryBuilder.NavButton("DataTemplate Galleries", () => new DataTemplateGallery(), Navigation),
					GalleryBuilder.NavButton("Observable Collection Galleries", () => new ObservableCollectionGallery(), Navigation),
				}
			};
		}
	}
}
