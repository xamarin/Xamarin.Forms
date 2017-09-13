using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.GTKSpecific;

namespace Xamarin.Forms.Controls.GalleryPages.PlatformSpecificsGalleries
{
    public class NavigationPageGtk : NavigationPage
    {
        public NavigationPageGtk() : base (new TestPage ())
		{
            BarBackgroundColor = Color.Pink;
            BarTextColor = Color.White;

            this.On<GTK>().SetBackButtonIcon("gtk_back_button.png");
        }
    }

    public class TestPage : ContentPage
    {
        public TestPage()
        {
            Content = new StackLayout
            {
                Children = {
                        new Button {
                            Text = "Push", Command = new Command (o => ((NavigationPage) Parent).PushAsync (new TestPage ()))
                        },
                        new Button {
                            Text = "Pop", Command = new Command (o => ((NavigationPage) Parent).Navigation.PopAsync ())
                        },
                    },
            };
        }
    }
}