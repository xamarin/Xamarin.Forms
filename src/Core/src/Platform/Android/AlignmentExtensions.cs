using Android.Views;
using ATextAlignment = Android.Views.TextAlignment;
using XTextAlignment = Xamarin.Forms.TextAlignment;

namespace Xamarin.Platform
{
	public static class AlignmentExtensions
	{
		public static ATextAlignment ToTextAlignment(this XTextAlignment alignment)
		{
			switch (alignment)
			{
				case XTextAlignment.Center:
					return ATextAlignment.Center;
				case XTextAlignment.End:
					return ATextAlignment.ViewEnd;
				default:
					return ATextAlignment.ViewStart;
			}
		}

		public static GravityFlags ToHorizontalGravityFlags(this XTextAlignment alignment)
		{
			switch (alignment)
			{
				case XTextAlignment.Center:
					return GravityFlags.CenterHorizontal;
				case XTextAlignment.End:
					return GravityFlags.End;
				default:
					return GravityFlags.Start;
			}
		}

		public static GravityFlags ToVerticalGravityFlags(this XTextAlignment alignment)
		{
			switch (alignment)
			{
				case XTextAlignment.Start:
					return GravityFlags.Top;
				case XTextAlignment.End:
					return GravityFlags.Bottom;
				default:
					return GravityFlags.CenterVertical;
			}
		}
	}
}