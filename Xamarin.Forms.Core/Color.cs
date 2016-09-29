using System;
using System.Diagnostics;
using System.Globalization;

namespace Xamarin.Forms
{
	[DebuggerDisplay("R={R}, G={G}, B={B}, A={A}, Hue={Hue}, Saturation={Saturation}, Luminosity={Luminosity}")]
	[TypeConverter(typeof(ColorTypeConverter))]
	public struct Color
	{
		readonly Mode _mode;

		enum Mode
		{
			Default,
			Rgb,
			Hsl
		}

		public static Color Default
		{
			get { return new Color(-1d, -1d, -1d, -1d, Mode.Default); }
		}

		internal bool IsDefault
		{
			get { return _mode == Mode.Default; }
		}

		public static Color Accent { get; internal set; }

		readonly float _a;

		public double A
		{
			get { return _a; }
		}

		readonly float _r;

		public double R
		{
			get { return _r; }
		}

		readonly float _g;

		public double G
		{
			get { return _g; }
		}

		readonly float _b;

		public double B
		{
			get { return _b; }
		}

		readonly float _hue;

		public double Hue
		{
			get { return _hue; }
		}

		readonly float _saturation;

		public double Saturation
		{
			get { return _saturation; }
		}

		readonly float _luminosity;

		public double Luminosity
		{
			get { return _luminosity; }
		}

		public Color(double r, double g, double b, double a) : this(r, g, b, a, Mode.Rgb)
		{
		}

		Color(double w, double x, double y, double z, Mode mode)
		{
			_mode = mode;
			switch (mode)
			{
				default:
				case Mode.Default:
					_r = _g = _b = _a = -1;
					_hue = _saturation = _luminosity = -1;
					break;
				case Mode.Rgb:
					_r = (float)w.Clamp(0, 1);
					_g = (float)x.Clamp(0, 1);
					_b = (float)y.Clamp(0, 1);
					_a = (float)z.Clamp(0, 1);
					ConvertToHsl(_r, _g, _b, mode, out _hue, out _saturation, out _luminosity);
					break;
				case Mode.Hsl:
					_hue = (float)w.Clamp(0, 1);
					_saturation = (float)x.Clamp(0, 1);
					_luminosity = (float)y.Clamp(0, 1);
					_a = (float)z.Clamp(0, 1);
					ConvertToRgb(_hue, _saturation, _luminosity, mode, out _r, out _g, out _b);
					break;
			}
		}

		public Color(double r, double g, double b) : this(r, g, b, 1)
		{
		}

		public Color(double value) : this(value, value, value, 1)
		{
		}

		public Color MultiplyAlpha(double alpha)
		{
			switch (_mode)
			{
				default:
				case Mode.Default:
					throw new InvalidOperationException("Invalid on Color.Default");
				case Mode.Rgb:
					return new Color(_r, _g, _b, _a * alpha, Mode.Rgb);
				case Mode.Hsl:
					return new Color(_hue, _saturation, _luminosity, _a * alpha, Mode.Hsl);
			}
		}

		public Color AddLuminosity(double delta)
		{
			if (_mode == Mode.Default)
				throw new InvalidOperationException("Invalid on Color.Default");

			return new Color(_hue, _saturation, _luminosity + delta, _a, Mode.Hsl);
		}

		public Color WithHue(double hue)
		{
			if (_mode == Mode.Default)
				throw new InvalidOperationException("Invalid on Color.Default");
			return new Color(hue, _saturation, _luminosity, _a, Mode.Hsl);
		}

		public Color WithSaturation(double saturation)
		{
			if (_mode == Mode.Default)
				throw new InvalidOperationException("Invalid on Color.Default");
			return new Color(_hue, saturation, _luminosity, _a, Mode.Hsl);
		}

		public Color WithLuminosity(double luminosity)
		{
			if (_mode == Mode.Default)
				throw new InvalidOperationException("Invalid on Color.Default");
			return new Color(_hue, _saturation, luminosity, _a, Mode.Hsl);
		}

		static void ConvertToRgb(float hue, float saturation, float luminosity, Mode mode, out float r, out float g, out float b)
		{
			if (mode != Mode.Hsl)
				throw new InvalidOperationException();

			if (luminosity == 0)
			{
				r = g = b = 0;
				return;
			}

			if (saturation == 0)
			{
				r = g = b = luminosity;
				return;
			}
			float temp2 = luminosity <= 0.5f ? luminosity * (1.0f + saturation) : luminosity + saturation - luminosity * saturation;
			float temp1 = 2.0f * luminosity - temp2;

			var t3 = new[] { hue + 1.0f / 3.0f, hue, hue - 1.0f / 3.0f };
			var clr = new float[] { 0, 0, 0 };
			for (var i = 0; i < 3; i++)
			{
				if (t3[i] < 0)
					t3[i] += 1.0f;
				if (t3[i] > 1)
					t3[i] -= 1.0f;
				if (6.0 * t3[i] < 1.0)
					clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0f;
				else if (2.0 * t3[i] < 1.0)
					clr[i] = temp2;
				else if (3.0 * t3[i] < 2.0)
					clr[i] = temp1 + (temp2 - temp1) * (2.0f / 3.0f - t3[i]) * 6.0f;
				else
					clr[i] = temp1;
			}

			r = clr[0];
			g = clr[1];
			b = clr[2];
		}

		static void ConvertToHsl(float r, float g, float b, Mode mode, out float h, out float s, out float l)
		{
			float v = Math.Max(r, g);
			v = Math.Max(v, b);

			float m = Math.Min(r, g);
			m = Math.Min(m, b);

			l = (m + v) / 2.0f;
			if (l <= 0.0)
			{
				h = s = l = 0;
				return;
			}
			float vm = v - m;
			s = vm;

			if (s > 0.0)
			{
				s /= l <= 0.5f ? v + m : 2.0f - v - m;
			}
			else
			{
				h = 0;
				s = 0;
				return;
			}

			float r2 = (v - r) / vm;
			float g2 = (v - g) / vm;
			float b2 = (v - b) / vm;

			if (r == v)
			{
				h = g == m ? 5.0f + b2 : 1.0f - g2;
			}
			else if (g == v)
			{
				h = b == m ? 1.0f + r2 : 3.0f - b2;
			}
			else
			{
				h = r == m ? 3.0f + g2 : 5.0f - r2;
			}
			h /= 6.0f;
		}

		public static bool operator ==(Color color1, Color color2)
		{
			return EqualsInner(color1, color2);
		}

		public static bool operator !=(Color color1, Color color2)
		{
			return !EqualsInner(color1, color2);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashcode = _r.GetHashCode();
				hashcode = (hashcode * 397) ^ _g.GetHashCode();
				hashcode = (hashcode * 397) ^ _b.GetHashCode();
				hashcode = (hashcode * 397) ^ _a.GetHashCode();
				return hashcode;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Color)
			{
				return EqualsInner(this, (Color)obj);
			}
			return base.Equals(obj);
		}

		static bool EqualsInner(Color color1, Color color2)
		{
			if (color1._mode == Mode.Default && color2._mode == Mode.Default)
				return true;
			if (color1._mode == Mode.Default || color2._mode == Mode.Default)
				return false;
			if (color1._mode == Mode.Hsl && color2._mode == Mode.Hsl)
				return color1._hue == color2._hue && color1._saturation == color2._saturation && color1._luminosity == color2._luminosity && color1._a == color2._a;
			return color1._r == color2._r && color1._g == color2._g && color1._b == color2._b && color1._a == color2._a;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[Color: A={0}, R={1}, G={2}, B={3}, Hue={4}, Saturation={5}, Luminosity={6}]", A, R, G, B, Hue, Saturation, Luminosity);
		}

		public static Color FromHex(string hex)
		{
			hex = hex.Replace("#", "");
			switch (hex.Length)
			{
				case 3: //#rgb => ffrrggbb
					hex = string.Format("ff{0}{1}{2}{3}{4}{5}", hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
					break;
				case 4: //#argb => aarrggbb
					hex = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", hex[0], hex[0], hex[1], hex[1], hex[2], hex[2], hex[3], hex[3]);
					break;
				case 6: //#rrggbb => ffrrggbb
					hex = string.Format("ff{0}", hex);
					break;
			}
			return FromUint(Convert.ToUInt32(hex.Replace("#", ""), 16));
		}

		public static Color FromUint(uint argb)
		{
			return FromRgba((byte)((argb & 0x00ff0000) >> 0x10), (byte)((argb & 0x0000ff00) >> 0x8), (byte)(argb & 0x000000ff), (byte)((argb & 0xff000000) >> 0x18));
		}

		public static Color FromRgba(int r, int g, int b, int a)
		{
			double red = (double)r / 255;
			double green = (double)g / 255;
			double blue = (double)b / 255;
			double alpha = (double)a / 255;
			return new Color(red, green, blue, alpha, Mode.Rgb);
		}

		public static Color FromRgb(int r, int g, int b)
		{
			return FromRgba(r, g, b, 255);
		}

		public static Color FromRgba(double r, double g, double b, double a)
		{
			return new Color(r, g, b, a);
		}

		public static Color FromRgb(double r, double g, double b)
		{
			return new Color(r, g, b, 1d, Mode.Rgb);
		}

		public static Color FromHsla(double h, double s, double l, double a = 1d)
		{
			return new Color(h, s, l, a, Mode.Hsl);
		}

		#region Color Definitions

		public static readonly Color Transparent = FromRgba(0, 0, 0, 0);
		public static readonly Color Aqua = FromRgb(0, 255, 255);
		public static readonly Color Black = FromRgb(0, 0, 0);
		public static readonly Color Blue = FromRgb(0, 0, 255);
		public static readonly Color Fuchsia = FromRgb(255, 0, 255);
		[Obsolete("Fuschia is obsolete as of version 1.3, please use the correct spelling of Fuchsia")] public static readonly Color Fuschia = FromRgb(255, 0, 255);
		public static readonly Color Gray = FromRgb(128, 128, 128);
		public static readonly Color Green = FromRgb(0, 128, 0);
		public static readonly Color Lime = FromRgb(0, 255, 0);
		public static readonly Color Maroon = FromRgb(128, 0, 0);
		public static readonly Color Navy = FromRgb(0, 0, 128);
		public static readonly Color Olive = FromRgb(128, 128, 0);
		public static readonly Color Orange = FromRgb(255, 165, 0);
		public static readonly Color Purple = FromRgb(128, 0, 128);
		public static readonly Color Pink = FromRgb(255, 102, 255);
		public static readonly Color Red = FromRgb(255, 0, 0);
		public static readonly Color Silver = FromRgb(192, 192, 192);
		public static readonly Color Teal = FromRgb(0, 128, 128);
		public static readonly Color White = FromRgb(255, 255, 255);
		public static readonly Color Yellow = FromRgb(255, 255, 0);

		// remaining colors in WPF's System.Windows.Media.Colors
		public static readonly Color AliceBlue = FromHex("#FFF0F8FF");
		public static readonly Color AntiqueWhite = FromHex("#FFFAEBD7");
		public static readonly Color Aquamarine = FromHex("#FF7FFFD4");
		public static readonly Color Azure = FromHex("#FFF0FFFF");
		public static readonly Color Beige = FromHex("#FFF5F5DC");
		public static readonly Color Bisque = FromHex("#FFFFE4C4");
		public static readonly Color BlanchedAlmond = FromHex("#FFFFEBCD");
		public static readonly Color BlueViolet = FromHex("#FF8A2BE2");
		public static readonly Color Brown = FromHex("#FFA52A2A");
		public static readonly Color BurlyWood = FromHex("#FFDEB887");
		public static readonly Color CadetBlue = FromHex("#FF5F9EA0");
		public static readonly Color Chartreuse = FromHex("#FF7FFF00");
		public static readonly Color Chocolate = FromHex("#FFD2691E");
		public static readonly Color Coral = FromHex("#FFFF7F50");
		public static readonly Color CornflowerBlue = FromHex("#FF6495ED");
		public static readonly Color Cornsilk = FromHex("#FFFFF8DC");
		public static readonly Color Crimson = FromHex("#FFDC143C");
		public static readonly Color Cyan = FromHex("#FF00FFFF");
		public static readonly Color DarkBlue = FromHex("#FF00008B");
		public static readonly Color DarkCyan = FromHex("#FF008B8B");
		public static readonly Color DarkGoldenrod = FromHex("#FFB8860B");
		public static readonly Color DarkGray = FromHex("#FFA9A9A9");
		public static readonly Color DarkGreen = FromHex("#FF006400");
		public static readonly Color DarkKhaki = FromHex("#FFBDB76B");
		public static readonly Color DarkMagenta = FromHex("#FF8B008B");
		public static readonly Color DarkOliveGreen = FromHex("#FF556B2F");
		public static readonly Color DarkOrange = FromHex("#FFFF8C00");
		public static readonly Color DarkOrchid = FromHex("#FF9932CC");
		public static readonly Color DarkRed = FromHex("#FF8B0000");
		public static readonly Color DarkSalmon = FromHex("#FFE9967A");
		public static readonly Color DarkSeaGreen = FromHex("#FF8FBC8F");
		public static readonly Color DarkSlateBlue = FromHex("#FF483D8B");
		public static readonly Color DarkSlateGray = FromHex("#FF2F4F4F");
		public static readonly Color DarkTurquoise = FromHex("#FF00CED1");
		public static readonly Color DarkViolet = FromHex("#FF9400D3");
		public static readonly Color DeepPink = FromHex("#FFFF1493");
		public static readonly Color DeepSkyBlue = FromHex("#FF00BFFF");
		public static readonly Color DimGray = FromHex("#FF696969");
		public static readonly Color DodgerBlue = FromHex("#FF1E90FF");
		public static readonly Color Firebrick = FromHex("#FFB22222");
		public static readonly Color FloralWhite = FromHex("#FFFFFAF0");
		public static readonly Color ForestGreen = FromHex("#FF228B22");
		public static readonly Color Gainsboro = FromHex("#FFDCDCDC");
		public static readonly Color GhostWhite = FromHex("#FFF8F8FF");
		public static readonly Color Gold = FromHex("#FFFFD700");
		public static readonly Color Goldenrod = FromHex("#FFDAA520");
		public static readonly Color GreenYellow = FromHex("#FFADFF2F");
		public static readonly Color Honeydew = FromHex("#FFF0FFF0");
		public static readonly Color HotPink = FromHex("#FFFF69B4");
		public static readonly Color IndianRed = FromHex("#FFCD5C5C");
		public static readonly Color Indigo = FromHex("#FF4B0082");
		public static readonly Color Ivory = FromHex("#FFFFFFF0");
		public static readonly Color Khaki = FromHex("#FFF0E68C");
		public static readonly Color Lavender = FromHex("#FFE6E6FA");
		public static readonly Color LavenderBlush = FromHex("#FFFFF0F5");
		public static readonly Color LawnGreen = FromHex("#FF7CFC00");
		public static readonly Color LemonChiffon = FromHex("#FFFFFACD");
		public static readonly Color LightBlue = FromHex("#FFADD8E6");
		public static readonly Color LightCoral = FromHex("#FFF08080");
		public static readonly Color LightCyan = FromHex("#FFE0FFFF");
		public static readonly Color LightGoldenrodYellow = FromHex("#FFFAFAD2");
		public static readonly Color LightGray = FromHex("#FFD3D3D3");
		public static readonly Color LightGreen = FromHex("#FF90EE90");
		public static readonly Color LightPink = FromHex("#FFFFB6C1");
		public static readonly Color LightSalmon = FromHex("#FFFFA07A");
		public static readonly Color LightSeaGreen = FromHex("#FF20B2AA");
		public static readonly Color LightSkyBlue = FromHex("#FF87CEFA");
		public static readonly Color LightSlateGray = FromHex("#FF778899");
		public static readonly Color LightSteelBlue = FromHex("#FFB0C4DE");
		public static readonly Color LightYellow = FromHex("#FFFFFFE0");
		public static readonly Color LimeGreen = FromHex("#FF32CD32");
		public static readonly Color Linen = FromHex("#FFFAF0E6");
		public static readonly Color Magenta = FromHex("#FFFF00FF");
		public static readonly Color MediumAquamarine = FromHex("#FF66CDAA");
		public static readonly Color MediumBlue = FromHex("#FF0000CD");
		public static readonly Color MediumOrchid = FromHex("#FFBA55D3");
		public static readonly Color MediumPurple = FromHex("#FF9370DB");
		public static readonly Color MediumSeaGreen = FromHex("#FF3CB371");
		public static readonly Color MediumSlateBlue = FromHex("#FF7B68EE");
		public static readonly Color MediumSpringGreen = FromHex("#FF00FA9A");
		public static readonly Color MediumTurquoise = FromHex("#FF48D1CC");
		public static readonly Color MediumVioletRed = FromHex("#FFC71585");
		public static readonly Color MidnightBlue = FromHex("#FF191970");
		public static readonly Color MintCream = FromHex("#FFF5FFFA");
		public static readonly Color MistyRose = FromHex("#FFFFE4E1");
		public static readonly Color Moccasin = FromHex("#FFFFE4B5");
		public static readonly Color NavajoWhite = FromHex("#FFFFDEAD");
		public static readonly Color OldLace = FromHex("#FFFDF5E6");
		public static readonly Color OliveDrab = FromHex("#FF6B8E23");
		public static readonly Color OrangeRed = FromHex("#FFFF4500");
		public static readonly Color Orchid = FromHex("#FFDA70D6");
		public static readonly Color PaleGoldenrod = FromHex("#FFEEE8AA");
		public static readonly Color PaleGreen = FromHex("#FF98FB98");
		public static readonly Color PaleTurquoise = FromHex("#FFAFEEEE");
		public static readonly Color PaleVioletRed = FromHex("#FFDB7093");
		public static readonly Color PapayaWhip = FromHex("#FFFFEFD5");
		public static readonly Color PeachPuff = FromHex("#FFFFDAB9");
		public static readonly Color Peru = FromHex("#FFCD853F");
		public static readonly Color Plum = FromHex("#FFDDA0DD");
		public static readonly Color PowderBlue = FromHex("#FFB0E0E6");
		public static readonly Color RosyBrown = FromHex("#FFBC8F8F");
		public static readonly Color RoyalBlue = FromHex("#FF4169E1");
		public static readonly Color SaddleBrown = FromHex("#FF8B4513");
		public static readonly Color Salmon = FromHex("#FFFA8072");
		public static readonly Color SandyBrown = FromHex("#FFF4A460");
		public static readonly Color SeaGreen = FromHex("#FF2E8B57");
		public static readonly Color SeaShell = FromHex("#FFFFF5EE");
		public static readonly Color Sienna = FromHex("#FFA0522D");
		public static readonly Color SkyBlue = FromHex("#FF87CEEB");
		public static readonly Color SlateBlue = FromHex("#FF6A5ACD");
		public static readonly Color SlateGray = FromHex("#FF708090");
		public static readonly Color Snow = FromHex("#FFFFFAFA");
		public static readonly Color SpringGreen = FromHex("#FF00FF7F");
		public static readonly Color SteelBlue = FromHex("#FF4682B4");
		public static readonly Color Tan = FromHex("#FFD2B48C");
		public static readonly Color Thistle = FromHex("#FFD8BFD8");
		public static readonly Color Tomato = FromHex("#FFFF6347");
		public static readonly Color Turquoise = FromHex("#FF40E0D0");
		public static readonly Color Violet = FromHex("#FFEE82EE");
		public static readonly Color Wheat = FromHex("#FFF5DEB3");
		public static readonly Color WhiteSmoke = FromHex("#FFF5F5F5");
		public static readonly Color YellowGreen = FromHex("#FF9ACD32");

		#endregion
	}
}