using System;
using System.Globalization;

namespace Xamarin.Forms.Controls
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, 
                              object parameter, CultureInfo culture)
        {
            // Assumes value is double.
            double number = (double)value;

            // Return empty string for a zero (good for Entry views).
            if (number == 0)
            {
                return "";
            }

            return number.ToString();
        }

        public object ConvertBack(object value, Type targetType, 
                                  object parameter, CultureInfo culture)
        {
            double number = 0;
            Double.TryParse((string)value, out number);
            return number;
        }
    }
}
