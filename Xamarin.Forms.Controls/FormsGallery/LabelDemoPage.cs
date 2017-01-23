using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class LabelDemoPage : ContentPage
    {
        public LabelDemoPage()
        {
            Label header = new Label
            {
                Text = "Label",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            Label label = new Label
            {
                Text =
                    "Xamarin.Forms is a cross-platform natively " +
                    "backed UI toolkit abstraction that allows " +
                    "developers to easily create user interfaces " +
                    "that can be shared across Android, iOS, and " +
                    "Windows Phone.",

				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    label
                }
            };
        }
    }
}
