using System.Drawing;
using System.Globalization;

namespace Resizetizer
{
	internal class Utils
	{
		public static Color? ParseColorString(string tint)
		{
			if (string.IsNullOrEmpty(tint))
				return null;

			if (int.TryParse(tint.Trim('#'), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
				return Color.FromArgb(value);

			try
			{
				return Color.FromName(tint);
			}
			catch
			{
			}

			return null;
		}

		public static Size? ParseSizeString(string size)
		{
			if (string.IsNullOrEmpty(size))
				return null;

			var parts = size.Split(new char[] { ',', ';' }, 2);

			if (parts.Length > 0 && int.TryParse(parts[0], out var width))
			{
				if (parts.Length > 1 && int.TryParse(parts[1], out var height))
					return new Size(width, height);
				else
					return new Size(width, width);
			}

			return null;
		}
	}
}
