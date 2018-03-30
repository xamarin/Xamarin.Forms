using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class FormsTextView : TextView
	{
		bool _skip;

		public FormsTextView(Context context) : base(context)
		{
		}

		public FormsTextView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public FormsTextView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
		}

		protected FormsTextView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		//protected override void OnDraw(Canvas canvas)
		//{
		//	System.Diagnostics.Debug.WriteLine($">>>>> FormsTextView OnDraw: {canvas.Width}, {canvas.Height}");
		//	base.OnDraw(canvas);
		//}

		//protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		//{
		//	System.Diagnostics.Debug.WriteLine($">>>>> FormsTextView OnLayout: changed is {changed}, ltrb = {left}, {top}, {right}, {bottom}");
		//	base.OnLayout(changed, left, top, right, bottom);
		//}

		//protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		//{
		//	var widthMode =	MeasureSpec.GetMode(widthMeasureSpec);
		//	var heightMode = MeasureSpec.GetMode(heightMeasureSpec);

		//	var wmode = ((MeasureSpecMode)widthMode).ToString();
		//	var hmode = ((MeasureSpecMode)heightMode).ToString();

		//	var width = MeasureSpecFactory.GetSize(widthMeasureSpec);
		//	var height = MeasureSpecFactory.GetSize(heightMeasureSpec);
		//	System.Diagnostics.Debug.WriteLine($">>>>> FormsTextView OnMeasure: {wmode} {width}, {hmode} {height}");
		//	base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

		//	var mwidth = MeasuredWidth;
		//	var mheight = MeasuredHeight;

		//	System.Diagnostics.Debug.WriteLine($">>>>> FormsTextView OnMeasure result: {mwidth}, {mheight}");
		//}

		public override void Invalidate()
		{
			if (!_skip)
				base.Invalidate();
			_skip = false;
		}

		public void SkipNextInvalidate()
		{
			_skip = true;
		}
	}
}