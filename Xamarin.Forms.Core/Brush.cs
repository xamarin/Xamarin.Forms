using System;

namespace Xamarin.Forms
{
	internal class ImmutableBrush : SolidColorBrush
	{
		static readonly BindablePropertyKey ColorPropertyKey =
			BindableProperty.CreateReadOnly(nameof(Color), typeof(Color), typeof(ImmutableBrush), null);

		public new static readonly BindableProperty ColorProperty = ColorPropertyKey.BindableProperty;

		public ImmutableBrush(Color color)
		{
			SetValue(ColorPropertyKey, color);
		}

		public override Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set { }
		}
	}

	[TypeConverter(typeof(BrushTypeConverter))]
	public abstract partial class Brush : Element
	{
		static ImmutableBrush defaultBrush;
		/// <include file="../../docs/Microsoft.Maui.Controls/Brush.xml" path="//Member[@MemberName='Default']/Docs/*" />
		public static Brush Default => defaultBrush ??= new ImmutableBrush(Color.Default);

		public static implicit operator Brush(Color color) => new SolidColorBrush(color);

		/// <include file="../../docs/Microsoft.Maui.Controls/Brush.xml" path="//Member[@MemberName='IsEmpty']/Docs/*" />
		public abstract bool IsEmpty { get; }

		/// <include file="../../docs/Microsoft.Maui.Controls/Brush.xml" path="//Member[@MemberName='IsNullOrEmpty']/Docs/*" />
		public static bool IsNullOrEmpty(Brush brush)
		{
			return brush == null || brush.IsEmpty;
		}

		static ImmutableBrush aliceBlue;
		public static SolidColorBrush AliceBlue => aliceBlue ??= new ImmutableBrush(Color.AliceBlue);

		static ImmutableBrush antiqueWhite;
		public static SolidColorBrush AntiqueWhite => antiqueWhite ??= new ImmutableBrush(Color.AntiqueWhite);

		static ImmutableBrush aqua;
		public static SolidColorBrush Aqua => aqua ??= new ImmutableBrush(Color.Aqua);

		static ImmutableBrush aquamarine;
		public static SolidColorBrush Aquamarine => aquamarine ??= new ImmutableBrush(Color.Aquamarine);

		static ImmutableBrush azure;
		public static SolidColorBrush Azure => azure ??= new ImmutableBrush(Color.Azure);

		static ImmutableBrush beige;
		public static SolidColorBrush Beige => beige ??= new ImmutableBrush(Color.Beige);

		static ImmutableBrush bisque;
		public static SolidColorBrush Bisque => bisque ??= new ImmutableBrush(Color.Bisque);

		static ImmutableBrush black;
		public static SolidColorBrush Black => black ??= new ImmutableBrush(Color.Black);

		static ImmutableBrush blanchedAlmond;
		public static SolidColorBrush BlanchedAlmond => blanchedAlmond ??= new ImmutableBrush(Color.BlanchedAlmond);

		static ImmutableBrush blue;
		public static SolidColorBrush Blue => blue ??= new ImmutableBrush(Color.Blue);

		static ImmutableBrush blueViolet;
		public static SolidColorBrush BlueViolet => blueViolet ??= new ImmutableBrush(Color.BlueViolet);

		static ImmutableBrush brown;
		public static SolidColorBrush Brown => brown ??= new ImmutableBrush(Color.Brown);

		static ImmutableBrush burlyWood;
		public static SolidColorBrush BurlyWood => burlyWood ??= new ImmutableBrush(Color.BurlyWood);

		static ImmutableBrush cadetBlue;
		public static SolidColorBrush CadetBlue => cadetBlue ??= new ImmutableBrush(Color.CadetBlue);

		static ImmutableBrush chartreuse;
		public static SolidColorBrush Chartreuse => chartreuse ??= new ImmutableBrush(Color.Chartreuse);

		static ImmutableBrush chocolate;
		public static SolidColorBrush Chocolate => chocolate ??= new ImmutableBrush(Color.Chocolate);

		static ImmutableBrush coral;
		public static SolidColorBrush Coral => coral ??= new ImmutableBrush(Color.Coral);

		static ImmutableBrush cornflowerBlue;
		public static SolidColorBrush CornflowerBlue => cornflowerBlue ??= new ImmutableBrush(Color.CornflowerBlue);

		static ImmutableBrush cornsilk;
		public static SolidColorBrush Cornsilk => cornsilk ??= new ImmutableBrush(Color.Cornsilk);

		static ImmutableBrush crimson;
		public static SolidColorBrush Crimson => crimson ??= new ImmutableBrush(Color.Crimson);

		static ImmutableBrush cyan;
		public static SolidColorBrush Cyan => cyan ??= new ImmutableBrush(Color.Cyan);

		static ImmutableBrush darkBlue;
		public static SolidColorBrush DarkBlue => darkBlue ??= new ImmutableBrush(Color.DarkBlue);

		static ImmutableBrush darkCyan;
		public static SolidColorBrush DarkCyan => darkCyan ??= new ImmutableBrush(Color.DarkCyan);

		static ImmutableBrush darkGoldenrod;
		public static SolidColorBrush DarkGoldenrod => darkGoldenrod ??= new ImmutableBrush(Color.DarkGoldenrod);

		static ImmutableBrush darkGray;
		public static SolidColorBrush DarkGray => darkGray ??= new ImmutableBrush(Color.DarkGray);

		static ImmutableBrush darkGreen;
		public static SolidColorBrush DarkGreen => darkGreen ??= new ImmutableBrush(Color.DarkGreen);

		static ImmutableBrush darkKhaki;
		public static SolidColorBrush DarkKhaki => darkKhaki ??= new ImmutableBrush(Color.DarkKhaki);

		static ImmutableBrush darkMagenta;
		public static SolidColorBrush DarkMagenta => darkMagenta ??= new ImmutableBrush(Color.DarkMagenta);

		static ImmutableBrush darkOliveGreen;
		public static SolidColorBrush DarkOliveGreen => darkOliveGreen ??= new ImmutableBrush(Color.DarkOliveGreen);

		static ImmutableBrush darkOrange;
		public static SolidColorBrush DarkOrange => darkOrange ??= new ImmutableBrush(Color.DarkOrange);

		static ImmutableBrush darkOrchid;
		public static SolidColorBrush DarkOrchid => darkOrchid ??= new ImmutableBrush(Color.DarkOrchid);

		static ImmutableBrush darkRed;
		public static SolidColorBrush DarkRed => darkRed ??= new ImmutableBrush(Color.DarkRed);

		static ImmutableBrush darkSalmon;
		public static SolidColorBrush DarkSalmon => darkSalmon ??= new ImmutableBrush(Color.DarkSalmon);

		static ImmutableBrush darkSeaGreen;
		public static SolidColorBrush DarkSeaGreen => darkSeaGreen ??= new ImmutableBrush(Color.DarkSeaGreen);

		static ImmutableBrush darkSlateBlue;
		public static SolidColorBrush DarkSlateBlue => darkSlateBlue ??= new ImmutableBrush(Color.DarkSlateBlue);

		static ImmutableBrush darkSlateGray;
		public static SolidColorBrush DarkSlateGray => darkSlateGray ??= new ImmutableBrush(Color.DarkSlateGray);

		static ImmutableBrush darkTurquoise;
		public static SolidColorBrush DarkTurquoise => darkTurquoise ??= new ImmutableBrush(Color.DarkTurquoise);

		static ImmutableBrush darkViolet;
		public static SolidColorBrush DarkViolet => darkViolet ??= new ImmutableBrush(Color.DarkViolet);

		static ImmutableBrush deepPink;
		public static SolidColorBrush DeepPink => deepPink ??= new ImmutableBrush(Color.DeepPink);

		static ImmutableBrush deepSkyBlue;
		public static SolidColorBrush DeepSkyBlue => deepSkyBlue ??= new ImmutableBrush(Color.DeepSkyBlue);

		static ImmutableBrush dimGray;
		public static SolidColorBrush DimGray => dimGray ??= new ImmutableBrush(Color.DimGray);

		static ImmutableBrush dodgerBlue;
		public static SolidColorBrush DodgerBlue => dodgerBlue ??= new ImmutableBrush(Color.DodgerBlue);

		static ImmutableBrush firebrick;
		public static SolidColorBrush Firebrick => firebrick ??= new ImmutableBrush(Color.Firebrick);

		static ImmutableBrush floralWhite;
		public static SolidColorBrush FloralWhite => floralWhite ??= new ImmutableBrush(Color.FloralWhite);

		static ImmutableBrush forestGreen;
		public static SolidColorBrush ForestGreen => forestGreen ??= new ImmutableBrush(Color.ForestGreen);

		static ImmutableBrush fuschia;
		public static SolidColorBrush Fuchsia => fuschia ??= new ImmutableBrush(Color.Fuchsia);

		static ImmutableBrush gainsboro;
		public static SolidColorBrush Gainsboro => gainsboro ??= new ImmutableBrush(Color.Gainsboro);

		static ImmutableBrush ghostWhite;
		public static SolidColorBrush GhostWhite => ghostWhite ??= new ImmutableBrush(Color.GhostWhite);

		static ImmutableBrush gold;
		public static SolidColorBrush Gold => gold ??= new ImmutableBrush(Color.Gold);

		static ImmutableBrush goldenrod;
		public static SolidColorBrush Goldenrod => goldenrod ??= new ImmutableBrush(Color.Goldenrod);

		static ImmutableBrush gray;
		public static SolidColorBrush Gray => gray ??= new ImmutableBrush(Color.Gray);

		static ImmutableBrush green;
		public static SolidColorBrush Green => green ??= new ImmutableBrush(Color.Green);

		static ImmutableBrush greenYellow;
		public static SolidColorBrush GreenYellow => greenYellow ??= new ImmutableBrush(Color.GreenYellow);

		static ImmutableBrush honeydew;
		public static SolidColorBrush Honeydew => honeydew ??= new ImmutableBrush(Color.Honeydew);

		static ImmutableBrush hotPink;
		public static SolidColorBrush HotPink => hotPink ??= new ImmutableBrush(Color.HotPink);

		static ImmutableBrush indianRed;
		public static SolidColorBrush IndianRed => indianRed ??= new ImmutableBrush(Color.IndianRed);

		static ImmutableBrush indigo;
		public static SolidColorBrush Indigo => indigo ??= new ImmutableBrush(Color.Indigo);

		static ImmutableBrush ivory;
		public static SolidColorBrush Ivory => ivory ??= new ImmutableBrush(Color.Ivory);

		static ImmutableBrush khaki;
		public static SolidColorBrush Khaki => khaki ??= new ImmutableBrush(Color.Khaki);

		static ImmutableBrush lavender;
		public static SolidColorBrush Lavender => lavender ??= new ImmutableBrush(Color.Lavender);

		static ImmutableBrush lavenderBlush;
		public static SolidColorBrush LavenderBlush => lavenderBlush ??= new ImmutableBrush(Color.LavenderBlush);

		static ImmutableBrush lawnGreen;
		public static SolidColorBrush LawnGreen => lawnGreen ??= new ImmutableBrush(Color.LawnGreen);

		static ImmutableBrush lemonChiffon;
		public static SolidColorBrush LemonChiffon => lemonChiffon ??= new ImmutableBrush(Color.LemonChiffon);

		static ImmutableBrush lightBlue;
		public static SolidColorBrush LightBlue => lightBlue ??= new ImmutableBrush(Color.LightBlue);

		static ImmutableBrush lightCoral;
		public static SolidColorBrush LightCoral => lightCoral ??= new ImmutableBrush(Color.LightCoral);

		static ImmutableBrush lightCyan;
		public static SolidColorBrush LightCyan => lightCyan ??= new ImmutableBrush(Color.LightCyan);

		static ImmutableBrush lightGoldenrodYellow;
		public static SolidColorBrush LightGoldenrodYellow => lightGoldenrodYellow ??= new ImmutableBrush(Color.LightGoldenrodYellow);

		static ImmutableBrush lightGray;
		public static SolidColorBrush LightGray => lightGray ??= new ImmutableBrush(Color.LightGray);

		static ImmutableBrush lightGreen;
		public static SolidColorBrush LightGreen => lightGreen ??= new ImmutableBrush(Color.LightGreen);

		static ImmutableBrush lightPink;
		public static SolidColorBrush LightPink => lightPink ??= new ImmutableBrush(Color.LightPink);

		static ImmutableBrush lightSalmon;
		public static SolidColorBrush LightSalmon => lightSalmon ??= new ImmutableBrush(Color.LightSalmon);

		static ImmutableBrush lightSeaGreen;
		public static SolidColorBrush LightSeaGreen => lightSeaGreen ??= new ImmutableBrush(Color.LightSeaGreen);

		static ImmutableBrush lightSkyBlue;
		public static SolidColorBrush LightSkyBlue => lightSkyBlue ??= new ImmutableBrush(Color.LightSkyBlue);

		static ImmutableBrush lightSlateGray;
		public static SolidColorBrush LightSlateGray => lightSlateGray ??= new ImmutableBrush(Color.LightSlateGray);

		static ImmutableBrush lightSteelBlue;
		public static SolidColorBrush LightSteelBlue => lightSteelBlue ??= new ImmutableBrush(Color.LightSteelBlue);

		static ImmutableBrush lightYellow;
		public static SolidColorBrush LightYellow => lightYellow ??= new ImmutableBrush(Color.LightYellow);

		static ImmutableBrush lime;
		public static SolidColorBrush Lime => lime ??= new ImmutableBrush(Color.Lime);

		static ImmutableBrush limeGreen;
		public static SolidColorBrush LimeGreen => limeGreen ??= new ImmutableBrush(Color.LimeGreen);

		static ImmutableBrush linen;		 
		public static SolidColorBrush Linen => linen ??= new ImmutableBrush(Color.Linen);

		static ImmutableBrush magenta;
		public static SolidColorBrush Magenta => magenta ??= new ImmutableBrush(Color.Magenta);

		static ImmutableBrush maroon;
		public static SolidColorBrush Maroon => maroon ??= new ImmutableBrush(Color.Maroon);

		static ImmutableBrush mediumAquararine;
		public static SolidColorBrush MediumAquamarine => mediumAquararine ??= new ImmutableBrush(Color.MediumAquamarine);

		static ImmutableBrush mediumBlue;
		public static SolidColorBrush MediumBlue => mediumBlue ??= new ImmutableBrush(Color.MediumBlue);

		static ImmutableBrush mediumOrchid;
		public static SolidColorBrush MediumOrchid => mediumOrchid ??= new ImmutableBrush(Color.MediumOrchid);

		static ImmutableBrush mediumPurple;
		public static SolidColorBrush MediumPurple => mediumPurple ??= new ImmutableBrush(Color.MediumPurple);

		static ImmutableBrush mediumSeaGreen;
		public static SolidColorBrush MediumSeaGreen => mediumSeaGreen ??= new ImmutableBrush(Color.MediumSeaGreen);

		static ImmutableBrush mediumSlateBlue;
		public static SolidColorBrush MediumSlateBlue => mediumSlateBlue ??= new ImmutableBrush(Color.MediumSlateBlue);

		static ImmutableBrush mediumSpringGreen;
		public static SolidColorBrush MediumSpringGreen => mediumSpringGreen ??= new ImmutableBrush(Color.MediumSpringGreen);

		static ImmutableBrush mediumTurquoise;	
		public static SolidColorBrush MediumTurquoise => mediumTurquoise ??= new ImmutableBrush(Color.MediumTurquoise);

		static ImmutableBrush mediumVioletRed;
		public static SolidColorBrush MediumVioletRed => mediumVioletRed ??= new ImmutableBrush(Color.MediumVioletRed);

		static ImmutableBrush midnightBlue;
		public static SolidColorBrush MidnightBlue => midnightBlue ??= new ImmutableBrush(Color.MidnightBlue);

		static ImmutableBrush mintCream;
		public static SolidColorBrush MintCream => mintCream ??= new ImmutableBrush(Color.MintCream);

		static ImmutableBrush mistyRose;
		public static SolidColorBrush MistyRose => mistyRose ??= new ImmutableBrush(Color.MistyRose);

		static ImmutableBrush moccasin;
		public static SolidColorBrush Moccasin => moccasin ??= new ImmutableBrush(Color.Moccasin);

		static ImmutableBrush navajoWhite;
		public static SolidColorBrush NavajoWhite => navajoWhite ??= new ImmutableBrush(Color.NavajoWhite);

		static ImmutableBrush navy;
		public static SolidColorBrush Navy => navy ??= new ImmutableBrush(Color.Navy);

		static ImmutableBrush oldLace;
		public static SolidColorBrush OldLace => oldLace ??= new ImmutableBrush(Color.OldLace);

		static ImmutableBrush olive;
		public static SolidColorBrush Olive => olive ??= new ImmutableBrush(Color.Olive);

		static ImmutableBrush oliveDrab;
		public static SolidColorBrush OliveDrab => oliveDrab ??= new ImmutableBrush(Color.OliveDrab);

		static ImmutableBrush orange;
		public static SolidColorBrush Orange => orange ??= new ImmutableBrush(Color.Orange);

		static ImmutableBrush orangeRed;
		public static SolidColorBrush OrangeRed => orangeRed ??= new ImmutableBrush(Color.OrangeRed);

		static ImmutableBrush orchid;
		public static SolidColorBrush Orchid => orchid ??= new ImmutableBrush(Color.Orchid);

		static ImmutableBrush paleGoldenrod;
		public static SolidColorBrush PaleGoldenrod => paleGoldenrod ??= new ImmutableBrush(Color.PaleGoldenrod);

		static ImmutableBrush paleGreen;
		public static SolidColorBrush PaleGreen => paleGreen ??= new ImmutableBrush(Color.PaleGreen);

		static ImmutableBrush paleTurquoise;
		public static SolidColorBrush PaleTurquoise => paleTurquoise ??= new ImmutableBrush(Color.PaleTurquoise);

		static ImmutableBrush paleVioletRed;
		public static SolidColorBrush PaleVioletRed => paleVioletRed ??= new ImmutableBrush(Color.PaleVioletRed);

		static ImmutableBrush papayaWhip;
		public static SolidColorBrush PapayaWhip => papayaWhip ??= new ImmutableBrush(Color.PapayaWhip);

		static ImmutableBrush peachPuff;
		public static SolidColorBrush PeachPuff => peachPuff ??= new ImmutableBrush(Color.PeachPuff);

		static ImmutableBrush peru;
		public static SolidColorBrush Peru => peru ??= new ImmutableBrush(Color.Peru);

		static ImmutableBrush pink;
		public static SolidColorBrush Pink => pink ??= new ImmutableBrush(Color.Pink);

		static ImmutableBrush plum;
		public static SolidColorBrush Plum => plum ??= new ImmutableBrush(Color.Plum);

		static ImmutableBrush powderBlue;
		public static SolidColorBrush PowderBlue => powderBlue ??= new ImmutableBrush(Color.PowderBlue);

		static ImmutableBrush purple;
		public static SolidColorBrush Purple => purple ??= new ImmutableBrush(Color.Purple);

		static ImmutableBrush red;
		public static SolidColorBrush Red => red ??= new ImmutableBrush(Color.Red);

		static ImmutableBrush rosyBrown;
		public static SolidColorBrush RosyBrown => rosyBrown ??= new ImmutableBrush(Color.RosyBrown);

		static ImmutableBrush royalBlue;
		public static SolidColorBrush RoyalBlue => royalBlue ??= new ImmutableBrush(Color.RoyalBlue);

		static ImmutableBrush saddleBrown;
		public static SolidColorBrush SaddleBrown => saddleBrown ??= new ImmutableBrush(Color.SaddleBrown);

		static ImmutableBrush salmon;
		public static SolidColorBrush Salmon => salmon ??= new ImmutableBrush(Color.Salmon);

		static ImmutableBrush sandyBrown;
		public static SolidColorBrush SandyBrown => sandyBrown ??= new ImmutableBrush(Color.SandyBrown);

		static ImmutableBrush seaGreen;
		public static SolidColorBrush SeaGreen => seaGreen ??= new ImmutableBrush(Color.SeaGreen);

		static ImmutableBrush seaShell;
		public static SolidColorBrush SeaShell => seaShell ??= new ImmutableBrush(Color.SeaShell);

		static ImmutableBrush sienna;
		public static SolidColorBrush Sienna => sienna ??= new ImmutableBrush(Color.Sienna);

		static ImmutableBrush silver;
		public static SolidColorBrush Silver => silver ??= new ImmutableBrush(Color.Silver);

		static ImmutableBrush skyBlue;
		public static SolidColorBrush SkyBlue => skyBlue ??= new ImmutableBrush(Color.SkyBlue);

		static ImmutableBrush slateBlue;
		public static SolidColorBrush SlateBlue => slateBlue ??= new ImmutableBrush(Color.SlateBlue);

		static ImmutableBrush slateGray;
		public static SolidColorBrush SlateGray => slateGray ??= new ImmutableBrush(Color.SlateGray);

		static ImmutableBrush snow;
		public static SolidColorBrush Snow => snow ??= new ImmutableBrush(Color.Snow);

		static ImmutableBrush springGreen;
		public static SolidColorBrush SpringGreen => springGreen ??= new ImmutableBrush(Color.SpringGreen);

		static ImmutableBrush steelBlue;
		public static SolidColorBrush SteelBlue => steelBlue ??= new ImmutableBrush(Color.SteelBlue);

		static ImmutableBrush tan;
		public static SolidColorBrush Tan => tan ??= new ImmutableBrush(Color.Tan);

		static ImmutableBrush teal;
		public static SolidColorBrush Teal => teal ??= new ImmutableBrush(Color.Teal);

		static ImmutableBrush thistle;
		public static SolidColorBrush Thistle => thistle ??= new ImmutableBrush(Color.Thistle);

		static ImmutableBrush tomato;
		public static SolidColorBrush Tomato => tomato ??= new ImmutableBrush(Color.Tomato);

		static ImmutableBrush transparent;
		public static SolidColorBrush Transparent => transparent ??= new ImmutableBrush(Color.Transparent);

		static ImmutableBrush turquoise;
		public static SolidColorBrush Turquoise => turquoise ??= new ImmutableBrush(Color.Turquoise);

		static ImmutableBrush violet;
		public static SolidColorBrush Violet => violet ??= new ImmutableBrush(Color.Violet);

		static ImmutableBrush wheat;
		public static SolidColorBrush Wheat => wheat ??= new ImmutableBrush(Color.Wheat);

		static ImmutableBrush white;
		public static SolidColorBrush White => white ??= new ImmutableBrush(Color.White);

		static ImmutableBrush whiteSmoke;
		public static SolidColorBrush WhiteSmoke => whiteSmoke ??= new ImmutableBrush(Color.WhiteSmoke);

		static ImmutableBrush yellow;
		public static SolidColorBrush Yellow => yellow ??= new ImmutableBrush(Color.Yellow);

		static ImmutableBrush yellowGreen;
		public static SolidColorBrush YellowGreen => yellowGreen ??= new ImmutableBrush(Color.YellowGreen);
	}
}