using System;

namespace Xamarin.Forms.Controls.GalleryPages.GradientGalleries
{
	public partial class UpdateGradientColorGallery : ContentPage
	{
		readonly Random _random;

		public UpdateGradientColorGallery()
		{
			InitializeComponent();
			_random = new Random();
		}

		void OnUpdateColorsClicked(object sender, EventArgs e)
		{
			GradientStop firstStop = linearBrush.GradientStops[GetRandomGradientStop()];
			firstStop.Color = GetRandomColor();
		}

		int GetRandomGradientStop()
		{
			return _random.Next(3);
		}

		Color GetRandomColor()
		{
			return Color.FromRgb(_random.Next(256), _random.Next(256), _random.Next(256));
		}
	}
}