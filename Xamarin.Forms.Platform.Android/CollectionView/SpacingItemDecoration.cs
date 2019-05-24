using System;
using Android.Graphics;
using Android.Support.V7.Widget;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	internal class SpacingItemDecoration : RecyclerView.ItemDecoration
	{
		ItemsLayoutOrientation _orientation;
		int _spacing;
		int _adjustedSpacing = -1;

		public SpacingItemDecoration(IItemsLayout itemsLayout)
		{
			if (itemsLayout == null)
			{
				throw new ArgumentNullException(nameof(itemsLayout));
			}

			switch (itemsLayout)
			{
				//case GridItemsLayout gridItemsLayout:
				//	return CreateGridLayout(gridItemsLayout);
				case ListItemsLayout listItemsLayout:
					_orientation = listItemsLayout.Orientation;
					_spacing =  listItemsLayout.ItemSpacing;
					break;
			}
		}

		public override void GetItemOffsets(Rect outRect, AView view, RecyclerView parent, RecyclerView.State state)
		{
			base.GetItemOffsets(outRect, view, parent, state);

			if (parent.GetChildAdapterPosition(view) == 0)
			{
				return;
			}

			if (_adjustedSpacing == -1)
			{
				_adjustedSpacing = (int)parent.Context.ToPixels(_spacing);
			}

			switch (_orientation)
			{
				case ItemsLayoutOrientation.Vertical:
					outRect.Top = _adjustedSpacing;
					break;
				case ItemsLayoutOrientation.Horizontal:
					outRect.Left = _adjustedSpacing;
					break;
			}
		}
	}
}