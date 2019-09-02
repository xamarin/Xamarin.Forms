using Windows.UI.Input;

namespace Xamarin.Forms.Platform.UAP.Extensions
{
	static class PointExtensions
	{
		public static Point ToPoint(this PointerPoint pointerPoint)
		{
			return new Point(pointerPoint.Position.X, pointerPoint.Position.Y);
		}

		public static Point ToPoint(this Windows.Foundation.Point point)
		{
			return new Point(point.X, point.Y);
		}
	}
}