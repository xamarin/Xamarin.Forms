﻿namespace Xamarin.Forms.Controls
{
	public class StackLayoutGallery : ContentPage
	{
		public StackLayoutGallery()
		{
			if (Device.RuntimePlatform == Device.iOS && Device.Idiom == TargetIdiom.Tablet)
				Padding = new Thickness(0, 0, 0, 60);

			var stack = new StackLayout { Orientation = StackOrientation.Vertical };
			var b1 = new Button { Text = "Boring", HeightRequest = 500, MinimumHeightRequest = 50 };
			var b2 = new Button
			{
				Text = "Exciting!",
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};
			var b3 = new Button { Text = "Amazing!", VerticalOptions = LayoutOptions.FillAndExpand };
			var b4 = new Button { Text = "Meh", HeightRequest = 400, MinimumHeightRequest = 50 };
			b1.Clicked += (sender, e) => { b1.Text = "clicked1"; };
			b2.Clicked += (sender, e) => { b2.Text = "clicked2"; };
			b3.Clicked += (sender, e) => { b3.Text = "clicked3"; };
			b4.Clicked += (sender, e) => { b4.Text = "clicked4"; };
			stack.Children.Add(b1);
			stack.Children.Add(b2);
			stack.Children.Add(b3);
			stack.Children.Add(b4);
			Content = stack;
		}
	}
}