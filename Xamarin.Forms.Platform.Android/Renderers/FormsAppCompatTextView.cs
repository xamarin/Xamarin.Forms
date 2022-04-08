using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using AndroidX.AppCompat.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class FormsAppCompatTextView : AppCompatTextView
	{
		public FormsAppCompatTextView(Context context) : base(context)
		{
		}

		[Obsolete]
		public FormsAppCompatTextView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		[Obsolete]
		public FormsAppCompatTextView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
		}

		[Obsolete]
		protected FormsAppCompatTextView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		[Obsolete]
		public void SkipNextInvalidate()
		{
		}
	}
}