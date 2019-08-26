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


		/// <param name="action">Code to invoke when the gesture is recognized.</param>
		/// <summary>Constructs a gesture recognizer and provides a method to invoke when the gesture is recognized.</summary>
		/// <remarks>This overload allows the method that will be invoked to receive the recognizer that detected the gesture as a parameter.</remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public UITouchGestureRecognizer(Action<UITouchGestureRecognizer, TouchEventArgs> action)
		{
			_action = action;
		}

		
		public override void Reset()
		{
			base.Reset();
		}

		/// <summary>
		///   Is called when the fingers touch the screen.
		/// </summary>
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			//_action(this, GetTouchEventArgs(touches, evt));
			_action(this, new TouchEventArgs
			{
				TouchState = TouchState.Pressed
			});
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
			_action(this, new TouchEventArgs
			{
				TouchState = TouchState.Released
			});
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			_action(this, new TouchEventArgs
			{
				TouchState = TouchState.Released
			});
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);
			_action(this, new TouchEventArgs
			{
				TouchState = TouchState.Move
			});
		}

		TouchEventArgs GetTouchEventArgs(NSSet touches, UIEvent evt)
		{
			var ev = new TouchEventArgs();
			switch (evt.Type)
			{
				case UIEventType.Touches:
					ev.TouchState = TouchState.Pressed;
					break;
				case UIEventType.Motion:
					ev.TouchState = TouchState.Move;
					break;
				case UIEventType.RemoteControl:
					break;
				case UIEventType.Presses:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return ev;
		}
	}
}