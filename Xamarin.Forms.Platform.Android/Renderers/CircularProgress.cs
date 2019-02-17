using System;
using Android.Content;
using Android.Util;
using Android.Views;
using AProgressBar = Android.Widget.ProgressBar;

namespace Xamarin.Forms.Platform.Android
{
	internal class CircularProgress : AProgressBar, ViewTreeObserver.IOnPreDrawListener
	{
		public CircularProgress(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			ViewTreeObserver.AddOnPreDrawListener(this);
			Indeterminate = true;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && !this.IsDisposed())
				ViewTreeObserver.RemoveOnPreDrawListener(this);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

			if (IsInEditMode)
			{
				if (Math.Abs(heightMeasureSpec) < Math.Abs(widthMeasureSpec))
					OnMeasure(heightMeasureSpec, heightMeasureSpec);
				else
					OnMeasure(widthMeasureSpec, widthMeasureSpec);
			}
		}

		public bool OnPreDraw()
		{
			if (Width != Height)
			{
				var squareSize = Math.Min(Width, Height);
				var lp = LayoutParameters;
				lp.Width = squareSize;
				lp.Height = squareSize;
				RequestLayout();
				return false;
			}
			return true;
		}
	}
}