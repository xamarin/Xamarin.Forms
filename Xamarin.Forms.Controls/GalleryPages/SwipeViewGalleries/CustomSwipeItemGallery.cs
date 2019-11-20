using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.SwipeViewGalleries
{
	[Preserve(AllMembers = true)]
	public class CustomSwipeItemGallery : ContentPage
	{
		public CustomSwipeItemGallery()
		{
			Title = "CollectionView Galleries";
			var layout = new StackLayout
			{
				Children =
				{
					GalleryBuilder.NavButton("Customize SwipeItem Gallery", () => new CustomizeSwipeItemGallery(), Navigation)
				}
			};

			if (Device.RuntimePlatform != Device.UWP)
				layout.Children.Add(GalleryBuilder.NavButton("CustomSwipeItem Gallery", () => new CustomSwipeItemViewGallery(), Navigation));

			Content = layout;

		}
	}
}