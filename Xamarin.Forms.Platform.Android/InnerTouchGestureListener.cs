using System;
using Android.Runtime;
using Android.Views;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	class InnerTouchGestureListener : GestureDetector.SimpleOnGestureListener, AView.IOnTouchListener
	{
		bool _disposed;
		TouchGestureHandler _touchGestureHandler;

		public InnerTouchGestureListener(TouchGestureHandler touchGestureHandler)
		{
			_touchGestureHandler = touchGestureHandler ?? throw new ArgumentNullException(nameof(touchGestureHandler));
		}

		// This is needed because GestureRecognizer callbacks can be delayed several hundred milliseconds
		// which can result in the need to resurrect this object if it has already been disposed. We dispose
		// eagerly to allow easier garbage collection of the renderer
		internal InnerTouchGestureListener(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
		{
		}

		public bool OnTouch(AView v, MotionEvent e)
		{
			return _touchGestureHandler.OnTouch(e);
		}

		public bool OnCapturedPointer(AView view, MotionEvent e)
		{
			return _touchGestureHandler.OnTouch(e);
		}

		public bool OnTouchEvent(MotionEvent ev)
		{
			return HasAnyGestures() && _touchGestureHandler.OnTouch(ev);
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

		bool HasAnyGestures()
		{
			return _touchGestureHandler.HasAnyGestures();
		}
	}
}