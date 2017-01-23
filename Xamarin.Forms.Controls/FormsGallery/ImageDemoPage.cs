using System;
using Xamarin.Forms;

namespace FormsGallery
{
	class ImageDemoPage : ContentPage
	{
		public ImageDemoPage()
		{
			Label header = new Label
			{
				Text = "Image",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center
			};

			Image image = new Image
			{
				Source = ImageSource.FromUri(new Uri("https://www.xamarin.com/content/images/pages/branding/assets/xamagon.png")),
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			// Build the page.
			this.Content = new StackLayout
			{
				Children =
				{
					header,
					image
				}
			};
		}
	}
}
