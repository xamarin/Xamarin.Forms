using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.Markup
{
	public class FuncConverter<TSource, TDest> : IValueConverter
	{
		readonly Func<TSource, TDest> convert;
		readonly Func<TDest, TSource> convertBack;

		public FuncConverter(Func<TSource, TDest> convert = null, Func<TDest, TSource> convertBack = null)
		{ this.convert = convert; this.convertBack = convertBack; }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			=> convert != null ? convert.Invoke(value != null ? (TSource)value : default(TSource)) : default(TDest);

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			=> convertBack != null ? convertBack.Invoke(value != null ? (TDest)value : default(TDest)) : default(TSource);
	}

	public class FuncConverter<TSource> : FuncConverter<TSource, object>
	{
		public FuncConverter(Func<TSource, object> convert = null, Func<object, TSource> convertBack = null)
			: base(convert, convertBack) { }
	}

	public class FuncConverter : FuncConverter<object, object>
	{
		public FuncConverter(Func<object, object> convert = null, Func<object, object> convertBack = null)
			: base(convert, convertBack) { }
	}

	public class ToStringConverter : FuncConverter
	{
		public ToStringConverter(string format = "{0}")
			: base(o => string.Format(CultureInfo.InvariantCulture, format, o)) { }
	}

	public class BoolNotConverter : FuncConverter<bool>
	{
		static readonly Lazy<BoolNotConverter> instance = new Lazy<BoolNotConverter>(() => new BoolNotConverter());
		public static BoolNotConverter Instance => instance.Value;
		public BoolNotConverter() : base(t => !t) { }
	}
}
