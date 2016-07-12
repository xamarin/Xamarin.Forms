using System;
using System.Globalization;

namespace Xamarin.Forms.Platform.Android
{
	public class ColorConverter : INativeValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Color)
				return ((Color)value).ToAndroid();

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is global::Android.Graphics.Color)
				return ((global::Android.Graphics.Color)value).ToColor();

			return null;
		}
	}
}

