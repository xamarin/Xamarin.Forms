using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms
{
	[Flags]
	[TypeConverter(typeof(TextDecorationConverter))]
	public enum TextDecorations
	{
		None = 0,
		Underline = 1 << 0,
		Strikethrough = 1 << 1,
	}
	static class DecorableTextElement
	{
		public static readonly BindableProperty TextDecorationsProperty = BindableProperty.Create("TextDecorations", typeof(TextDecorations), typeof(IDecorableTextElement), TextDecorations.None);
	}

	[Xaml.TypeConversion(typeof(TextDecorations))]
	public class TextDecorationConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null)
			{
				if (Enum.TryParse(value, true, out TextDecorations textDecorations))
					return textDecorations;
				if (value.Equals("line-through", StringComparison.OrdinalIgnoreCase))
					return TextDecorations.Strikethrough;
			}
			throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(TextDecorations)));
		}
	}
}
