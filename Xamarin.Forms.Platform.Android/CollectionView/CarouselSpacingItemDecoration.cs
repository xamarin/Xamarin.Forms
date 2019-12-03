using System;
using Android.Graphics;
using Android.Support.V7.Widget;
using AView = Android.Views.View;
using FormsCarouselView = Xamarin.Forms.CarouselView;

namespace Xamarin.Forms.Platform.Android
{
	internal class CarouselSpacingItemDecoration : RecyclerView.ItemDecoration
	{
		readonly FormsCarouselView _carouselView;
		readonly ItemsLayoutOrientation _orientation;
		readonly double _verticalSpacing;
		double _adjustedVerticalSpacing = -1;
		readonly double _horizontalSpacing;
		double _adjustedHorizontalSpacing = -1;

		public CarouselSpacingItemDecoration(IItemsLayout itemsLayout, FormsCarouselView carouselView)
		{
			var layout = itemsLayout ?? throw new ArgumentNullException(nameof(itemsLayout));
		
			switch (layout)
			{
				case GridItemsLayout gridItemsLayout:
					_orientation = gridItemsLayout.Orientation;
					_horizontalSpacing = gridItemsLayout.HorizontalItemSpacing;
					_verticalSpacing = gridItemsLayout.VerticalItemSpacing;
					break;
				case LinearItemsLayout listItemsLayout:
					_orientation = listItemsLayout.Orientation;
					if (_orientation == ItemsLayoutOrientation.Horizontal)
						_horizontalSpacing = listItemsLayout.ItemSpacing;
					else
						_verticalSpacing = listItemsLayout.ItemSpacing;
					break;
			}

			_carouselView = carouselView;
		}

		public override void GetItemOffsets(Rect outRect, AView view, RecyclerView parent, RecyclerView.State state)
		{
			base.GetItemOffsets(outRect, view, parent, state);

			if (Math.Abs(_adjustedVerticalSpacing - (-1)) < double.Epsilon)
			{
				_adjustedVerticalSpacing = parent.Context.ToPixels(_verticalSpacing);
			}

			if (Math.Abs(_adjustedHorizontalSpacing - (-1)) < double.Epsilon)
			{
				_adjustedHorizontalSpacing = parent.Context.ToPixels(_horizontalSpacing);
			}

			int position = parent.GetChildAdapterPosition(view);
			int itemCount = state.ItemCount;

			if (position == RecyclerView.NoPosition || itemCount == 0)
				return;

			if (_orientation == ItemsLayoutOrientation.Vertical)
			{
				var bottomPeekAreaInsets = parent.Context.ToPixels(_carouselView.PeekAreaInsets.Bottom);
				var topPeekAreaInsets = parent.Context.ToPixels(_carouselView.PeekAreaInsets.Top);

				var finalVerticalSpacing = (int)(_adjustedVerticalSpacing - (_verticalSpacing * 2));

				outRect.Left = position == 0 ? 0 : (int)_adjustedHorizontalSpacing;
				outRect.Bottom = (position == (itemCount - 1) && bottomPeekAreaInsets > 0) ? ((int)Math.Ceiling(bottomPeekAreaInsets / 2) + finalVerticalSpacing) : finalVerticalSpacing;
				outRect.Top = (position == 0 && topPeekAreaInsets > 0) ? ((int)Math.Ceiling(topPeekAreaInsets / 2) + finalVerticalSpacing) : finalVerticalSpacing;
			}

			if (_orientation == ItemsLayoutOrientation.Horizontal)
			{
				var leftPeekAreaInsets = parent.Context.ToPixels(_carouselView.PeekAreaInsets.Left);
				var rightPeekAreaInsets = parent.Context.ToPixels(_carouselView.PeekAreaInsets.Right);

				var finalHorizontalSpacing = (int)(_adjustedHorizontalSpacing - (_horizontalSpacing * 2));

				outRect.Top = position == 0 ? 0 : (int)_adjustedVerticalSpacing;
				outRect.Right = (position == (itemCount - 1) && rightPeekAreaInsets > 0) ? ((int)Math.Ceiling(rightPeekAreaInsets / 2) + finalHorizontalSpacing) : finalHorizontalSpacing;
				outRect.Left = (position == 0 && leftPeekAreaInsets > 0) ? ((int)Math.Ceiling(leftPeekAreaInsets / 2) + finalHorizontalSpacing) : finalHorizontalSpacing;
			}
		}
	}
}