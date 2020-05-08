using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms
{
	[TypeConverter(typeof(AutoFitTextModeConverter))]
	public enum AutoFitTextMode
	{
		None,
		FitToWidth
	}

	[Xaml.TypeConversion(typeof(AutoFitTextMode))]
	public class AutoFitTextModeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null)
			{
				if (value.Equals("None", StringComparison.OrdinalIgnoreCase))
					return AutoFitTextMode.None;
				if (value.Equals("FitToWidth", StringComparison.OrdinalIgnoreCase))
					return AutoFitTextMode.FitToWidth;

				if (Enum.TryParse(value, out TextAlignment direction))
					return direction;
			}

			throw new InvalidOperationException($"Cannot convert \"{value}\" into {nameof(AutoFitTextMode)}");
		}
	}
}
