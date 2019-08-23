using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	internal class TouchGestureHandler
	{
		readonly Func<double, double> _pixelTranslation;

		public TouchGestureHandler(Func<View> getView, Func<IList<GestureElement>> getChildElements, Func<double, double> pixelTranslation)
		{
			GetView = getView;
			GetChildElements = getChildElements;
			_pixelTranslation = pixelTranslation;
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
				return false;

			var result = false;
			foreach (TouchGestureRecognizer touchGesture in view.GestureRecognizers.GetGesturesFor<TouchGestureRecognizer>())
			{
				touchGesture.SendTouch(view, new TouchEventArgs
				{
					Id = ev.PointerCount,
					TouchState = GetTouchState(ev),
					
				});

				if (GetTouchState(ev) == TouchState.Pressed)
				{
					return true;
				}
			}

			return result;
		}

		private TouchState GetTouchState(MotionEvent me)
		{
			switch (me.Action)
			{
				case MotionEventActions.ButtonPress:
					return TouchState.Pressed;
				case MotionEventActions.ButtonRelease:
					return TouchState.Released;
				case MotionEventActions.Cancel:
					return TouchState.Cancelled;
				case MotionEventActions.Down:
					return TouchState.Pressed;
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
				case MotionEventActions.Pointer1Down:
					return TouchState.Pressed;
				case MotionEventActions.Pointer1Up:
					return TouchState.Released;
				case MotionEventActions.Pointer2Down:
					return TouchState.Pressed;
				case MotionEventActions.Pointer2Up:
					return TouchState.Released;
				case MotionEventActions.Pointer3Down:
					return TouchState.Pressed;
				case MotionEventActions.Pointer3Up:
					return TouchState.Released;
				case MotionEventActions.PointerIdMask:
					return TouchState.Unknown;
				case MotionEventActions.PointerIdShift:
					return TouchState.Unknown;
				case MotionEventActions.Up:
					return TouchState.Released;
				default:
					return TouchState.Unknown;
			}
		}
	}
}