using System;
using Android.Support.V7.Widget;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class SnapManager : IDisposable
	{
		readonly RecyclerView _recyclerView;
		readonly ItemsView _itemsView;
		SnapHelper _snapHelper;

		public SnapManager(ItemsView itemsView, RecyclerView recyclerView)
		{
			_recyclerView = recyclerView;
			_itemsView = itemsView;
		}

		internal void UpdateSnapBehavior()
		{
			if (!(_itemsView.ItemsLayout is ItemsLayout itemsLayout))
			{
				return;
			}

			var snapPointsType = itemsLayout.SnapPointsType;

			// Clear our the existing snap helper (if any)
			_snapHelper?.AttachToRecyclerView(null);
			_snapHelper = null;

			if (snapPointsType == SnapPointsType.None)
			{
				return;
			}

			var alignment = itemsLayout.SnapPointsAlignment;

			// Create a new snap helper
			_snapHelper = CreateSnapHelper(snapPointsType, alignment);
			
			// And attach it to this RecyclerView
			_snapHelper.AttachToRecyclerView(_recyclerView);
		}

		protected virtual SnapHelper CreateSnapHelper(SnapPointsType snapPointsType, SnapPointsAlignment alignment)
		{
			if (snapPointsType == SnapPointsType.Mandatory)
			{
				switch (alignment)
				{
					case SnapPointsAlignment.Start:
						return new StartSnapHelper();
					case SnapPointsAlignment.Center:
					case SnapPointsAlignment.End:
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
				}
			}

			return new LinearSnapHelper();
		}

		public void Dispose()
		{
			_recyclerView?.Dispose();
			_snapHelper?.Dispose();
		}
	}

	// TODO hartez 2018/08/08 17:22:47 Set up a vertical test so we can determine if this modified StartSnapHelper will work vertically as well	

	internal class StartSnapHelper : LinearSnapHelper
	{
		public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, AView targetView)
		{
			var orientationHelper = CreateOrientationHelper(layoutManager);

			var distance = orientationHelper.GetDecoratedStart(targetView);

			return layoutManager.CanScrollHorizontally()
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
				// Don't snap to anything if this isn't a LinearLayoutManager; we'll need to update this to handle grids eventually
				return null;
			}

			// Find the first visible item; may be only partially on screen
			var firstItemPosition = linearLayoutManager.FindFirstVisibleItemPosition();

			if (firstItemPosition == RecyclerView.NoPosition)
			{
				return null;
			}

			// Get the view itself
			var firstView = linearLayoutManager.FindViewByPosition(firstItemPosition);

			// If the first visible item is the last one in the collection, snap to it
			if(firstItemPosition == linearLayoutManager.ItemCount - 1)
			{
				return firstView;
			}

			if (IsAtLeastHalfVisible(firstView, layoutManager))
			{
				// If it's halfway on screen, snap to it
				return firstView;
			}

			// The first item is mostly off screen, and it's not the last item in the collection
			// So we'll snap to the next item snap to the start of the next item
			return linearLayoutManager.FindViewByPosition(firstItemPosition + 1);
		}

		static bool IsAtLeastHalfVisible(AView view, RecyclerView.LayoutManager layoutManager)
		{
			var orientationHelper = CreateOrientationHelper(layoutManager);

			// Find the size of the view (including margins, etc.)
			var size = orientationHelper.GetDecoratedMeasurement(view);
			var trailingEdge = orientationHelper.GetDecoratedEnd(view);

			// Is the first visible view at least halfway on screen?
			return trailingEdge >= size / 2;
		}

		static OrientationHelper CreateOrientationHelper(RecyclerView.LayoutManager layoutManager)
		{
			return layoutManager.CanScrollHorizontally()
				? OrientationHelper.CreateHorizontalHelper(layoutManager)
				: OrientationHelper.CreateVerticalHelper(layoutManager);
		}
	}
}