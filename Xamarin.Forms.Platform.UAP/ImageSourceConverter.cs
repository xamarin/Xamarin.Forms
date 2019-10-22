using System;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	internal class ImageSourceConverter : Windows.UI.Xaml.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			// TODO: investigate whether we can use AsyncValue<T> instead of blocking

			if (value is ImageSource source)
				return source.ToWindowsImageSource();

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}