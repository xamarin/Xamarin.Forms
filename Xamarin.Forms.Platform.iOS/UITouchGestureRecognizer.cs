﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CoreGraphics;
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
			var touchState = GetTouchState(state);
			return new TouchEventArgs((int)touches.Count, touchState, GetTouchPoints(touches, touchState));
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

		IReadOnlyList<TouchPoint> GetTouchPoints(NSSet touches, TouchState touchState)
		{
			var points = new List<TouchPoint>((int)touches.Count);

			foreach (UITouch touch in touches)
			{
				var point = touch.LocationInView(touch.View).ToPoint();
				var view = _getView();
				var isInView = view?.Bounds.Contains(point) ?? false;
				points.Add(new TouchPoint(touches.IndexOf(touch), point, touchState, isInView));
			}

			return points.AsReadOnly();
		}
	}
}