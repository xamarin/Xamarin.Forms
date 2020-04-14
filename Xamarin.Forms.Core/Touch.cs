using System.Collections.Generic;

namespace Xamarin.Forms
{
	public class Touch
	{
		public Touch(int touchIndex, GestureRecognizer.RawTouchPoint touchPoint, View view)
		{
			TouchIndex = touchIndex;
			TouchPoints = new List<GestureRecognizer.RawTouchPoint>(2) { touchPoint };
			Target = view;
		}

		public GestureDirection Gesture { get; set; }

		public View Target { get; }

		public int TouchIndex { get; }

		public List<GestureRecognizer.RawTouchPoint> TouchPoints { get; }
	}
}


/*public class Touch
{
	public int TouchIndex { get; set; }
	public Point ViewPosition { get; set; }
	public Point PagePosition { get; set; }
	public Point ScreenPosition { get; set; }
	public GestureStatusType StatusType { get; set; }
	View Target { get; }
}
*/