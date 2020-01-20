using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
    [RenderWith(typeof(_PathRenderer))]
    public class Path : Shape
    {
        public static readonly BindableProperty DataProperty =
             BindableProperty.Create(nameof(Data), typeof(Geometry), typeof(Path), null);

        [TypeConverter(typeof(PathGeometryConverter))]
        public Geometry Data
        {
            set { SetValue(DataProperty, value); }
            get { return (Geometry)GetValue(DataProperty); }
        }
    }
}