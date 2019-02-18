using System;
using Android.Content;
using Android.Util;
using AProgressBar = Android.Widget.ProgressBar;

namespace Xamarin.Forms.Platform.Android
{
	internal class CircularProgress : AProgressBar
	{
		public CircularProgress(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			Indeterminate = true;
		}

		public override void Layout(int l, int t, int r, int b)
		{
			var width = r - l;
			var height = b - t;
			var squareSize = Math.Min(width, height);
			l += (width - squareSize) / 2;
			t += (height - squareSize) / 2;
			base.Layout(l, t, l + squareSize, t + squareSize);
		}
	}
}