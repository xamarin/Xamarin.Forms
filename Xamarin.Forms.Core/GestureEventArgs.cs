using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms
{
	public class GestureEventArgs : EventArgs
	{
		public GestureEventArgs(TouchEventArgs arg)
		{
			Id = arg.Id;
			TouchState = arg.TouchState;
			TouchPoints = arg.TouchPoints;
			IsInOriginalView = arg.IsInOriginalView;
			TouchCount = arg.TouchCount;
		}

		public int Id { get; }

		public bool IsInOriginalView { get; set; }

		public int TouchCount { get; }

		public IReadOnlyList<GestureRecognizer.RawTouchPoint> TouchPoints { get; }

		public TouchState TouchState { get; }
	}
}