using Xamarin.Forms;

namespace Xamarin.Platform
{
	public abstract class Brush2
	{
		public static Brush2 Default
		{
			get { return new SolidColorBrush2(Color.Default); }
		}

		public abstract bool IsEmpty { get; }

		public static bool IsNullOrEmpty(Brush2 brush)
		{
			return brush == null || brush.IsEmpty;
		}

		public static readonly SolidColorBrush2 AliceBlue = new SolidColorBrush2(Color.AliceBlue);
		public static readonly SolidColorBrush2 AntiqueWhite = new SolidColorBrush2(Color.AntiqueWhite);
		public static readonly SolidColorBrush2 Aqua = new SolidColorBrush2(Color.Aqua);
		public static readonly SolidColorBrush2 Aquamarine = new SolidColorBrush2(Color.Aquamarine);
		public static readonly SolidColorBrush2 Azure = new SolidColorBrush2(Color.Azure);
		public static readonly SolidColorBrush2 Beige = new SolidColorBrush2(Color.Beige);
		public static readonly SolidColorBrush2 Bisque = new SolidColorBrush2(Color.Bisque);
		public static readonly SolidColorBrush2 Black = new SolidColorBrush2(Color.Black);
		public static readonly SolidColorBrush2 BlanchedAlmond = new SolidColorBrush2(Color.BlanchedAlmond);
		public static readonly SolidColorBrush2 Blue = new SolidColorBrush2(Color.Blue);
		public static readonly SolidColorBrush2 BlueViolet = new SolidColorBrush2(Color.BlueViolet);
		public static readonly SolidColorBrush2 Brown = new SolidColorBrush2(Color.Brown);
		public static readonly SolidColorBrush2 BurlyWood = new SolidColorBrush2(Color.BurlyWood);
		public static readonly SolidColorBrush2 CadetBlue = new SolidColorBrush2(Color.CadetBlue);
		public static readonly SolidColorBrush2 Chartreuse = new SolidColorBrush2(Color.Chartreuse);
		public static readonly SolidColorBrush2 Chocolate = new SolidColorBrush2(Color.Chocolate);
		public static readonly SolidColorBrush2 Coral = new SolidColorBrush2(Color.Coral);
		public static readonly SolidColorBrush2 CornflowerBlue = new SolidColorBrush2(Color.CornflowerBlue);
		public static readonly SolidColorBrush2 Cornsilk = new SolidColorBrush2(Color.Cornsilk);
		public static readonly SolidColorBrush2 Crimson = new SolidColorBrush2(Color.Crimson);
		public static readonly SolidColorBrush2 Cyan = new SolidColorBrush2(Color.Cyan);
		public static readonly SolidColorBrush2 DarkBlue = new SolidColorBrush2(Color.DarkBlue);
		public static readonly SolidColorBrush2 DarkCyan = new SolidColorBrush2(Color.DarkCyan);
		public static readonly SolidColorBrush2 DarkGoldenrod = new SolidColorBrush2(Color.DarkGoldenrod);
		public static readonly SolidColorBrush2 DarkGray = new SolidColorBrush2(Color.DarkGray);
		public static readonly SolidColorBrush2 DarkGreen = new SolidColorBrush2(Color.DarkGreen);
		public static readonly SolidColorBrush2 DarkKhaki = new SolidColorBrush2(Color.DarkKhaki);
		public static readonly SolidColorBrush2 DarkMagenta = new SolidColorBrush2(Color.DarkMagenta);
		public static readonly SolidColorBrush2 DarkOliveGreen = new SolidColorBrush2(Color.DarkOliveGreen);
		public static readonly SolidColorBrush2 DarkOrange = new SolidColorBrush2(Color.DarkOrange);
		public static readonly SolidColorBrush2 DarkOrchid = new SolidColorBrush2(Color.DarkOrchid);
		public static readonly SolidColorBrush2 DarkRed = new SolidColorBrush2(Color.DarkRed);
		public static readonly SolidColorBrush2 DarkSalmon = new SolidColorBrush2(Color.DarkSalmon);
		public static readonly SolidColorBrush2 DarkSeaGreen = new SolidColorBrush2(Color.DarkSeaGreen);
		public static readonly SolidColorBrush2 DarkSlateBlue = new SolidColorBrush2(Color.DarkSlateBlue);
		public static readonly SolidColorBrush2 DarkSlateGray = new SolidColorBrush2(Color.DarkSlateGray);
		public static readonly SolidColorBrush2 DarkTurquoise = new SolidColorBrush2(Color.DarkTurquoise);
		public static readonly SolidColorBrush2 DarkViolet = new SolidColorBrush2(Color.DarkViolet);
		public static readonly SolidColorBrush2 DeepPink = new SolidColorBrush2(Color.DeepPink);
		public static readonly SolidColorBrush2 DeepSkyBlue = new SolidColorBrush2(Color.DeepSkyBlue);
		public static readonly SolidColorBrush2 DimGray = new SolidColorBrush2(Color.DimGray);
		public static readonly SolidColorBrush2 DodgerBlue = new SolidColorBrush2(Color.DodgerBlue);
		public static readonly SolidColorBrush2 Firebrick = new SolidColorBrush2(Color.Firebrick);
		public static readonly SolidColorBrush2 FloralWhite = new SolidColorBrush2(Color.FloralWhite);
		public static readonly SolidColorBrush2 ForestGreen = new SolidColorBrush2(Color.ForestGreen);
		public static readonly SolidColorBrush2 Fuchsia = new SolidColorBrush2(Color.Fuchsia);
		public static readonly SolidColorBrush2 Gainsboro = new SolidColorBrush2(Color.Gainsboro);
		public static readonly SolidColorBrush2 GhostWhite = new SolidColorBrush2(Color.GhostWhite);
		public static readonly SolidColorBrush2 Gold = new SolidColorBrush2(Color.Gold);
		public static readonly SolidColorBrush2 Goldenrod = new SolidColorBrush2(Color.Goldenrod);
		public static readonly SolidColorBrush2 Gray = new SolidColorBrush2(Color.Gray);
		public static readonly SolidColorBrush2 Green = new SolidColorBrush2(Color.Green);
		public static readonly SolidColorBrush2 GreenYellow = new SolidColorBrush2(Color.GreenYellow);
		public static readonly SolidColorBrush2 Honeydew = new SolidColorBrush2(Color.Honeydew);
		public static readonly SolidColorBrush2 HotPink = new SolidColorBrush2(Color.HotPink);
		public static readonly SolidColorBrush2 IndianRed = new SolidColorBrush2(Color.IndianRed);
		public static readonly SolidColorBrush2 Indigo = new SolidColorBrush2(Color.Indigo);
		public static readonly SolidColorBrush2 Ivory = new SolidColorBrush2(Color.Ivory);
		public static readonly SolidColorBrush2 Khaki = new SolidColorBrush2(Color.Ivory);
		public static readonly SolidColorBrush2 Lavender = new SolidColorBrush2(Color.Lavender);
		public static readonly SolidColorBrush2 LavenderBlush = new SolidColorBrush2(Color.LavenderBlush);
		public static readonly SolidColorBrush2 LawnGreen = new SolidColorBrush2(Color.LawnGreen);
		public static readonly SolidColorBrush2 LemonChiffon = new SolidColorBrush2(Color.LemonChiffon);
		public static readonly SolidColorBrush2 LightBlue = new SolidColorBrush2(Color.LightBlue);
		public static readonly SolidColorBrush2 LightCoral = new SolidColorBrush2(Color.LightCoral);
		public static readonly SolidColorBrush2 LightCyan = new SolidColorBrush2(Color.LightCyan);
		public static readonly SolidColorBrush2 LightGoldenrodYellow = new SolidColorBrush2(Color.LightGoldenrodYellow);
		public static readonly SolidColorBrush2 LightGray = new SolidColorBrush2(Color.LightGray);
		public static readonly SolidColorBrush2 LightGreen = new SolidColorBrush2(Color.LightGreen);
		public static readonly SolidColorBrush2 LightPink = new SolidColorBrush2(Color.LightPink);
		public static readonly SolidColorBrush2 LightSalmon = new SolidColorBrush2(Color.LightSalmon);
		public static readonly SolidColorBrush2 LightSeaGreen = new SolidColorBrush2(Color.LightSeaGreen);
		public static readonly SolidColorBrush2 LightSkyBlue = new SolidColorBrush2(Color.LightSkyBlue);
		public static readonly SolidColorBrush2 LightSlateGray = new SolidColorBrush2(Color.LightSlateGray);
		public static readonly SolidColorBrush2 LightSteelBlue = new SolidColorBrush2(Color.LightSteelBlue);
		public static readonly SolidColorBrush2 LightYellow = new SolidColorBrush2(Color.LightYellow);
		public static readonly SolidColorBrush2 Lime = new SolidColorBrush2(Color.Lime);
		public static readonly SolidColorBrush2 LimeGreen = new SolidColorBrush2(Color.LimeGreen);
		public static readonly SolidColorBrush2 Linen = new SolidColorBrush2(Color.Linen);
		public static readonly SolidColorBrush2 Magenta = new SolidColorBrush2(Color.Magenta);
		public static readonly SolidColorBrush2 Maroon = new SolidColorBrush2(Color.Maroon);
		public static readonly SolidColorBrush2 MediumAquamarine = new SolidColorBrush2(Color.MediumAquamarine);
		public static readonly SolidColorBrush2 MediumBlue = new SolidColorBrush2(Color.MediumBlue);
		public static readonly SolidColorBrush2 MediumOrchid = new SolidColorBrush2(Color.MediumOrchid);
		public static readonly SolidColorBrush2 MediumPurple = new SolidColorBrush2(Color.MediumPurple);
		public static readonly SolidColorBrush2 MediumSeaGreen = new SolidColorBrush2(Color.MediumSeaGreen);
		public static readonly SolidColorBrush2 MediumSlateBlue = new SolidColorBrush2(Color.MediumSlateBlue);
		public static readonly SolidColorBrush2 MediumSpringGreen = new SolidColorBrush2(Color.MediumSpringGreen);
		public static readonly SolidColorBrush2 MediumTurquoise = new SolidColorBrush2(Color.MediumTurquoise);
		public static readonly SolidColorBrush2 MediumVioletRed = new SolidColorBrush2(Color.MediumVioletRed);
		public static readonly SolidColorBrush2 MidnightBlue = new SolidColorBrush2(Color.MidnightBlue);
		public static readonly SolidColorBrush2 MintCream = new SolidColorBrush2(Color.MintCream);
		public static readonly SolidColorBrush2 MistyRose = new SolidColorBrush2(Color.MistyRose);
		public static readonly SolidColorBrush2 Moccasin = new SolidColorBrush2(Color.Moccasin);
		public static readonly SolidColorBrush2 NavajoWhite = new SolidColorBrush2(Color.NavajoWhite);
		public static readonly SolidColorBrush2 Navy = new SolidColorBrush2(Color.Navy);
		public static readonly SolidColorBrush2 OldLace = new SolidColorBrush2(Color.DarkBlue);
		public static readonly SolidColorBrush2 Olive = new SolidColorBrush2(Color.Olive);
		public static readonly SolidColorBrush2 OliveDrab = new SolidColorBrush2(Color.OliveDrab);
		public static readonly SolidColorBrush2 Orange = new SolidColorBrush2(Color.Orange);
		public static readonly SolidColorBrush2 OrangeRed = new SolidColorBrush2(Color.OrangeRed);
		public static readonly SolidColorBrush2 Orchid = new SolidColorBrush2(Color.Orchid);
		public static readonly SolidColorBrush2 PaleGoldenrod = new SolidColorBrush2(Color.PaleGoldenrod);
		public static readonly SolidColorBrush2 PaleGreen = new SolidColorBrush2(Color.MistyRose);
		public static readonly SolidColorBrush2 PaleTurquoise = new SolidColorBrush2(Color.PaleTurquoise);
		public static readonly SolidColorBrush2 PaleVioletRed = new SolidColorBrush2(Color.PaleVioletRed);
		public static readonly SolidColorBrush2 PapayaWhip = new SolidColorBrush2(Color.PapayaWhip);
		public static readonly SolidColorBrush2 PeachPuff = new SolidColorBrush2(Color.PeachPuff);
		public static readonly SolidColorBrush2 Peru = new SolidColorBrush2(Color.Peru);
		public static readonly SolidColorBrush2 Pink = new SolidColorBrush2(Color.Pink);
		public static readonly SolidColorBrush2 Plum = new SolidColorBrush2(Color.Plum);
		public static readonly SolidColorBrush2 PowderBlue = new SolidColorBrush2(Color.PowderBlue);
		public static readonly SolidColorBrush2 Purple = new SolidColorBrush2(Color.Purple);
		public static readonly SolidColorBrush2 Red = new SolidColorBrush2(Color.Red);
		public static readonly SolidColorBrush2 RosyBrown = new SolidColorBrush2(Color.RosyBrown);
		public static readonly SolidColorBrush2 RoyalBlue = new SolidColorBrush2(Color.RoyalBlue);
		public static readonly SolidColorBrush2 SaddleBrown = new SolidColorBrush2(Color.SaddleBrown);
		public static readonly SolidColorBrush2 Salmon = new SolidColorBrush2(Color.Salmon);
		public static readonly SolidColorBrush2 SandyBrown = new SolidColorBrush2(Color.SandyBrown);
		public static readonly SolidColorBrush2 SeaGreen = new SolidColorBrush2(Color.SeaGreen);
		public static readonly SolidColorBrush2 SeaShell = new SolidColorBrush2(Color.SeaShell);
		public static readonly SolidColorBrush2 Sienna = new SolidColorBrush2(Color.Sienna);
		public static readonly SolidColorBrush2 Silver = new SolidColorBrush2(Color.Silver);
		public static readonly SolidColorBrush2 SkyBlue = new SolidColorBrush2(Color.SkyBlue);
		public static readonly SolidColorBrush2 SlateBlue = new SolidColorBrush2(Color.SlateBlue);
		public static readonly SolidColorBrush2 SlateGray = new SolidColorBrush2(Color.SlateGray);
		public static readonly SolidColorBrush2 Snow = new SolidColorBrush2(Color.Snow);
		public static readonly SolidColorBrush2 SpringGreen = new SolidColorBrush2(Color.SpringGreen);
		public static readonly SolidColorBrush2 SteelBlue = new SolidColorBrush2(Color.SteelBlue);
		public static readonly SolidColorBrush2 Tan = new SolidColorBrush2(Color.Tan);
		public static readonly SolidColorBrush2 Teal = new SolidColorBrush2(Color.Teal);
		public static readonly SolidColorBrush2 Thistle = new SolidColorBrush2(Color.Thistle);
		public static readonly SolidColorBrush2 Tomato = new SolidColorBrush2(Color.Tomato);
		public static readonly SolidColorBrush2 Transparent = new SolidColorBrush2(Color.Transparent);
		public static readonly SolidColorBrush2 Turquoise = new SolidColorBrush2(Color.Turquoise);
		public static readonly SolidColorBrush2 Violet = new SolidColorBrush2(Color.Violet);
		public static readonly SolidColorBrush2 Wheat = new SolidColorBrush2(Color.Wheat);
		public static readonly SolidColorBrush2 White = new SolidColorBrush2(Color.White);
		public static readonly SolidColorBrush2 WhiteSmoke = new SolidColorBrush2(Color.WhiteSmoke);
		public static readonly SolidColorBrush2 Yellow = new SolidColorBrush2(Color.Yellow);
		public static readonly SolidColorBrush2 YellowGreen = new SolidColorBrush2(Color.YellowGreen);
	}
}