﻿using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls.ControlGallery.GalleryPages.SwipeViewGalleries
{
	[Preserve(AllMembers = true)]
	public partial class SwipeListViewGallery : ContentPage
	{
		public SwipeListViewGallery()
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