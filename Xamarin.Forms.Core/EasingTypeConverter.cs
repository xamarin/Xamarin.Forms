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

			switch (value)
			{
				case string easing when AreEqual(easing, nameof(Linear)): return Linear;
				case string easing when AreEqual(easing, nameof(SinIn)): return SinIn;
				case string easing when AreEqual(easing, nameof(SinOut)): return SinOut;
				case string easing when AreEqual(easing, nameof(SinInOut)): return SinInOut;
				case string easing when AreEqual(easing, nameof(CubicIn)): return CubicIn;
				case string easing when AreEqual(easing, nameof(CubicOut)): return CubicOut;
				case string easing when AreEqual(easing, nameof(CubicInOut)): return CubicInOut;
				case string easing when AreEqual(easing, nameof(BounceIn)): return BounceIn;
				case string easing when AreEqual(easing, nameof(BounceOut)): return BounceOut;
				case string easing when AreEqual(easing, nameof(SpringIn)): return SpringIn;
				case string easing when AreEqual(easing, nameof(SpringOut)): return SpringOut;
				default: throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(Easing)}");
			}
		}

		bool AreEqual(string first, string second)
			=> first.Equals(second, StringComparison.OrdinalIgnoreCase);
	}
}
