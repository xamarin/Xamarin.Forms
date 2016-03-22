﻿using System;
using Windows.UI.Xaml;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public sealed class BoolToVisibilityConverter : Windows.UI.Xaml.Data.IValueConverter
	{
		public bool FalseIsVisible { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var v = (bool)value;
			if (FalseIsVisible)
				v = !v;

			return v ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}