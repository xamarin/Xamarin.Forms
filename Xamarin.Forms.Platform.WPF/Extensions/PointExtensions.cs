using WPoint = System.Windows.Point;

namespace Xamarin.Forms.Platform.WPF.Extensions
{
	public static class PointExtensions
	{
		public static WPoint ToWindows(this Point point)
		{
			return new WPoint(point.X, point.Y);
		}
	}
}