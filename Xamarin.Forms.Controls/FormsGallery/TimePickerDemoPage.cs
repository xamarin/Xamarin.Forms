using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class TimePickerDemoPage : ContentPage
    {
        public TimePickerDemoPage()
        {
            Label header = new Label
            {
                Text = "TimePicker",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            TimePicker timePicker = new TimePicker
            {
                Format = "T",
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    timePicker
                }
            };
        }
    }
}
