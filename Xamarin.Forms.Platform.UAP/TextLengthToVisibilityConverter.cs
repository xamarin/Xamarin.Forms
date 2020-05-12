using System;
using Windows.UI.Xaml;

namespace Xamarin.Forms.Platform.UWP
{
	public class TextLengthToVisibilityConverter : Windows.UI.Xaml.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return string.IsNullOrEmpty(value?.ToString())?Visibility.Collapsed:Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}