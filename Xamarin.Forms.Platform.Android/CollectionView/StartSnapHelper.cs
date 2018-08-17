using Android.Support.V7.Widget;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	internal class StartSnapHelper : EdgeSnapHelper
	{
		public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, AView targetView)
		{
			return CalculateDistanceToFinalSnap(layoutManager, targetView);
		}

		public override AView FindSnapView(RecyclerView.LayoutManager layoutManager)
		{
			if (layoutManager.ItemCount == 0)
			{
				return null;
			}

			if (!(layoutManager is LinearLayoutManager linearLayoutManager))
			{
				// Don't snap to anything if this isn't a LinearLayoutManager;
				// we'll need to update this to handle grids eventually
				return null;
			}

			// Find the first visible item; may be only partially on screen
			var firstVisibleItemPosition = linearLayoutManager.FindFirstVisibleItemPosition();

			if (firstVisibleItemPosition == RecyclerView.NoPosition)
			{
				return null;
			}

			// Get the view itself
			var firstView = linearLayoutManager.FindViewByPosition(firstVisibleItemPosition);

			// If the first visible item is the last one in the collection, snap to it
			if(firstVisibleItemPosition == linearLayoutManager.ItemCount - 1)
			{
				return firstView;
			}

			if (IsAtLeastHalfVisible(firstView, layoutManager))
			{
				// If it's halfway in the viewport, snap to it
				return firstView;
			}

			// The first item is mostly off screen, and it's not the last item in the collection
			// So we'll snap to the start of the next item
			return linearLayoutManager.FindViewByPosition(firstVisibleItemPosition + 1);
		}

		protected override int VisiblePortion(AView view, OrientationHelper orientationHelper, bool rtl)
		{
			if (rtl)
			{
				return orientationHelper.TotalSpace - orientationHelper.GetDecoratedStart(view);
			}

			return orientationHelper.GetDecoratedEnd(view);
		}
	}
}