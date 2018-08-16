using Android.Support.V7.Widget;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	internal class StartSnapHelper : LinearSnapHelper
	{
		public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, AView targetView)
		{
			var orientationHelper = CreateOrientationHelper(layoutManager);

			var isHorizontal = layoutManager.CanScrollHorizontally();

			var distance = isHorizontal && IsLayoutReversed(layoutManager)
				? orientationHelper.GetDecoratedEnd(targetView) - orientationHelper.TotalSpace
				: orientationHelper.GetDecoratedStart(targetView);

			return isHorizontal
				? new[] { distance, 1 }
				: new[] { 1, distance };
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

		static bool IsAtLeastHalfVisible(AView view, RecyclerView.LayoutManager layoutManager)
		{
			var orientationHelper = CreateOrientationHelper(layoutManager);
			var reversed = IsLayoutReversed(layoutManager);
			var isHorizontal = layoutManager.CanScrollHorizontally();

			// Find the size of the view (including margins, etc.)
			var size = orientationHelper.GetDecoratedMeasurement(view);

			var amountInView = isHorizontal && reversed 
				? orientationHelper.TotalSpace - orientationHelper.GetDecoratedStart(view)
				: orientationHelper.GetDecoratedEnd(view);
			
			// Is the first visible view at least halfway on screen?
			return amountInView >= size / 2;
		}

		static bool IsLayoutReversed(RecyclerView.LayoutManager layoutManager)
		{
			if (layoutManager is LinearLayoutManager linearLayoutManager)
			{
				return linearLayoutManager.ReverseLayout;
			}

			return false;
		}

		static OrientationHelper CreateOrientationHelper(RecyclerView.LayoutManager layoutManager)
		{
			return layoutManager.CanScrollHorizontally()
				? OrientationHelper.CreateHorizontalHelper(layoutManager)
				: OrientationHelper.CreateVerticalHelper(layoutManager);
		}
	}

	internal class EndSnapHelper : LinearSnapHelper
	{
		public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, AView targetView)
		{
			var orientationHelper = CreateOrientationHelper(layoutManager);

			var isHorizontal = layoutManager.CanScrollHorizontally();

			var distance = isHorizontal && IsLayoutReversed(layoutManager)
				? orientationHelper.GetDecoratedStart(targetView)
				: orientationHelper.GetDecoratedEnd(targetView) - orientationHelper.TotalSpace;

			return isHorizontal
				? new[] { distance, 1 }
				: new[] { 1, distance };
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

		static bool IsAtLeastHalfVisible(AView view, RecyclerView.LayoutManager layoutManager)
		{
			var orientationHelper = CreateOrientationHelper(layoutManager);
			var reversed = IsLayoutReversed(layoutManager);
			var isHorizontal = layoutManager.CanScrollHorizontally();

			// Find the size of the view (including margins, etc.)
			var size = orientationHelper.GetDecoratedMeasurement(view);

			var amountInView = isHorizontal && reversed 
				? orientationHelper.GetDecoratedEnd(view)
				: orientationHelper.TotalSpace - orientationHelper.GetDecoratedStart(view);
			
			// Is the first visible view at least halfway on screen?
			return amountInView >= size / 2;
		}

		static bool IsLayoutReversed(RecyclerView.LayoutManager layoutManager)
		{
			if (layoutManager is LinearLayoutManager linearLayoutManager)
			{
				return linearLayoutManager.ReverseLayout;
			}

			return false;
		}

		static OrientationHelper CreateOrientationHelper(RecyclerView.LayoutManager layoutManager)
		{
			return layoutManager.CanScrollHorizontally()
				? OrientationHelper.CreateHorizontalHelper(layoutManager)
				: OrientationHelper.CreateVerticalHelper(layoutManager);
		}
	}
}