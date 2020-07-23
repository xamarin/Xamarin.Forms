using System;
using System.Globalization;

namespace Xamarin.Forms
{
	internal class ContentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is View view)
			{
				return view;
			}

			if (value is string textContent)
			{
				return new Label
				{
					Text = textContent
				};
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}