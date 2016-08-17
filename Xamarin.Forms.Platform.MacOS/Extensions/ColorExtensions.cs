using System;
using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	public static class ColorExtensions
	{
		internal static readonly NSColor Black = NSColor.Black;
		internal static readonly NSColor SeventyPercentGrey = NSColor.FromRgba(0.7f, 0.7f, 0.7f, 1);

		public static CGColor ToCGColor(this Color color)
		{
			return new CGColor((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		public static Color ToColor(this NSColor color)
		{
			nfloat red;
			nfloat green;
			nfloat blue;
			nfloat alpha;
			color.GetRgba(out red, out green, out blue, out alpha);
			return new Color(red, green, blue, alpha);
		}

		public static NSColor ToNSColor(this Color color)
		{
			return NSColor.FromRgba((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		public static NSColor ToNSColor(this Color color, Color defaultColor)
		{
			if (color.IsDefault)
				return defaultColor.ToNSColor();

			return color.ToNSColor();
		}

		public static NSColor ToUIColor(this Color color, NSColor defaultColor)
		{
			if (color.IsDefault)
				return defaultColor;

			return color.ToNSColor();
		}
	}
}

