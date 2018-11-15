using Android.Support.V7.Widget;

namespace Xamarin.Forms.Platform.Android
{
	internal class EndPagerSnapHelper : PagerSnapHelper
	{
		protected static OrientationHelper CreateOrientationHelper(RecyclerView.LayoutManager layoutManager)
		{
			return layoutManager.CanScrollHorizontally()
				? OrientationHelper.CreateHorizontalHelper(layoutManager)
				: OrientationHelper.CreateVerticalHelper(layoutManager);
		}

		protected static bool IsLayoutReversed(RecyclerView.LayoutManager layoutManager)
		{
			if (layoutManager is LinearLayoutManager linearLayoutManager)
			{
				return linearLayoutManager.ReverseLayout;
			}

			return false;
		}

		public override global::Android.Views.View FindSnapView(RecyclerView.LayoutManager layoutManager)
		{
			if (layoutManager.ItemCount == 0)
			{
				return null;
			}

			if (!(layoutManager is LinearLayoutManager linearLayoutManager))
			{
				// Don't snap to anything if this isn't a LinearLayoutManager;
				return null;
			}

			// Find the last fully visible item
			var lastVisibleItemPosition = linearLayoutManager.FindLastVisibleItemPosition();

			if (lastVisibleItemPosition == RecyclerView.NoPosition)
			{
				// If there are no fully visible items, drop back to default PagerSnapHelper behavior
				return base.FindSnapView(layoutManager);
			}

			// Return the view to snap
			return linearLayoutManager.FindViewByPosition(lastVisibleItemPosition);
		}

		public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, global::Android.Views.View targetView)
		{
			var orientationHelper = CreateOrientationHelper(layoutManager);
			var isHorizontal = layoutManager.CanScrollHorizontally();
			var rtl = isHorizontal && IsLayoutReversed(layoutManager);

			var distance = rtl
				? -orientationHelper.GetDecoratedStart(targetView)
				: orientationHelper.GetDecoratedEnd(targetView);

			return isHorizontal
				? new[] { distance, 1 }
				: new[] { 1, distance };
		}
	}
}