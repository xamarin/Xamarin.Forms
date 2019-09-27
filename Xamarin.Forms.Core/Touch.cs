using System.Collections.Generic;

namespace Xamarin.Forms
{
	public class Touch
	{
		public Touch(int touchIndex, TouchPoint touchPoint, View view)
		{
			TouchIndex = touchIndex;
			TouchPoints = new List<TouchPoint>(2) { touchPoint };
			Target = view;
		}

		public GestureDirection Gesture { get; set; }

		public View Target { get; }

		public int TouchIndex { get; }

		public List<TouchPoint> TouchPoints { get; }
	}
}