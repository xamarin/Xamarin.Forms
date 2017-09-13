using System;
using System.Globalization;

namespace Xamarin.Forms.Controls
{
    public class DoubleRoundingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return Round((double)value, parameter);
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return Round((double)value, parameter);
        }

        double Round(double number, object parameter)
        {
            double precision = 1;

            // Assume parameter is string encoding precision.
            if (parameter != null)
            {
                precision = Double.Parse((string)parameter);
            }
            return precision * Math.Round(number / precision);
        }
    }
}
