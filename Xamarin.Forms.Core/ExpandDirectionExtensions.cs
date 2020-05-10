namespace Xamarin.Forms
{
	static class ExpandDirectionExtensions
	{
		public static bool IsVertical(this ExpandDirection orientation)
			=> orientation == ExpandDirection.Down
			|| orientation == ExpandDirection.Up;

		public static bool IsRegularOrder(this ExpandDirection orientation)
			=> orientation == ExpandDirection.Down
			|| orientation == ExpandDirection.Right;
	}
}