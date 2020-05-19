using Android.Content;

#if __ANDROID_29__
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
#else
using Android.Support.V7.Widget;
#endif

namespace Xamarin.Forms.Platform.Android
{
	public class ItemsGridLayoutManager : GridLayoutManager
	{
		public ItemsGridLayoutManager(Context context, int spanCount) : base(context, spanCount)
		{

		}

		public ItemsGridLayoutManager(Context context, int spanCount, int orientation, bool reverseLayout) : base(context, spanCount, orientation, reverseLayout)
		{

		}

		public override bool SupportsPredictiveItemAnimations()
		{
			return false;
		}
	}
}