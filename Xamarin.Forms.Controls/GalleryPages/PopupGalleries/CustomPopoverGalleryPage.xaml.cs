using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CustomPopoverGalleryPage : ContentPage
	{
		public CustomPopoverGalleryPage()
		{
			InitializeComponent();
			position.SelectedItem = "Center";
			heightSlider.Maximum = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height;
			widthSlider.Maximum = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width;
		}

		private async void ShowDialog_Clicked(object sender, EventArgs e)
		{
			var popup = new SimplePopup()
			{
				Size = new Size(widthSlider.Value, heightSlider.Value)
			};

			ConfigureDialogPosition(popup);
			await Navigation.ShowPopup(popup);
		}

		void ConfigureDialogPosition(Popup popup)
		{
			if ((string)position.SelectedItem == "Center")
			{
				popup.HorizontalOptions = LayoutOptions.CenterAndExpand;
				popup.VerticalOptions = LayoutOptions.CenterAndExpand;
			}
			else if ((string)position.SelectedItem == "Top")
			{
				popup.HorizontalOptions = LayoutOptions.CenterAndExpand;
				popup.VerticalOptions = LayoutOptions.StartAndExpand;
			}
			else if ((string)position.SelectedItem == "TopRight")
			{
				popup.HorizontalOptions = LayoutOptions.EndAndExpand;
				popup.VerticalOptions = LayoutOptions.StartAndExpand;
			}
			else if ((string)position.SelectedItem == "Right")
			{
				popup.HorizontalOptions = LayoutOptions.EndAndExpand;
				popup.VerticalOptions = LayoutOptions.CenterAndExpand;
			}
			else if ((string)position.SelectedItem == "BottomRight")
			{
				popup.HorizontalOptions = LayoutOptions.EndAndExpand;
				popup.VerticalOptions = LayoutOptions.EndAndExpand;
			}
			else if ((string)position.SelectedItem == "Bottom")
			{
				popup.HorizontalOptions = LayoutOptions.CenterAndExpand;
				popup.VerticalOptions = LayoutOptions.EndAndExpand;
			}
			else if ((string)position.SelectedItem == "BottomLeft")
			{
				popup.HorizontalOptions = LayoutOptions.StartAndExpand;
				popup.VerticalOptions = LayoutOptions.EndAndExpand;
			}
			else if ((string)position.SelectedItem == "Left")
			{
				popup.HorizontalOptions = LayoutOptions.StartAndExpand;
				popup.VerticalOptions = LayoutOptions.CenterAndExpand;
			}
			else if ((string)position.SelectedItem == "TopLeft")
			{
				popup.HorizontalOptions = LayoutOptions.StartAndExpand;
				popup.VerticalOptions = LayoutOptions.StartAndExpand;
			}
		}
	}
}