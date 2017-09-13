using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.GTKSpecific;

namespace Xamarin.Forms.Controls.GalleryPages.PlatformSpecificsGalleries
{
    public class TabbedPageGtk : TabbedPage
    {
        public TabbedPageGtk()
        {
            Children.Add(CreatePage("Page One"));
            Children.Add(CreatePage("Page Two"));
        }

        private ContentPage CreatePage(string title)
        {
            var page = new ContentPage
            {
                Title = title
            };

            var content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 20, 0, 0)
            };

            content.Children.Add(new Label
            {
                Text = "TabbedPage GTK Features",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            });

            var toggleTabPositionButton = new Button
            {
                Text = "Toggle TabPosition for TabbedPage"
            };

            toggleTabPositionButton.Command = new Command(() =>
            {
                var mode = On<GTK>().GetTabPosition();

                if (mode == TabPosition.Top)
                    On<GTK>().SetTabPosition(TabPosition.Top);
                else
                    On<GTK>().SetTabPosition(TabPosition.Bottom);
            });

            content.Children.Add(toggleTabPositionButton);

            page.Content = content;

            return page;
        }
    }
}