using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
    [TypeConversion(typeof(Brush))]
    public class BrushTypeConverter : TypeConverter
    {
        readonly ColorTypeConverter _colorTypeConverter = new ColorTypeConverter();

        public override object ConvertFromInvariantString(string value)
        {
            return new SolidColorBrush()
            {
                Color = (Color)_colorTypeConverter.ConvertFromInvariantString(value)
            };
        }
    }
}