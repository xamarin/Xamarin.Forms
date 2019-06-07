using System;

namespace Xamarin.Forms.Controls
{
	public class PopupLabelGallery : ContentPage
	{
		public PopupLabelGallery()
		{
			Content = new StackLayout
			{
				Children =
				{
					LayoutOptionsButton(),
					PopupLabelColorGallery()
				}
			};
		}

		private View PopupLabelColorGallery()
		{
			var button = new Button
			{
				Text = "Label - Color"
			};
			button.Clicked += async (s, e) =>
			{
				await Navigation.PushAsync(new PopupLabelColorGallery());
			};
			return button;
		}

		private Button LayoutOptionsButton()
		{
			var button = new Button
			{
				Text = "Label - Layout Options"
			};
			button.Clicked += async (s, e) =>
			{
				await Navigation.PushAsync(new PopupLabelLayoutOptionsGallery());
			};

			return button;
		}
	}
}
