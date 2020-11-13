using Tizen.UIExtensions.Common;
using TColor = Tizen.UIExtensions.Common.Color;
using EColor = ElmSharp.Color;

namespace Microsoft.Maui
{
	public static class ColorExtensions
	{
		public static TColor ToNative(this Color c)
		{
			if (c.IsDefault)
			{
				// Trying to convert the default color, this may result in black color.
				return TColor.Default;
			}
			else
			{
				return new TColor((int)(255.0 * c.R), (int)(255.0 * c.G), (int)(255.0 * c.B), (int)(255.0 * c.A));
			}
		}

		public static EColor ToNativeEFL(this Color c)
		{
			if (c.IsDefault)
			{
				// Trying to convert the default color, this may result in black color.
				return EColor.Default;
			}
			else
			{
				return new EColor((int)(255.0 * c.R), (int)(255.0 * c.G), (int)(255.0 * c.B), (int)(255.0 * c.A));
			}
		}

		public static Color WithAlpha(this Color color, double alpha)
		{
			return new Color(color.R, color.G, color.B, (int)(255 * alpha));
		}

		public static Color WithPremultiplied(this Color color, double alpha)
		{
			return new Color((int)(color.R * alpha), (int)(color.G * alpha), (int)(color.B * alpha), color.A);
		}

		internal static string ToHex(this TColor c)
		{
			if (c.IsDefault)
			{
				Log.Warn("Trying to convert the default color to hexagonal notation, it does not works as expected.");
			}
			return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.R, c.G, c.B, c.A);
		}
	}
}