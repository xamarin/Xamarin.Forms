using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.SwipeViewGalleries
{
	[Preserve(AllMembers = true)]
	public class CustomSwipeItemGallery : ContentPage
	{
		public CustomSwipeItemGallery()
		{
			Title = "CollectionView Galleries";
			Content = new StackLayout
			{
				Children =
				{
					GalleryBuilder.NavButton("Customize SwipeItem Gallery", () => new CustomizeSwipeItemGallery(), Navigation),
					GalleryBuilder.NavButton("CustomSwipeItem Gallery", () => new CustomSwipeItemViewGallery(), Navigation)
				}
			};
		}
	}
}