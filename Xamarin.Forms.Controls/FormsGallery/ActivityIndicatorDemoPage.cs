using System;
using Xamarin.Forms;

namespace FormsGallery
{
	class ActivityIndicatorDemoPage : ContentPage
	{
		public ActivityIndicatorDemoPage()
		{
			Label header = new Label
			{
				Text = "ActivityIndicator",
				FontSize = 40,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center
			};

			ActivityIndicator activityIndicator = new ActivityIndicator
			{
				IsRunning = true,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					activityIndicator.Color = Color.Black;
					break;
				default:
					activityIndicator.Color = Color.Default;
					break;
			}

			// Build the page.
			this.Content = new StackLayout
			{
				Children =
				{
					header,
					activityIndicator
				}
			};
		}
	}
}
