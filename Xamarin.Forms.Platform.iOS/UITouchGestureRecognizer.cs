using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	class UITouchGestureRecognizer : UIGestureRecognizer
	{
		readonly Action<UITouchGestureRecognizer, TouchEventArgs> _action;
		readonly Func<View> _getView;


		/// <param name="action">Code to invoke when the gesture is recognized.</param>
		/// <summary>Constructs a gesture recognizer and provides a method to invoke when the gesture is recognized.</summary>
		/// <remarks>This overload allows the method that will be invoked to receive the recognizer that detected the gesture as a parameter.</remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public UITouchGestureRecognizer(Action<UITouchGestureRecognizer, TouchEventArgs> action, Func<View> getView)
		{
			_action = action;
			_getView = getView;
		}

		/// <summary>
		///   Is called when the fingers touch the screen.
		/// </summary>
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			State = UIGestureRecognizerState.Began;
			_action(this, CreateTouchEventArgs(touches, evt));
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
			State = UIGestureRecognizerState.Cancelled;
			_action(this, CreateTouchEventArgs(touches, evt));
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			State = UIGestureRecognizerState.Ended;
			_action(this, CreateTouchEventArgs(touches, evt));
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);
			State = UIGestureRecognizerState.Changed;
			_action(this, CreateTouchEventArgs(touches, evt));
		}


		TouchEventArgs CreateTouchEventArgs(NSSet touches, UIEvent evt)
		{
			return new TouchEventArgs((int)touches.Count, GetTouchState(touches, evt), GetTouchPoints(touches, evt));
		}


		TouchState GetTouchState(NSSet touches, UIEvent evt)
		{
			switch (State)
			{
				case UIGestureRecognizerState.Possible:
					return TouchState.Unknown;
				case UIGestureRecognizerState.Began:
					return TouchState.Pressed;
				case UIGestureRecognizerState.Changed:
					return TouchState.Move;
				case UIGestureRecognizerState.Ended:
					return TouchState.Released;
				case UIGestureRecognizerState.Cancelled:
					return TouchState.Cancelled;
				case UIGestureRecognizerState.Failed:
					return TouchState.Failed;
				default:
					return TouchState.Unknown;
			}
		}

		IReadOnlyList<TouchPoint> GetTouchPoints(NSSet touches, UIEvent evt)
		{
			var points = new List<TouchPoint>((int)touches.Count);

			foreach (UITouch touch in touches)
			{
				touch.PreviousLocationInView()

				var point = touch.LocationInView(touch.View).ToPoint();
				var view = _getView();
				var isInView = view?.Bounds.Contains(point) ?? false;
				points.Add(new TouchPoint(point, isInView));
			}

			return points.AsReadOnly();
		}

	}
}