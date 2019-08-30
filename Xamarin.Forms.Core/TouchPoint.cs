namespace Xamarin.Forms
{
	public struct TouchPoint
	{
		public TouchPoint(Point point, bool isInOriginalView)
		{
			Point = point;
			IsInOriginalView = isInOriginalView;
		}

		public Point Point { get; }

		public bool IsInOriginalView { get; }
	}
}