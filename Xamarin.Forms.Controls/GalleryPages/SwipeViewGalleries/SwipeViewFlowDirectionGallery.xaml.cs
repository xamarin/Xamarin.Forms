using System;

namespace Xamarin.Forms.Controls.GalleryPages.SwipeViewGalleries
{
	public partial class SwipeViewFlowDirectionGallery : ContentPage
	{
		public SwipeViewFlowDirectionGallery()
		{
			InitializeComponent();
			BindingContext = new SwipeViewGalleryViewModel();

			MessagingCenter.Subscribe<SwipeViewGalleryViewModel>(this, "favourite", sender => { DisplayAlert("SwipeView", "Favourite", "Ok"); });
			MessagingCenter.Subscribe<SwipeViewGalleryViewModel>(this, "delete", sender => { DisplayAlert("SwipeView", "Delete", "Ok"); });

			FlowDirectionInfo.Text = SwipeCollectionView.FlowDirection.ToString();
		}

		async void OnSwipeCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs args)
		{
			await DisplayAlert("OnSwipeCollectionViewSelectionChanged", "CollectionView SelectionChanged", "Ok");
		}

		void OnChangeFlowDirectionClicked(object sender, EventArgs e)
		{
			if (SwipeCollectionView.FlowDirection == FlowDirection.RightToLeft)
				SwipeCollectionView.FlowDirection = FlowDirection.LeftToRight;
			else
				SwipeCollectionView.FlowDirection = FlowDirection.RightToLeft;

			FlowDirectionInfo.Text = SwipeCollectionView.FlowDirection.ToString();
		}
	}
}