using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class EntryCellDemoPage : ContentPage
    {
        public EntryCellDemoPage()
        {
            Label header = new Label
            {
                Text = "EntryCell",
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
                        new EntryCell
                        {
                            Label = "EntryCell:",
                            Placeholder = "Type Text Here"
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
