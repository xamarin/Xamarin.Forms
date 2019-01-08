using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.UWP
{
	public class ImageConverter : Windows.UI.Xaml.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var source = (ImageSource)value;
			var task = source.ToWindowsImageSourceAsync();
			return new AsyncValue<Windows.UI.Xaml.Media.ImageSource>(task, null);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}