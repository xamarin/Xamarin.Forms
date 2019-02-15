using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	internal class SquareLayout : FrameLayout, ViewTreeObserver.IOnPreDrawListener
	{
		public SquareLayout(Context context) : base(context)
		{
			ViewTreeObserver.AddOnPreDrawListener(this);
		}

		protected SquareLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
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