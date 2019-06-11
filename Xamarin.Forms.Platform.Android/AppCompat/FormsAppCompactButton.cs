using Android.Content;
using Android.Support.V7.Widget;

namespace Xamarin.Forms.Platform.Android.AppCompat
{
	internal class FormsAppCompatButton : AppCompatButton
	{
		public FormsAppCompatButton(Context ctx) : base(ctx)
		{
		}

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);
			// fix autoSizeText 
			// https://android.googlesource.com/platform/frameworks/support/+/master/v7/appcompat/src/main/java/androidx/appcompat/widget/AppCompatTextViewAutoSizeHelper.java?autodive=0%2F%2F#632
			if (Layout != null)
			{
				if (!IsInLayout)
					RequestLayout();
				else
					ForceLayout();
				Invalidate();
			}
		}
	}
}
