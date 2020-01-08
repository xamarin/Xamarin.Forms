using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms.Internals;

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

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDefault
		{
			get { return _mode == Mode.Default; }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetAccent(Color value) => Accent = value;
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

		Color(int r, int g, int b)
		{
			_mode = Mode.Rgb;
			_r = r / 255f;
			_g = g / 255f;
			_b = b / 255f;
			_a = 1;
			ConvertToHsl(_r, _g, _b, _mode, out _hue, out _saturation, out _luminosity);
		}

		Color(int r, int g, int b, int a)
		{
			_mode = Mode.Rgb;
			_r = r / 255f;
			_g = g / 255f;
			_b = b / 255f;
			_a = a / 255f;
			ConvertToHsl(_r, _g, _b, _mode, out _hue, out _saturation, out _luminosity);
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

		public string ToHex()
		{
			var red = (uint)(R * 255);
			var green = (uint)(G * 255);
			var blue = (uint)(B * 255);
			var alpha = (uint)(A * 255);
			return $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";
		}

		static uint ToHex (char c)
		{
			ushort x = (ushort)c;
			if (x >= '0' && x <= '9')
				return (uint)(x - '0');

			x |= 0x20;
			if (x >= 'a' && x <= 'f')
				return (uint)(x - 'a' + 10);
			return 0;
		} 

		static uint ToHexD (char c)
		{
			var j = ToHex (c);
			return (j << 4) | j;
		}

		public static Color FromHex (string hex)
		{
			// Undefined
			if (hex.Length < 3)
				return Default;
			int idx = (hex [0] == '#') ? 1 : 0;

			switch (hex.Length - idx) {
			case 3: //#rgb => ffrrggbb
				var t1 = ToHexD (hex [idx++]);
				var t2 = ToHexD (hex [idx++]);
				var t3 = ToHexD (hex [idx]);

				return FromRgb ((int)t1, (int)t2, (int)t3);

			case 4: //#argb => aarrggbb
				var f1 = ToHexD (hex [idx++]);
				var f2 = ToHexD (hex [idx++]);
				var f3 = ToHexD (hex [idx++]);
				var f4 = ToHexD (hex [idx]);
				return FromRgba ((int)f2, (int)f3, (int)f4, (int)f1);

			case 6: //#rrggbb => ffrrggbb
				return FromRgb ((int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx++])),
						(int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx++])),
						(int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx])));
				
			case 8: //#aarrggbb
				var a1 = ToHex (hex [idx++]) << 4 | ToHex (hex [idx++]);
				return FromRgba ((int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx++])),
						(int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx++])),
						(int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx])),
						(int)a1);
				
			default: //everything else will result in unexpected results
				return Default;
			}
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
#if !NETSTANDARD1_0
		public static implicit operator System.Drawing.Color(Color color)
		{
			if (color.IsDefault)
				return System.Drawing.Color.Empty;
			return System.Drawing.Color.FromArgb((byte)(color._a * 255), (byte)(color._r * 255), (byte)(color._g * 255), (byte)(color._b * 255));
		}

		public static implicit operator Color(System.Drawing.Color color)
		{
			if (color.IsEmpty)
				return Color.Default;
			return FromRgba(color.R, color.G, color.B, color.A);
		}
#endif
		#region Color Definitions

		// matches colors in WPF's System.Windows.Media.Colors
		public static Color AliceBlue => aliceBlue ??= new Color(240, 248, 255);
		public static Color AntiqueWhite => antiqueWhite ??= new Color(250, 235, 215);
		public static Color Aqua => aqua ??= new Color(0, 255, 255);
		public static Color Aquamarine => aquamarine ??= new Color(127, 255, 212);
		public static Color Azure => azure ??= new Color(240, 255, 255);
		public static Color Beige => beige ??= new Color(245, 245, 220);
		public static Color Bisque => bisque ??= new Color(255, 228, 196);
		public static Color Black => black ??= new Color(0, 0, 0);
		public static Color BlanchedAlmond => blanchedAlmond ??= new Color(255, 235, 205);
		public static Color Blue => blue ??= new Color(0, 0, 255);
		public static Color BlueViolet => blueViolet ??= new Color(138, 43, 226);
		public static Color Brown => brown ??= new Color(165, 42, 42);
		public static Color BurlyWood => burlyWood ??= new Color(222, 184, 135);
		public static Color CadetBlue => cadetBlue ??= new Color(95, 158, 160);
		public static Color Chartreuse => chartreuse ??= new Color(127, 255, 0);
		public static Color Chocolate => chocolate ??= new Color(210, 105, 30);
		public static Color Coral => coral ??= new Color(255, 127, 80);
		public static Color CornflowerBlue => cornflowerBlue ??= new Color(100, 149, 237);
		public static Color Cornsilk => cornsilk ??= new Color(255, 248, 220);
		public static Color Crimson => crimson ??= new Color(220, 20, 60);
		public static Color Cyan => cyan ??= new Color(0, 255, 255);
		public static Color DarkBlue => darkBlue ??= new Color(0, 0, 139);
		public static Color DarkCyan => darkCyan ??= new Color(0, 139, 139);
		public static Color DarkGoldenrod => darkGoldenrod ??= new Color(184, 134, 11);
		public static Color DarkGray => darkGray ??= new Color(169, 169, 169);
		public static Color DarkGreen => darkGreen ??= new Color(0, 100, 0);
		public static Color DarkKhaki => darkKhaki ??= new Color(189, 183, 107);
		public static Color DarkMagenta => darkMagenta ??= new Color(139, 0, 139);
		public static Color DarkOliveGreen => darkOliveGreen ??= new Color(85, 107, 47);
		public static Color DarkOrange => darkOrange ??= new Color(255, 140, 0);
		public static Color DarkOrchid => darkOrchid ??= new Color(153, 50, 204);
		public static Color DarkRed => darkRed ??= new Color(139, 0, 0);
		public static Color DarkSalmon => darkSalmon ??= new Color(233, 150, 122);
		public static Color DarkSeaGreen => darkSeaGreen ??= new Color(143, 188, 143);
		public static Color DarkSlateBlue => darkSlateBlue ??= new Color(72, 61, 139);
		public static Color DarkSlateGray => darkSlateGray ??= new Color(47, 79, 79);
		public static Color DarkTurquoise => darkTurquoise ??= new Color(0, 206, 209);
		public static Color DarkViolet => darkViolet ??= new Color(148, 0, 211);
		public static Color DeepPink => deepPink ??= new Color(255, 20, 147);
		public static Color DeepSkyBlue => deepSkyBlue ??= new Color(0, 191, 255);
		public static Color DimGray => dimGray ??= new Color(105, 105, 105);
		public static Color DodgerBlue => dodgerBlue ??= new Color(30, 144, 255);
		public static Color Firebrick => firebrick ??= new Color(178, 34, 34);
		public static Color FloralWhite => floralWhite ??= new Color(255, 250, 240);
		public static Color ForestGreen => forestGreen ??= new Color(34, 139, 34);
		public static Color Fuchsia => fuchsia ??= new Color(255, 0, 255);
		[Obsolete("Fuschia is obsolete as of version 1.3.0. Please use Fuchsia instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Color Fuschia => fuschia ??= new Color(255, 0, 255);
		public static Color Gainsboro => gainsboro ??= new Color(220, 220, 220);
		public static Color GhostWhite => ghostWhite ??= new Color(248, 248, 255);
		public static Color Gold => gold ??= new Color(255, 215, 0);
		public static Color Goldenrod => goldenrod ??= new Color(218, 165, 32);
		public static Color Gray => gray ??= new Color(128, 128, 128);
		public static Color Green => green ??= new Color(0, 128, 0);
		public static Color GreenYellow => greenYellow ??= new Color(173, 255, 47);
		public static Color Honeydew => honeydew ??= new Color(240, 255, 240);
		public static Color HotPink => hotPink ??= new Color(255, 105, 180);
		public static Color IndianRed => indianRed ??= new Color(205, 92, 92);
		public static Color Indigo => indigo ??= new Color(75, 0, 130);
		public static Color Ivory => ivory ??= new Color(255, 255, 240);
		public static Color Khaki => khaki ??= new Color(240, 230, 140);
		public static Color Lavender => lavender ??= new Color(230, 230, 250);
		public static Color LavenderBlush => lavenderBlush ??= new Color(255, 240, 245);
		public static Color LawnGreen => lawnGreen ??= new Color(124, 252, 0);
		public static Color LemonChiffon => lemonChiffon ??= new Color(255, 250, 205);
		public static Color LightBlue => lightBlue ??= new Color(173, 216, 230);
		public static Color LightCoral => lightCoral ??= new Color(240, 128, 128);
		public static Color LightCyan => lightCyan ??= new Color(224, 255, 255);
		public static Color LightGoldenrodYellow => lightGoldenrodYellow ??= new Color(250, 250, 210);
		public static Color LightGray => lightGray ??= new Color(211, 211, 211);
		public static Color LightGreen => lightGreen ??= new Color(144, 238, 144);
		public static Color LightPink => lightPink ??= new Color(255, 182, 193);
		public static Color LightSalmon => lightSalmon ??= new Color(255, 160, 122);
		public static Color LightSeaGreen => lightSeaGreen ??= new Color(32, 178, 170);
		public static Color LightSkyBlue => lightSkyBlue ??= new Color(135, 206, 250);
		public static Color LightSlateGray => lightSlateGray ??= new Color(119, 136, 153);
		public static Color LightSteelBlue => lightSteelBlue ??= new Color(176, 196, 222);
		public static Color LightYellow => lightYellow ??= new Color(255, 255, 224);
		public static Color Lime => lime ??= new Color(0, 255, 0);
		public static Color LimeGreen => limeGreen ??= new Color(50, 205, 50);
		public static Color Linen => linen ??= new Color(250, 240, 230);
		public static Color Magenta => magenta ??= new Color(255, 0, 255);
		public static Color Maroon => maroon ??= new Color(128, 0, 0);
		public static Color MediumAquamarine => mediumAquamarine ??= new Color(102, 205, 170);
		public static Color MediumBlue => mediumBlue ??= new Color(0, 0, 205);
		public static Color MediumOrchid => mediumOrchid ??= new Color(186, 85, 211);
		public static Color MediumPurple => mediumPurple ??= new Color(147, 112, 219);
		public static Color MediumSeaGreen => mediumSeaGreen ??= new Color(60, 179, 113);
		public static Color MediumSlateBlue => mediumSlateBlue ??= new Color(123, 104, 238);
		public static Color MediumSpringGreen => mediumSpringGreen ??= new Color(0, 250, 154);
		public static Color MediumTurquoise => mediumTurquoise ??= new Color(72, 209, 204);
		public static Color MediumVioletRed => mediumVioletRed ??= new Color(199, 21, 133);
		public static Color MidnightBlue => midnightBlue ??= new Color(25, 25, 112);
		public static Color MintCream => mintCream ??= new Color(245, 255, 250);
		public static Color MistyRose => mistyRose ??= new Color(255, 228, 225);
		public static Color Moccasin => moccasin ??= new Color(255, 228, 181);
		public static Color NavajoWhite => navajoWhite ??= new Color(255, 222, 173);
		public static Color Navy => navy ??= new Color(0, 0, 128);
		public static Color OldLace => oldLace ??= new Color(253, 245, 230);
		public static Color Olive => olive ??= new Color(128, 128, 0);
		public static Color OliveDrab => oliveDrab ??= new Color(107, 142, 35);
		public static Color Orange => orange ??= new Color(255, 165, 0);
		public static Color OrangeRed => orangeRed ??= new Color(255, 69, 0);
		public static Color Orchid => orchid ??= new Color(218, 112, 214);
		public static Color PaleGoldenrod => paleGoldenrod ??= new Color(238, 232, 170);
		public static Color PaleGreen => paleGreen ??= new Color(152, 251, 152);
		public static Color PaleTurquoise => paleTurquoise ??= new Color(175, 238, 238);
		public static Color PaleVioletRed => paleVioletRed ??= new Color(219, 112, 147);
		public static Color PapayaWhip => papayaWhip ??= new Color(255, 239, 213);
		public static Color PeachPuff => peachPuff ??= new Color(255, 218, 185);
		public static Color Peru => peru ??= new Color(205, 133, 63);
		public static Color Pink => pink ??= new Color(255, 192, 203);
		public static Color Plum => plum ??= new Color(221, 160, 221);
		public static Color PowderBlue => powderBlue ??= new Color(176, 224, 230);
		public static Color Purple => purple ??= new Color(128, 0, 128);
		public static Color Red => red ??= new Color(255, 0, 0);
		public static Color RosyBrown => rosyBrown ??= new Color(188, 143, 143);
		public static Color RoyalBlue => royalBlue ??= new Color(65, 105, 225);
		public static Color SaddleBrown => saddleBrown ??= new Color(139, 69, 19);
		public static Color Salmon => salmon ??= new Color(250, 128, 114);
		public static Color SandyBrown => sandyBrown ??= new Color(244, 164, 96);
		public static Color SeaGreen => seaGreen ??= new Color(46, 139, 87);
		public static Color SeaShell => seaShell ??= new Color(255, 245, 238);
		public static Color Sienna => sienna ??= new Color(160, 82, 45);
		public static Color Silver => silver ??= new Color(192, 192, 192);
		public static Color SkyBlue => skyBlue ??= new Color(135, 206, 235);
		public static Color SlateBlue => slateBlue ??= new Color(106, 90, 205);
		public static Color SlateGray => slateGray ??= new Color(112, 128, 144);
		public static Color Snow => snow ??= new Color(255, 250, 250);
		public static Color SpringGreen => springGreen ??= new Color(0, 255, 127);
		public static Color SteelBlue => steelBlue ??= new Color(70, 130, 180);
		public static Color Tan => tan ??= new Color(210, 180, 140);
		public static Color Teal => teal ??= new Color(0, 128, 128);
		public static Color Thistle => thistle ??= new Color(216, 191, 216);
		public static Color Tomato => tomato ??= new Color(255, 99, 71);
		public static Color Transparent => transparent ??= new Color(255, 255, 255, 0);
		public static Color Turquoise => turquoise ??= new Color(64, 224, 208);
		public static Color Violet => violet ??= new Color(238, 130, 238);
		public static Color Wheat => wheat ??= new Color(245, 222, 179);
		public static Color White => white ??= new Color(255, 255, 255);
		public static Color WhiteSmoke => whiteSmoke ??= new Color(245, 245, 245);
		public static Color Yellow => yellow ??= new Color(255, 255, 0);
		public static Color YellowGreen => yellowGreen ??= new Color(154, 205, 50);

		public static Color? aliceBlue;
		public static Color? antiqueWhite;
		public static Color? aqua;
		public static Color? aquamarine;
		public static Color? azure;
		public static Color? beige;
		public static Color? bisque;
		public static Color? black;
		public static Color? blanchedAlmond;
		public static Color? blue;
		public static Color? blueViolet;
		public static Color? brown;
		public static Color? burlyWood;
		public static Color? cadetBlue;
		public static Color? chartreuse;
		public static Color? chocolate;
		public static Color? coral;
		public static Color? cornflowerBlue;
		public static Color? cornsilk;
		public static Color? crimson;
		public static Color? cyan;
		public static Color? darkBlue;
		public static Color? darkCyan;
		public static Color? darkGoldenrod;
		public static Color? darkGray;
		public static Color? darkGreen;
		public static Color? darkKhaki;
		public static Color? darkMagenta;
		public static Color? darkOliveGreen;
		public static Color? darkOrange;
		public static Color? darkOrchid;
		public static Color? darkRed;
		public static Color? darkSalmon;
		public static Color? darkSeaGreen;
		public static Color? darkSlateBlue;
		public static Color? darkSlateGray;
		public static Color? darkTurquoise;
		public static Color? darkViolet;
		public static Color? deepPink;
		public static Color? deepSkyBlue;
		public static Color? dimGray;
		public static Color? dodgerBlue;
		public static Color? firebrick;
		public static Color? floralWhite;
		public static Color? forestGreen;
		public static Color? fuchsia;
		public static Color? fuschia;
		public static Color? gainsboro;
		public static Color? ghostWhite;
		public static Color? gold;
		public static Color? goldenrod;
		public static Color? gray;
		public static Color? green;
		public static Color? greenYellow;
		public static Color? honeydew;
		public static Color? hotPink;
		public static Color? indianRed;
		public static Color? indigo;
		public static Color? ivory;
		public static Color? khaki;
		public static Color? lavender;
		public static Color? lavenderBlush;
		public static Color? lawnGreen;
		public static Color? lemonChiffon;
		public static Color? lightBlue;
		public static Color? lightCoral;
		public static Color? lightCyan;
		public static Color? lightGoldenrodYellow;
		public static Color? lightGray;
		public static Color? lightGreen;
		public static Color? lightPink;
		public static Color? lightSalmon;
		public static Color? lightSeaGreen;
		public static Color? lightSkyBlue;
		public static Color? lightSlateGray;
		public static Color? lightSteelBlue;
		public static Color? lightYellow;
		public static Color? lime;
		public static Color? limeGreen;
		public static Color? linen;
		public static Color? magenta;
		public static Color? maroon;
		public static Color? mediumAquamarine;
		public static Color? mediumBlue;
		public static Color? mediumOrchid;
		public static Color? mediumPurple;
		public static Color? mediumSeaGreen;
		public static Color? mediumSlateBlue;
		public static Color? mediumSpringGreen;
		public static Color? mediumTurquoise;
		public static Color? mediumVioletRed;
		public static Color? midnightBlue;
		public static Color? mintCream;
		public static Color? mistyRose;
		public static Color? moccasin;
		public static Color? navajoWhite;
		public static Color? navy;
		public static Color? oldLace;
		public static Color? olive;
		public static Color? oliveDrab;
		public static Color? orange;
		public static Color? orangeRed;
		public static Color? orchid;
		public static Color? paleGoldenrod;
		public static Color? paleGreen;
		public static Color? paleTurquoise;
		public static Color? paleVioletRed;
		public static Color? papayaWhip;
		public static Color? peachPuff;
		public static Color? peru;
		public static Color? pink;
		public static Color? plum;
		public static Color? powderBlue;
		public static Color? purple;
		public static Color? red;
		public static Color? rosyBrown;
		public static Color? royalBlue;
		public static Color? saddleBrown;
		public static Color? salmon;
		public static Color? sandyBrown;
		public static Color? seaGreen;
		public static Color? seaShell;
		public static Color? sienna;
		public static Color? silver;
		public static Color? skyBlue;
		public static Color? slateBlue;
		public static Color? slateGray;
		public static Color? snow;
		public static Color? springGreen;
		public static Color? steelBlue;
		public static Color? tan;
		public static Color? teal;
		public static Color? thistle;
		public static Color? tomato;
		public static Color? transparent;
		public static Color? turquoise;
		public static Color? violet;
		public static Color? wheat;
		public static Color? white;
		public static Color? whiteSmoke;
		public static Color? yellow;
		public static Color? yellowGreen;
		#endregion
	}
}
