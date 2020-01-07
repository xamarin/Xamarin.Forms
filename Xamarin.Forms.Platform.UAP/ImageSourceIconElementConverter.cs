﻿using System;

namespace Xamarin.Forms.Platform.UWP
{
	internal class ImageSourceIconElementConverter : Windows.UI.Xaml.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is ImageSource source)
				return source.ToWindowsIconElement();

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}