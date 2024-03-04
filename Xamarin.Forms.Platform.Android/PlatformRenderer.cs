using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	internal class PlatformRenderer : ViewGroup
	{
		readonly IPlatformLayout _canvas;

		public PlatformRenderer(Context context, IPlatformLayout canvas) : base(context)
		{
			_canvas = canvas;
			if (!Flags.IsAccessibilityExperimentalSet())
			{
				Focusable = true;
				FocusableInTouchMode = true;
			}
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			Profile.FrameBegin();
			SetMeasuredDimension(r - l, b - t);
			_canvas?.OnLayout(changed, l, t, r, b);
			Profile.FrameEnd();
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			SetMeasuredDimension(MeasureSpec.GetSize(widthMeasureSpec), MeasureSpec.GetSize(heightMeasureSpec));

			var width = MeasureSpecFactory.GetSize(widthMeasureSpec);
			var height = MeasureSpecFactory.GetSize(heightMeasureSpec);

			for (int i = 0; i < ChildCount; i++)
			{
				var child = GetChildAt(i);
				child.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
					MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.Exactly));
			}
		}
	}
}