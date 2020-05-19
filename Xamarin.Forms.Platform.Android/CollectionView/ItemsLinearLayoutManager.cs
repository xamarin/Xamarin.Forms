using Android.Content;

#if __ANDROID_29__
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
#else
using Android.Support.V7.Widget;
#endif

namespace Xamarin.Forms.Platform.Android
{
	public class ItemsLinearLayoutManager : LinearLayoutManager
	{
		public ItemsLinearLayoutManager(Context context) : base(context)
		{

		}

		public ItemsLinearLayoutManager(Context context, int orientation, bool reverseLayout) : base(context, orientation, reverseLayout)
		{

		}

		public override bool SupportsPredictiveItemAnimations()
		{
			return false;
		}
	}
}