using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.UWP
{
	public class ImageConverter : Windows.UI.Xaml.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is ImageSource source)
			{
				var task = source.ToWindowsImageSourceAsync();
				return new AsyncValue<Windows.UI.Xaml.Media.ImageSource>(task, null);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}