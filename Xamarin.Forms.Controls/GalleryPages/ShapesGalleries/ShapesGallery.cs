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
                     GalleryBuilder.NavButton("Path Gallery", () => new PathGallery(), Navigation)
                 }
            };
        }
    }
}