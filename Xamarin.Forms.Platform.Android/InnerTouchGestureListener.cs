using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android
{
	internal class InnerTouchGestureListener : GestureDetector.SimpleOnGestureListener, //GestureDetector.IOnGestureListener, GestureDetector.IOnDoubleTapListener ,
		 global::Android.Views.View.IOnTouchListener        //global::Android.Views.View.IOnGenericMotionListener
	{
		TouchGestureHandler _touchGestureHandler;
		bool _disposed;
		public InnerTouchGestureListener(TouchGestureHandler touchGestureHandler)
		{
			if (touchGestureHandler == null)
			{
				throw new ArgumentNullException(nameof(touchGestureHandler));
			}

			_touchGestureHandler = touchGestureHandler;
			
			//_swipeCompletedDelegate = swipeGestureHandler.OnSwipeComplete;
		}

		public bool OnTouchEvent(MotionEvent ev)
		{
			if(!HasAnyGestures())
				return false;

			var x1 = ev.ActionMasked;
			var x2 = ev.ActionIndex;


			return _touchGestureHandler.OnTouch(ev);
		}
		

		bool HasAnyGestures()
		{
			return _touchGestureHandler.HasAnyGestures();
		}

		// This is needed because GestureRecognizer callbacks can be delayed several hundred milliseconds
		// which can result in the need to resurrect this object if it has already been disposed. We dispose
		// eagerly to allow easier garbage collection of the renderer
		internal InnerTouchGestureListener(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
		{
		}


		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				_touchGestureHandler = null;
			}

			base.Dispose(disposing);
		}

		public bool OnCapturedPointer(global::Android.Views.View view, MotionEvent e)
		{
			return _touchGestureHandler.OnTouch(e);
		}

		public bool OnTouch(global::Android.Views.View v, MotionEvent e)
		{
			return _touchGestureHandler.OnTouch(e);
		}

	}
}