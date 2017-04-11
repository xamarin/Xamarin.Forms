using System;
using System.Diagnostics;

namespace Xamarin.Forms.Controls
{
	public class SwipeGestureGalleryPage : ContentPage
	{

		public SwipeGestureGalleryPage()
		{
			BoxView bv = new BoxView
			{
				BackgroundColor = Color.Red,
				HeightRequest = 150
			};

			var label = new Label { Text = "Use one finger and swipe inside the gray box." };

			SwipeGestureRecognizer swipeG = new SwipeGestureRecognizer();
			swipeG.SwipeLeft += (sender, e) =>
			{
				label.Text = "test";
			};
			swipeG.SwipeRight += (sender, args) => label.Text = "You swiped right.";
			swipeG.SwipeUp += (sender, args) => label.Text = "You swiped up.";
			swipeG.SwipeDown += (sender, args) => label.Text = "You swiped down.";

			bv.GestureRecognizers.Add(swipeG);

			Content = new StackLayout { Children = { label, bv }, Padding = new Thickness(20) };
		}
	}
}