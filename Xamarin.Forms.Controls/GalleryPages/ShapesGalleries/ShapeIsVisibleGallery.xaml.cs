using System;

namespace Xamarin.Forms.Controls.GalleryPages.ShapesGalleries
{
	public partial class ShapeIsVisibleGallery : ContentPage
	{
		public ShapeIsVisibleGallery()
		{
			InitializeComponent();
		}

		void ToggleEllipse1Clicked(object sender, EventArgs e)
		{
			ellipse1.IsVisible = !ellipse1.IsVisible;
		}
		
		void ToggleEllipse2Clicked(object sender, EventArgs e)
		{
			ellipse2.IsVisible = !ellipse2.IsVisible;
		}

		void ToggleEllipse3Clicked(object sender, EventArgs e)
		{
			ellipse3.IsVisible = !ellipse3.IsVisible;
		}
	}
}