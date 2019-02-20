using Android.Support.V7.Widget;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	internal class EndSingleSnapHelper : SingleSnapHelper
	{
		public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, AView targetView)
		{
			var orientationHelper = CreateOrientationHelper(layoutManager);
			var isHorizontal = layoutManager.CanScrollHorizontally();
			var rtl = isHorizontal && IsLayoutReversed(layoutManager);

			var distance = rtl
				? -orientationHelper.GetDecoratedStart(targetView)
				: orientationHelper.TotalSpace - orientationHelper.GetDecoratedEnd(targetView);

			return isHorizontal
				? new[] { -distance, 1 }
				: new[] { 1, -distance };
		}
	}
}