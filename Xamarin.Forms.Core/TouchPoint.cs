using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public struct TouchPoint
	{
		public Xamarin.Forms.Point Point { get; }
		public bool IsInOriginalView { get; }
	}

	//public sealed class TouchPoint
	//{
	//	public static readonly BindableProperty TouchIndexProperty;
	//	public int TouchIndex { get; set; }

	//	public static readonly BindableProperty IsTouchingProperty;
	//	public bool IsTouching { get; set; }

	//	// Bindable Property will live on GestureRecognizer
	//	public ICommand CancelledCommand { get; set; }
	//	public object CancelledCommandParameter { get; set; }

	//	// Bindable Property will live on GestureRecognizer
	//	public ICommand StartedCommand { get; set; }
	//	public object StartedCommandParameter { get; set; }

	//	public ICommand CompletedCommand { get; set; }
	//	public object CompleteCommandParameter { get; set; }

	//	public event EventHandler<TouchPointEventArgs> TouchPointUpdated { get; }
	//}


	public class Touch
	{
		public int TouchIndex { get; set; }
		public Point ViewPosition { get; set; }
		public Point PagePosition { get; set; }
		public Point ScreenPosition { get; set; }
		//public GestureStatusType StatusType { get; set; }
		View Target { get; }
	}

	public class TouchEvent
	{
		public IReadOnlyList<Touch> Touches { get; set; }
		public IReadOnlyList<Touch> ChangedTouches { get; set; }
		public IReadOnlyList<Touch> TargetTouches { get; set; }
		View Target { get; }
	}

	public class GestureEventArgs : EventArgs
	{
		public TouchEvent TouchEvent { get; private set; }
	}

	public class TouchEventArgs : EventArgs
	{
		public TouchState TouchState { get; set; }
		public IReadOnlyList<TouchPoint> TouchPoints { get; }
		public long Id { get; set; }
		public bool IsInContact { get; }
	}

}
