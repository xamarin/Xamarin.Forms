using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Foundation;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	class UITouchGestureRecognizer : UIGestureRecognizer
	{
		readonly Action<UITouchGestureRecognizer, TouchEventArgs> _action;
		readonly Func<View> _getView;

		/// <param name="action">Code to invoke when the gesture is recognized.</param>
		/// <summary>Constructs a gesture recognizer and provides a method to invoke when the gesture is recognized.</summary>
		/// <remarks>
		///     This overload allows the method that will be invoked to receive the recognizer that detected the gesture as a
		///     parameter.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public UITouchGestureRecognizer(Action<UITouchGestureRecognizer, TouchEventArgs> action, Func<View> getView)
		{
			_action = action;
			_getView = getView;
		}

		/// <summary>
		///     Is called when the fingers touch the screen.
		/// </summary>
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			_action(this, CreateTouchEventArgs(touches, evt, UIGestureRecognizerState.Began));
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
			_action(this, CreateTouchEventArgs(touches, evt, UIGestureRecognizerState.Cancelled));
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			_action(this, CreateTouchEventArgs(touches, evt, UIGestureRecognizerState.Ended));
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);
			_action(this, CreateTouchEventArgs(touches, evt, UIGestureRecognizerState.Changed));
		}

		TouchEventArgs CreateTouchEventArgs(NSSet touches, UIEvent evt, UIGestureRecognizerState state)
		{
			TouchState touchState = GetTouchState(state);
			return new TouchEventArgs((int)touches.Count, touchState, GetTouchPoints(touches, touchState));
		}

		IReadOnlyList<TouchPoint> GetTouchPoints(NSSet touches, TouchState touchState)
		{
			var points = new List<TouchPoint>((int)touches.Count);

			foreach (UITouch touch in touches)
			{
				View view = _getView();
				Point point = touch.LocationInView(touch.View).ToPoint();
				var isInView = view.Bounds.Contains(point);
				points.Add(new TouchPoint(touches.IndexOf(touch), point, GetTouchState(touch.Phase), isInView));
			}

			return points.AsReadOnly();
		}

		TouchState GetTouchState(UIGestureRecognizerState state)
		{
			switch (state)
			{
				case UIGestureRecognizerState.Possible:
					return TouchState.Default;
				case UIGestureRecognizerState.Began:
					return TouchState.Press;
				case UIGestureRecognizerState.Changed:
					return TouchState.Move;
				case UIGestureRecognizerState.Ended:
					return TouchState.Release;
				case UIGestureRecognizerState.Cancelled:
					return TouchState.Cancel;
				case UIGestureRecognizerState.Failed:
					return TouchState.Fail;
				default:
					return TouchState.Default;
			}
		}

		TouchState GetTouchState(UITouchPhase phase)
		{
			switch (phase)
			{
				case UITouchPhase.Began:
					return TouchState.Press;
				case UITouchPhase.Moved:
					return TouchState.Move;
				case UITouchPhase.Stationary:
					return TouchState.Move;
				case UITouchPhase.Ended:
					return TouchState.Release;
				case UITouchPhase.Cancelled:
					return TouchState.Cancel;
				default:
					return TouchState.Default;
			}
		}
	}
}