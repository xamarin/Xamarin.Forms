using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class SwitchDemoPage : ContentPage
    {
        Label label;

        public SwitchDemoPage()
        {
            Label header = new Label
            {
                Text = "Switch",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            Switch switcher = new Switch
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            switcher.Toggled += switcher_Toggled;

            label = new Label
            {
                Text = "Switch is now False",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    switcher,
                    label
                }
            };
        }

        void switcher_Toggled(object sender, ToggledEventArgs e)
        {
            label.Text = String.Format("Switch is now {0}", e.Value);
        }
    }
}
