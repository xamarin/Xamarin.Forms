namespace Xamarin.Forms
{
	public struct TouchPoint
	{
		public TouchPoint(int touchId, Point point, TouchState state, bool isInOriginalView)
		{
			TouchId = touchId;
			Point = point;
			TouchState = state;
			IsInOriginalView = isInOriginalView;
		}

		public Point Point { get; }

		public bool IsInOriginalView { get; }

		public int TouchId { get; }

		public TouchState TouchState { get; }
	}
}