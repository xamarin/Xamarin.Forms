using System;
using Xamarin.Forms;

namespace FormsGallery
{
    // Used in TabbedPageDemoPage & CarouselPageDemoPage.
    class NamedColor
    {
        public NamedColor(string name, Color color)
        {
            this.Name = name;
            this.Color = color;
        }

        public string Name { private set; get; }

        public Color Color { private set; get; }

        public override string ToString()
        {
            return Name;
        }
    }

}
