using System;
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
			if (parts.Length == 2 && parts[0] == nameof(Easing))
				value = parts[parts.Length - 1];

			switch (value.ToLowerInvariant().Trim())
			{
				case "linear": return Linear;
				case "sinin": return SinIn;
				case "sinout": return SinOut;
				case "sininout": return SinInOut;
				case "cubicin": return CubicIn;
				case "cubicout": return CubicOut;
				case "cubicinout": return CubicInOut;
				case "bouncein": return BounceIn;
				case "bounceout": return BounceOut;
				case "springin": return SpringIn;
				case "springout": return SpringOut;
				default: throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(Easing)}");
			}
		}
	}
}
