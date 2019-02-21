using System;
using Android.Content;
using Android.Util;
using AProgressBar = Android.Widget.ProgressBar;

namespace Xamarin.Forms.Platform.Android
{
	internal class CircularProgress : AProgressBar
	{
		public int MaxSize { get; set; } = int.MaxValue;

		public int MinSize { get; set; } = 0;

		const int _paddingRatio = 10;

		public CircularProgress(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			Indeterminate = true;
		}

		public override void Layout(int l, int t, int r, int b)
		{
			var width = r - l;
			var height = b - t;
			var squareSize = Math.Min(Math.Max(Math.Min(width, height), MinSize), MaxSize);
			l += (width - squareSize) / 2;
			t += (height - squareSize) / 2;
			var strokeWidth = squareSize / _paddingRatio;
			squareSize += strokeWidth;
			base.Layout(l - strokeWidth, t - strokeWidth, l + squareSize, t + squareSize);
		}
	}
}