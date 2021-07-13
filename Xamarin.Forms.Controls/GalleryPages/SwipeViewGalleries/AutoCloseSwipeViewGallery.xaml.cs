namespace Xamarin.Forms.Controls.GalleryPages.SwipeViewGalleries
{
	public partial class AutoCloseSwipeViewGallery : ContentPage
	{
		public AutoCloseSwipeViewGallery()
		{
			InitializeComponent();
			BindingContext = new SwipeViewGalleryViewModel();

			MessagingCenter.Subscribe<SwipeViewGalleryViewModel>(this, "favourite", sender => { DisplayAlert("SwipeView", "Favourite", "Ok"); });
			MessagingCenter.Subscribe<SwipeViewGalleryViewModel>(this, "delete", sender => { DisplayAlert("SwipeView", "Delete", "Ok"); });
		}

		async void OnSwipeListViewItemTapped(object sender, ItemTappedEventArgs args)
		{
			await DisplayAlert("OnSwipeListViewItemTapped", "You have tapped a ListView item", "Ok");
		}
	}
}