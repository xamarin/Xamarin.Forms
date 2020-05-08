using System;
using AndroidX.RecyclerView.Widget;
using ARect = Android.Graphics.Rect;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class SpacingItemDecoration : RecyclerView.ItemDecoration
	{
		readonly ItemsLayoutOrientation _orientation;
		readonly double _verticalSpacing;
		double _adjustedVerticalSpacing = -1;
		double _adjustedVerticalOffset;
		readonly double _horizontalSpacing;
		double _adjustedHorizontalSpacing = -1;
		double _adjustedHorizontalOffset;
		readonly int _spanCount;

		public SpacingItemDecoration(IItemsLayout itemsLayout)
		{
			if (itemsLayout == null)
			{
				throw new ArgumentNullException(nameof(itemsLayout));
			}

			switch (itemsLayout)
			{
				case GridItemsLayout gridItemsLayout:
					_orientation = gridItemsLayout.Orientation;
					_horizontalSpacing = gridItemsLayout.HorizontalItemSpacing;
					_verticalSpacing = gridItemsLayout.VerticalItemSpacing;
					_spanCount = gridItemsLayout.Span;
					break;
				case LinearItemsLayout listItemsLayout:
					_orientation = listItemsLayout.Orientation;
					if (_orientation == ItemsLayoutOrientation.Horizontal)
						_horizontalSpacing = listItemsLayout.ItemSpacing;
					else
						_verticalSpacing = listItemsLayout.ItemSpacing;
					_spanCount = 1;
					break;
			}
		}

		public override void GetItemOffsets(ARect outRect, AView view, RecyclerView parent, RecyclerView.State state)
		{
			base.GetItemOffsets(outRect, view, parent, state);

			if (_adjustedVerticalSpacing == -1)
			{
				_adjustedVerticalSpacing = parent.Context.ToPixels(_verticalSpacing);
				_adjustedVerticalOffset = _adjustedVerticalSpacing / 2;
			}

			if (_adjustedHorizontalSpacing == -1)
			{
				_adjustedHorizontalSpacing = parent.Context.ToPixels(_horizontalSpacing);
				_adjustedHorizontalOffset = _adjustedHorizontalSpacing / 2;
			}

			var itemViewType = parent.GetChildViewHolder(view).ItemViewType;

			if (itemViewType == ItemViewType.Header)
			{
				outRect.Bottom = (int)_adjustedVerticalSpacing;
				return;
			}

			if (itemViewType == ItemViewType.Footer)
			{
				return;
			}

			var spanIndex = 0;

			if (view.LayoutParameters is GridLayoutManager.LayoutParams gridLayoutParameters)
			{
				spanIndex = gridLayoutParameters.SpanIndex;
			}

			var adapter = parent.GetAdapter();
			var childCount = adapter.ItemCount - 1;
			var index = parent.GetChildAdapterPosition(view);
			var lastSpanIndex = childCount - (_spanCount - childCount % _spanCount);

			if (_orientation == ItemsLayoutOrientation.Vertical)
			{
				outRect.Left = index < _spanCount ? 0 : (int)_adjustedHorizontalOffset;
				if (_spanCount > 1)
					outRect.Right = index < lastSpanIndex ? (int)_adjustedHorizontalOffset : 0;
				else
					outRect.Right = index < childCount ? (int)_adjustedHorizontalOffset : 0;
				outRect.Top = spanIndex == 0 ? 0 : (int)_adjustedVerticalOffset;
				outRect.Bottom = spanIndex == _spanCount - 1 ? 0 : (int)_adjustedVerticalOffset;
			}

			if (_orientation == ItemsLayoutOrientation.Horizontal)
			{
				outRect.Left = spanIndex == 0 ? 0 : (int)_adjustedHorizontalOffset;
				outRect.Right = spanIndex == _spanCount - 1 ? 0 : (int)_adjustedHorizontalOffset;
				outRect.Top = index < _spanCount ? 0 : (int)_adjustedVerticalOffset;
				if (_spanCount > 1)
					outRect.Bottom = index < lastSpanIndex ? (int)_adjustedVerticalOffset : 0;
				else
					outRect.Bottom = index < childCount ? (int)_adjustedVerticalOffset : 0;
			}
		}
	}
}