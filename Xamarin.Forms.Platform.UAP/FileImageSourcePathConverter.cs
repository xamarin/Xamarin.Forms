using System;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	internal class FileImageSourcePathConverter : Windows.UI.Xaml.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is FileImageSource source)
			{
				string uri = "ms-appx:///" + (source != null ? source.File : string.Empty);
				return new BitmapIcon { UriSource = new Uri(uri) };
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}