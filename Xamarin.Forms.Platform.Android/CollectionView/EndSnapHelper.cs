using Android.Support.V7.Widget;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	internal class EndSnapHelper : EdgeSnapHelper
	{
		public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, AView targetView)
		{
			// The -1 flips everything around so we look at the end of the view instead of the start
			return CalculateDistanceToFinalSnap(layoutManager, targetView, -1);
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

			// Find the last visible item; may be only partially on screen
			var lastVisibleItemPosition = linearLayoutManager.FindLastVisibleItemPosition();

			if (lastVisibleItemPosition == RecyclerView.NoPosition)
			{
				return null;
			}

			// Get the view itself
			var lastView = linearLayoutManager.FindViewByPosition(lastVisibleItemPosition);

			// If the last visible item is the first one in the collection, snap to it
			if(lastVisibleItemPosition == 0)
			{
				return lastView;
			}

			if (IsAtLeastHalfVisible(lastView, layoutManager))
			{
				// If it's halfway in the viewport, snap to it
				return lastView;
			}

			// The last item is mostly off screen, and it's not the first item in the collection
			// So we'll snap to the end of the previous item
			return linearLayoutManager.FindViewByPosition(lastVisibleItemPosition - 1);
		}

		protected override int VisiblePortion(AView view, OrientationHelper orientationHelper, bool rtl)
		{
			if (rtl)
			{
				return orientationHelper.GetDecoratedEnd(view);
			}

			return orientationHelper.TotalSpace - orientationHelper.GetDecoratedStart(view);
		}
	}
}