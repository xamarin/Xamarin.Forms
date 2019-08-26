using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	class TouchGestureHandler
	{
		public TouchGestureHandler(Func<View> getView, Func<IList<GestureElement>> getChildElements)
		{
			GetView = getView;
			GetChildElements = getChildElements;
		}

		Func<IList<GestureElement>> GetChildElements { get; }

		Func<View> GetView { get; }

		public bool HasAnyGestures()
		{
			View view = GetView();
			return view != null && view.GestureRecognizers.OfType<TouchGestureRecognizer>().Any() ||
			       GetChildElements().GetChildGesturesFor<TouchGestureRecognizer>().Any();
		}

		public bool OnTouch(MotionEvent ev)
		{
			View view = GetView();

			if (view == null)
			{
				return false;
			}

			var result = false;
			foreach (TouchGestureRecognizer touchGesture in view.GestureRecognizers.GetGesturesFor<TouchGestureRecognizer>())
			{
				touchGesture.SendTouch(view, new TouchEventArgs { Id = ev.PointerCount, TouchState = GetTouchState(ev) });

				if (GetTouchState(ev) == TouchState.Pressed)
				{
					return true;
				}
			}

			return result;
		}

		TouchState GetTouchState(MotionEvent me)
		{
			switch (me.Action)
			{
				case MotionEventActions.Down:
				case MotionEventActions.Pointer1Down:
				case MotionEventActions.Pointer2Down:
				case MotionEventActions.Pointer3Down:
				case MotionEventActions.ButtonPress:
					return TouchState.Pressed;

				case MotionEventActions.ButtonRelease:
				case MotionEventActions.Up:
				case MotionEventActions.Pointer1Up:
				case MotionEventActions.Pointer2Up:
				case MotionEventActions.Pointer3Up:
					return TouchState.Released;

				case MotionEventActions.Cancel:
					return TouchState.Cancelled;

				case MotionEventActions.HoverEnter:
					return TouchState.Entered;
				case MotionEventActions.HoverExit:
					return TouchState.Exited;
				case MotionEventActions.HoverMove:
					return TouchState.Move;

				case MotionEventActions.Mask:
					return TouchState.Unknown;

				case MotionEventActions.Move:
					return TouchState.Move;

				case MotionEventActions.Outside:
					return TouchState.Released;

				case MotionEventActions.PointerIdMask:
					return TouchState.Unknown;
				case MotionEventActions.PointerIdShift:
					return TouchState.Unknown;

				default:
					return TouchState.Unknown;
			}
		}
	}
}