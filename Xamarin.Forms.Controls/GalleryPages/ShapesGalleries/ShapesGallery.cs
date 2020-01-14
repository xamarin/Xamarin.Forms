using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.ShapesGalleries
{
	[Preserve(AllMembers = true)]
	public class ShapesGallery : ContentPage
	{
		public ShapesGallery()
		{
			Title = "Shapes Gallery";

			Content = new StackLayout
			{
				Children =
				{
					GalleryBuilder.NavButton("Ellipse Gallery", () => new EllipseGallery(), Navigation),
					GalleryBuilder.NavButton("Line Gallery", () => new LineGallery(), Navigation),
					GalleryBuilder.NavButton("Polygon Gallery", () => new PolygonGallery(), Navigation),
					GalleryBuilder.NavButton("Polyline Gallery", () => new PolylineGallery(), Navigation),
					GalleryBuilder.NavButton("Rectangle Gallery", () => new RectGallery(), Navigation)
				}
			};
		}
	}
}