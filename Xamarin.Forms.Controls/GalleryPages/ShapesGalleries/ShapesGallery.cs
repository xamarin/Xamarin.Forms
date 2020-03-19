using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.ShapesGalleries
{
    [Preserve(AllMembers = true)]
    public class ShapesGallery : ContentPage
    {
        public ShapesGallery()
        {
            Title = "Shapes Gallery";

            var button = new Button
            {
                Text = "Enable Shapes",
                AutomationId = "EnableShapes"
            };
            button.Clicked += ButtonClicked;

            Content = new StackLayout
            {
                Children =
				{
					button,
                    GalleryBuilder.NavButton("Path Gallery", () => new PathGallery(), Navigation),
					GalleryBuilder.NavButton("Transform Playground", () => new TransformPlaygroundGallery(), Navigation),
					GalleryBuilder.NavButton("Path Transform using string (TypeConverter) Gallery", () => new PathTransformStringGallery(), Navigation)
                }
            };
        }

        void ButtonClicked(object sender, System.EventArgs e)
        {
            var button = sender as Button;

            button.Text = "Shapes Enabled!";
            button.TextColor = Color.Black;
            button.IsEnabled = false;

            Device.SetFlags(new[] { ExperimentalFlags.ShapesExperimental });
        }
    }
}