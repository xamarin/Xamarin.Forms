using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms
{
	public class TouchEventArgs : EventArgs
	{
		public TouchEventArgs(int id, TouchState touchState, IReadOnlyList<GestureRecognizer.RawTouchPoint> touchPoints)
		{
			Id = id;
			TouchState = touchState;
			TouchPoints = touchPoints;
		}

		public int Id { get; }

		public bool IsInOriginalView { get => TouchPoints?.All(a => a.IsInOriginalView) ?? false; }

		public int TouchCount
		{
			get => TouchPoints?.Select(s => s.TouchId).Distinct().Count() ?? 0;
		}

		public IReadOnlyList<GestureRecognizer.RawTouchPoint> TouchPoints { get; }

		public TouchState TouchState { get; }
	}
}