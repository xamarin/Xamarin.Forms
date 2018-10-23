﻿using System;
using System.Globalization;
using Xamarin.Forms.Platform.WPF.Enums;

namespace Xamarin.Forms.Platform.WPF.Converters
{
	public class SymbolToValueConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Symbol symbol)
				return Char.ConvertFromUtf32((int)symbol);

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
