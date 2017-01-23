using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsGallery
{
    class CarouselPageDemoPage : CarouselPage
    {
        public CarouselPageDemoPage()
        {
            this.Title = "CarouselPage";

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
                return new NamedColorPage(true);
            });
        }
    }
}
