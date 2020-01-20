using WPoint = Windows.Foundation.Point;

namespace Xamarin.Forms.Platform.UWP
{
	public static class PointExtensions
	{
		public static WPoint ToWindows(this Point point)
		{
			return new WPoint(point.X, point.Y);
		}
	}
}