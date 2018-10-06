using System;
using System.Globalization;

namespace Xamarin.Forms
{
	public class ToStringValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}

			if (value is IFormattable formattable)
			{
				return formattable.ToString(null, culture);
			}

			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
