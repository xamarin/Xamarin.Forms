namespace Xamarin.Forms
{
	public static class ExpanderOrientationExtensions
	{
		public static bool IsVertical(this ExpanderOrientation orientation)
			=> orientation == ExpanderOrientation.TopToBottom
			|| orientation == ExpanderOrientation.BottomToTop;

		public static bool IsRegularOrder(this ExpanderOrientation orientation)
			=> orientation == ExpanderOrientation.TopToBottom
			|| orientation == ExpanderOrientation.LeadingToTrailing;
	}
}