using System;
using System.Collections.Generic;

namespace Xamarin.Forms
{
	public class TouchEventArgs : EventArgs
	{
		public TouchEventArgs(long id, TouchState touchState, IReadOnlyList<TouchPoint> touchPoints)
		{
			Id = id;
			TouchState = touchState;
			TouchPoints = touchPoints;
		}

		public long Id { get; }

		public bool IsInContact { get; }

		public IReadOnlyList<TouchPoint> TouchPoints { get; }

		public TouchState TouchState { get; }
	}
}