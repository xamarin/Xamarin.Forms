using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class TabbedPageDemoPage : TabbedPage
    {
        public TabbedPageDemoPage()
        {
            this.Title = "TabbedPage";

            this.ItemsSource = new NamedColor[] 
            {
                new NamedColor("Red", Color.Red),
                new NamedColor("Yellow", Color.Yellow),
                new NamedColor("Green", Color.Green),
                new NamedColor("Aqua", Color.Aqua),
                new NamedColor("Blue", Color.Blue),
                new NamedColor("Purple", Color.Purple)
            };

            this.ItemTemplate = new DataTemplate(() => 
            { 
                return new NamedColorPage(false); 
            });
        }
    }
}
