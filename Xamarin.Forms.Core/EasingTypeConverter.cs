﻿using System;
using static Xamarin.Forms.Easing;

namespace Xamarin.Forms
{
	[Xaml.ProvideCompiled("Xamarin.Forms.Core.XamlC.EasingTypeConverter")]
	[Xaml.TypeConversion(typeof(Easing))]
	public class EasingTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			value = value?.Trim() ?? "";
			var parts = value.Split('.');
			if (parts.Length == 2 && parts[0].Equals(nameof(Easing), StringComparison.OrdinalIgnoreCase))
				value = parts[parts.Length - 1];

			if (value.Equals(nameof(Linear), StringComparison.OrdinalIgnoreCase))
				return Linear;
			if (value.Equals(nameof(SinIn), StringComparison.OrdinalIgnoreCase))
				return SinIn;
			if (value.Equals(nameof(SinOut), StringComparison.OrdinalIgnoreCase))
				return SinOut;
			if (value.Equals(nameof(SinInOut), StringComparison.OrdinalIgnoreCase))
				return SinInOut;
			if (value.Equals(nameof(CubicIn), StringComparison.OrdinalIgnoreCase))
				return CubicIn;
			if (value.Equals(nameof(CubicOut), StringComparison.OrdinalIgnoreCase))
				return CubicOut;
			if (value.Equals(nameof(CubicInOut), StringComparison.OrdinalIgnoreCase))
				return CubicInOut;
			if (value.Equals(nameof(BounceIn), StringComparison.OrdinalIgnoreCase))
				return BounceIn;
			if (value.Equals(nameof(BounceOut), StringComparison.OrdinalIgnoreCase))
				return BounceOut;
			if (value.Equals(nameof(SpringIn), StringComparison.OrdinalIgnoreCase))
				return SpringIn;
			if (value.Equals(nameof(SpringOut), StringComparison.OrdinalIgnoreCase))
				return SpringOut;

			throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(Easing)}");
		}
	}
}
