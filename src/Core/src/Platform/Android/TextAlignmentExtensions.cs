using Android.Widget;
using AGravityFlags = Android.Views.GravityFlags;
using ATextAlignment = Android.Views.TextAlignment;

namespace Microsoft.Maui
{
	internal static class TextAlignmentExtensions
	{
		public static ATextAlignment ToTextAlignment(this TextAlignment alignment)
		{
			switch (alignment)
			{
				case TextAlignment.Center:
					return ATextAlignment.Center;
				case TextAlignment.End:
					return ATextAlignment.ViewEnd;
				default:
					return ATextAlignment.ViewStart;
			}
		}

		public static AGravityFlags ToVerticalGravityFlags(this TextAlignment alignment)
		{
			switch (alignment)
			{
				case TextAlignment.Start:
					return AGravityFlags.Top;
				case TextAlignment.End:
					return AGravityFlags.Bottom;
				default:
					return AGravityFlags.CenterVertical;
			}
		}

		internal static void UpdateHorizontalAlignment(this TextView view, TextAlignment alignment, bool hasRtlSupport, AGravityFlags orMask = AGravityFlags.NoGravity)
		{
			view.TextAlignment = alignment.ToTextAlignment();
		}

		internal static void UpdateVerticalAlignment(this TextView view, TextAlignment alignment, AGravityFlags orMask = AGravityFlags.NoGravity)
		{
			view.Gravity = alignment.ToVerticalGravityFlags() | orMask;
		}

		internal static void UpdateTextAlignment(this TextView view, TextAlignment horizontal, TextAlignment vertical)
		{
			view.TextAlignment = horizontal.ToTextAlignment();
			view.Gravity = vertical.ToVerticalGravityFlags();
		}

		internal static void UpdateTextAlignment(this TextView view, ITextAlignment textAlignment)
		{
			view.TextAlignment = textAlignment.HorizontalTextAlignment.ToTextAlignment();
			view.Gravity = textAlignment.VerticalTextAlignment.ToVerticalGravityFlags();
		}
	}
}