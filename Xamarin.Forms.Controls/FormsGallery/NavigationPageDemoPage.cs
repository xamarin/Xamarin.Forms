using System;
using Xamarin.Forms;

namespace FormsGallery
{
    // Although this page is actually a ContentPage, it can 
    //  function as a NavigationPage because the HomePage
    //  is launched as an ApplicationPage in App. 
    class NavigationPageDemoPage : ContentPage
    {
        public NavigationPageDemoPage()
        {
            Label header = new Label
            {
                Text = "NavigationPage",
				FontSize = 40,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            Button button1 = new Button
            {
                Text = " Go to Label Demo Page ",
                Font = Font.SystemFontOfSize(NamedSize.Large),
                BorderWidth = 1
            };
            button1.Clicked += async (sender, args) =>
                await Navigation.PushAsync(new LabelDemoPage());

            Button button2 = new Button
            {
                Text = " Go to Image Demo Page ",
                Font = Font.SystemFontOfSize(NamedSize.Large),
                BorderWidth = 1
            };
            button2.Clicked += async (sender, args) =>
                await Navigation.PushAsync(new ImageDemoPage());

            Button button3 = new Button
            {
                Text = " Go to BoxView Demo Page ",
                Font = Font.SystemFontOfSize(NamedSize.Large),
                BorderWidth = 1
            };
            button3.Clicked += async (sender, args) =>
                await Navigation.PushAsync(new BoxViewDemoPage());

            Button button4 = new Button
            {
                Text = " Go to WebView Demo Page ",
                Font = Font.SystemFontOfSize(NamedSize.Large),
                BorderWidth = 1
            };
            button4.Clicked += async (sender, args) =>
                await Navigation.PushAsync(new WebViewDemoPage());

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        Children = 
                        {
                            button1,
                            button2,
                            button3,
                            button4
                        }
                    }
                }
            };
        }
    }
}
