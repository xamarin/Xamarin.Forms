using System;
using System.Linq;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;

namespace Xamarin.Forms.Platform.Android
{
	public class HorizontalViewPager : ViewPager, IViewPager
	{
		bool _isSwipeEnabled = true;
		CarouselView _element;

		// Fix for #171 System.MissingMethodException: No constructor found
		// SGN: This fix probably indicates something isn't being properly unwired
		public HorizontalViewPager(IntPtr intPtr, JniHandleOwnership jni) : base(intPtr, jni)
		{
		}

		public HorizontalViewPager(Context context) : base(context, null)
		{
		}

		public HorizontalViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			if (ev.Action == MotionEventActions.Up)
			{
				if (_element?.GestureRecognizers.Count > 0)
				{
					var gesture = _element.GestureRecognizers.First() as TapGestureRecognizer;
					if (gesture != null)
						gesture.Command?.Execute(gesture.CommandParameter);
				}
			}

			if (_isSwipeEnabled)
				return base.OnInterceptTouchEvent(ev);
			
			return false;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (_isSwipeEnabled)
				return base.OnTouchEvent(e);
			
			return false;
		}

		public void SetPagingEnabled(bool enabled)
		{
			_isSwipeEnabled = enabled;
		}

		public void SetElement(CarouselView element)
		{
			_element = element;
		}
	}
}