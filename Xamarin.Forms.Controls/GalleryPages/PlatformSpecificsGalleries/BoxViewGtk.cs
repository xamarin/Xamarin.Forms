using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.GTKSpecific;

namespace Xamarin.Forms.Controls.GalleryPages.PlatformSpecificsGalleries
{
    public class BoxViewGtk : ContentPage
    {
        private bool _hasCornerRadius;

        public BoxViewGtk()
        {
            var box = new BoxView
            {
                Color = Color.Red
            };

            Content = new StackLayout
            {
                Children =
                {
                    box,
                    new Button
                    {
                        Text = "Toggle HasCornerRadius",
                        Command = new Command(() =>
                        {
                            _hasCornerRadius = !_hasCornerRadius;
                            box.On<GTK>().SetHasCornerRadius(_hasCornerRadius);
                        })
                    }
                }
            };
        }
    }
}
