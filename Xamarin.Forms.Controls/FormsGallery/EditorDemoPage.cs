using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class EditorDemoPage : ContentPage
    {
        public EditorDemoPage()
        {
            Label header = new Label
            {
                Text = "Editor",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            Editor editor = new Editor
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    editor
                }
            };
        }
    }
}
