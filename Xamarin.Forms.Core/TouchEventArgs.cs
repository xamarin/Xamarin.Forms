using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms
{
	public class TouchEventArgs : EventArgs
	{
		public TouchEventArgs(int id, TouchState touchState, IReadOnlyList<TouchPoint> touchPoints)
		{
			Id = id;
			TouchState = touchState;
			TouchPoints = touchPoints;
		}

		public int Id { get; }

		public bool IsInOriginalView { get => TouchPoints.All(a => a.IsInOriginalView); }

		public int TouchCount
		{
			get => TouchPoints.Select(s => s.TouchId).Distinct().Count();
		}

		public IReadOnlyList<TouchPoint> TouchPoints { get; }

		public TouchState TouchState { get; }
	}
}