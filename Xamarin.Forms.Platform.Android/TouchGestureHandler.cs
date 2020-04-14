using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	class TouchGestureHandler
	{
		readonly Func<double, double> _fromPixels;

		readonly bool _onlyListen;

		public TouchGestureHandler(Func<View> getView, Func<double, double> fromPixels, bool onlyListen = false)
		{
			GetView = getView;
			GetChildElements = () => GetView()?.GetChildElements(Point.Zero) ?? new List<GestureElement>();
			_fromPixels = fromPixels;
			_onlyListen = onlyListen;
		}

		Func<IList<GestureElement>> GetChildElements { get; }

		Func<View> GetView { get; }

		public bool HasAnyGestures()
		{
			View view = GetView();
			return view != null && view.GestureRecognizers.OfType<GestureRecognizer>().Any() ||
			       GetChildElements().GetChildGesturesFor<GestureRecognizer>().Any();
		}

		public bool OnTouch(MotionEvent ev)
		{
			View view = GetView();
			var recognizers = view?.GestureRecognizers.GetGesturesFor<GestureRecognizer>().ToList();

			if (recognizers != null && recognizers.Count > 0)
			{
				foreach (GestureRecognizer touchGesture in recognizers)
				{
					touchGesture.SendTouch(view, CreateTouchEventArgs(ev));
				}

				return !_onlyListen;
			}

			return false;
		}

		TouchEventArgs CreateTouchEventArgs(MotionEvent motionEvent)
		{
			return new TouchEventArgs(motionEvent.PointerCount, GetTouchState(motionEvent), GetTouchPoints(motionEvent));
		}

		IReadOnlyList<GestureRecognizer.RawTouchPoint> GetTouchPoints(MotionEvent me)
		{
			var points = new List<GestureRecognizer.RawTouchPoint>(me.PointerCount);
			var touchState = GetTouchState(me);
			for (var i = 0; i < me.PointerCount; i++)
			{
				var point = new Point(_fromPixels(me.GetX(i)), _fromPixels(me.GetY(i)));
				View view = GetView();
				var isInView = view?.Bounds.Contains(point) ?? false;
				points.Add(new GestureRecognizer.RawTouchPoint(me.GetPointerId(i), point, touchState, isInView));
			}

			return points.AsReadOnly();
		}

		TouchState GetTouchState(MotionEvent motionEvent)
		{
			switch (motionEvent.Action)
			{
				case MotionEventActions.Down:
				case MotionEventActions.Pointer1Down:
				case MotionEventActions.Pointer2Down:
				case MotionEventActions.Pointer3Down:
				case MotionEventActions.ButtonPress:
					return TouchState.Press;

				case MotionEventActions.ButtonRelease:
				case MotionEventActions.Up:
				case MotionEventActions.Pointer1Up:
				case MotionEventActions.Pointer2Up:
				case MotionEventActions.Pointer3Up:
					return TouchState.Release;

				case MotionEventActions.Cancel:
					return TouchState.Cancel;

				case MotionEventActions.HoverEnter:
					return TouchState.Enter;

				case MotionEventActions.HoverExit:
					return TouchState.Exit;

				case MotionEventActions.HoverMove:
					return TouchState.Hover;

				case MotionEventActions.PointerIdMask:
				case MotionEventActions.PointerIdShift:
				case MotionEventActions.Mask:
				case MotionEventActions.Move:
					return TouchState.Move;

				case MotionEventActions.Outside:
					return TouchState.Release;

				default:
					return TouchState.Default;
			}
		}
	}
}