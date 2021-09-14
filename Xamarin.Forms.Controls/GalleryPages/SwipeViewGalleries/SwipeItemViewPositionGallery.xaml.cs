﻿using System;

namespace Xamarin.Forms.Controls.GalleryPages.SwipeViewGalleries
{
	public partial class SwipeItemViewPositionGallery : ContentPage
	{
		public SwipeItemViewPositionGallery()
		{
			InitializeComponent();
			ModePicker.SelectedIndex = 0;
		}

		void OnModePickerSelectedIndexChanged(object sender, EventArgs e)
		{
			LeftSwipeItems.Mode = TopSwipeItems.Mode = RightSwipeItems.Mode = BottomSwipeItems.Mode = ModePicker.SelectedIndex == 0 ? SwipeMode.Reveal : SwipeMode.Execute;
		}
	}
}