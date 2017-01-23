using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class SwitchCellDemoPage : ContentPage
    {
        public SwitchCellDemoPage()
        {
            Label header = new Label
            {
                Text = "SwitchCell",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            TableView tableView = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot
                {
                    new TableSection
                    {
                        new SwitchCell
                        {
                            Text = "SwitchCell:"
                        }
                    }
                }
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    tableView
                }
            };
        }
    }
}
