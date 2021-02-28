using Android.Graphics;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Android
{
	internal interface ITypefaceManager
	{
		Typeface DefaultTypeface { get; }

		Typeface GetTypeface(Font self);

		Typeface GetTypeface(string fontFamily, FontAttributes fontAttributes);
		
		float GetScaledPixel(Font self);
	}
}