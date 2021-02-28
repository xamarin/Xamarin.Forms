using Android.Graphics;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Android
{
	public static class FontExtensions
	{
		public static float ToScaledPixel(this Font self)
			=> Forms.TypefaceManager.GetScaledPixel(self);

		public static Typeface ToTypeface(this Font self)
			=> Forms.TypefaceManager.GetTypeface(self);

		internal static Typeface ToTypeface(this string fontfamily, FontAttributes attr = FontAttributes.None)
			=> Forms.TypefaceManager.GetTypeface(fontfamily, attr);

		internal static bool IsDefault(this IFontElement self)
			=> self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true) && self.FontAttributes == FontAttributes.None;

		internal static Typeface ToTypeface(this IFontElement self)
		{
			if (self.IsDefault())
				return Forms.TypefaceManager.DefaultTypeface;

			return Forms.TypefaceManager.GetTypeface(self.FontFamily, self.FontAttributes);
		}
	}
}