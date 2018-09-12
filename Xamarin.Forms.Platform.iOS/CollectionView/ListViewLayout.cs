using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class ListViewLayout : ItemsViewLayout
	{
		public ListViewLayout(UICollectionViewScrollDirection scrollDirection)
		{
			Initialize(scrollDirection);
		}

		public override void ConstrainTo(CGSize size)
		{
			ConstrainedDimension =
				ScrollDirection == UICollectionViewScrollDirection.Vertical ? size.Width : size.Height;
			UpdateItemSizeEstimate(size);
		}

		public override void PrepareCellForLayout(IConstrainedCell cell)
		{
			if (EstimatedItemSize == CGSize.Empty)
			{
				cell.Constrain(ItemSize);
			}
			else
			{
				cell.Constrain(ConstrainedDimension);
			}
		}

		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			var shouldInvalidate = base.ShouldInvalidateLayoutForBoundsChange(newBounds);

			if (shouldInvalidate)
			{
				UpdateConstraints(newBounds.Size);
			}

			return shouldInvalidate;
		}

		void Initialize(UICollectionViewScrollDirection scrollDirection)
		{
			ScrollDirection = scrollDirection;
		}

		void UpdateCellConstraints()
		{
			var cells = CollectionView.VisibleCells;

			for (int n = 0; n < cells.Length; n++)
			{
				if (cells[n] is IConstrainedCell constrainedCell)
				{
					{
						PrepareCellForLayout(constrainedCell);
					}
				}
			}
		}

		void UpdateConstraints(CGSize size)
		{
			// TODO hartez 2018/09/12 13:01:02 De-duplicate the code here	
			if (ScrollDirection == UICollectionViewScrollDirection.Vertical
				&& ConstrainedDimension != size.Width)
			{
				ConstrainTo(size);
				UpdateCellConstraints();
			}
			else if (ScrollDirection == UICollectionViewScrollDirection.Horizontal
					&& ConstrainedDimension != size.Height)
			{
				ConstrainTo(size);
				UpdateCellConstraints();
			}
		}

		void UpdateItemSizeEstimate(CGSize size)
		{
			// TODO hartez 2018/09/10 19:13:48 We should probably have this update after layouts for template cells
			// And the default cells should also report back, maybe using their Labels' intrinsic content sizes
			if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				EstimatedItemSize = new CGSize(64, size.Height);
				return;
			}

			EstimatedItemSize = new CGSize(size.Width, 64);
		}
	}
}