using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatusBarGallery : ContentPage
	{
		public StatusBarGallery()
		{
			InitializeComponent();
		}

		void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
		{
			StatusBarColor = Color.FromRgb(Convert.ToInt32(RedSlider.Value), Convert.ToInt32(GreenSlider.Value), Convert.ToInt32(BlueSlider.Value));
		}

		void Switch_OnToggled(object sender, ToggledEventArgs e)
		{
			StatusBarStyle = e.Value ? StatusBarStyle.DarkContent : StatusBarStyle.LightContent;
		}
	}
}