using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class WebViewDemoPage : ContentPage
    {
        public WebViewDemoPage()
        {
            Label header = new Label
            {
                Text = "WebView",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            WebView webView = new WebView
            {
                Source = new UrlWebViewSource
                {
                    Url = "https://blog.xamarin.com/",
                },
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    webView
                }
            };
        }
    }
}
