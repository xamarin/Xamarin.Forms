using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Xamarin.Forms;

namespace Xamarin.Platform.Handlers
{
	public class LayoutViewGroup : ViewGroup
	{
		public LayoutViewGroup(Context context) : base(context)
		{
		}

		public LayoutViewGroup(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public LayoutViewGroup(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public LayoutViewGroup(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
		}

		public LayoutViewGroup(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
		{
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			if (CrossPlatformMeasure == null)
			{
				base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
			}

			var widthMode = widthMeasureSpec.GetMode();
			var heightMode = heightMeasureSpec.GetMode();

			var width = widthMeasureSpec.GetSize();
			var height = heightMeasureSpec.GetSize();

			var deviceIndependentWidth = Context.FromPixels(width);

			// TODO ezhart Turn this into a method and apply it to width, too
			var deviceIndependentHeight = heightMode == MeasureSpecMode.Unspecified
				? double.PositiveInfinity
				: Context.FromPixels(height);

			var sizeRequest = CrossPlatformMeasure(deviceIndependentWidth, deviceIndependentHeight);

			var nativeWidth = Context.ToPixels(sizeRequest.Request.Width);
			var nativeHeight = Context.ToPixels(sizeRequest.Request.Height);

			SetMeasuredDimension((int)nativeWidth, (int)nativeHeight);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			if (CrossPlatformArrange == null)
			{
				return;
			}

			var deviceIndependentLeft = Context.FromPixels(l);
			var deviceIndependentTop = Context.FromPixels(t);
			var deviceIndependentRight = Context.FromPixels(r);
			var deviceIndependentBottom = Context.FromPixels(b);

			var destination = Xamarin.Forms.Rectangle.FromLTRB(deviceIndependentLeft, deviceIndependentTop,
				deviceIndependentRight, deviceIndependentBottom);

			CrossPlatformArrange(destination);
		}

		internal Func<double, double, SizeRequest> CrossPlatformMeasure { get; set; }
		internal Action<Xamarin.Forms.Rectangle> CrossPlatformArrange { get; set; }
	}
}
