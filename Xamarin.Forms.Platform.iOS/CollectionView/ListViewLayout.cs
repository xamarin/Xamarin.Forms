using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class ItemsViewLayout : UICollectionViewFlowLayout, IUICollectionViewDelegateFlowLayout
	{
		public abstract string CellReuseId { get; }

		public abstract Type CellType { get; }

		public nfloat ConstrainedDimension { get; set; }

		public abstract void ConstrainTo(CGSize size);

		public abstract void PrepareCellForLayout(DefaultCell cell);
	}

	internal class ListViewLayout : ItemsViewLayout
	{
		public ListViewLayout(UICollectionViewScrollDirection scrollDirection)
		{
			Initialize(scrollDirection);
		}

		public override string CellReuseId => ScrollDirection == UICollectionViewScrollDirection.Horizontal
			? DefaultHorizontalListCell.ReuseId
			: DefaultVerticalListCell.ReuseId;

		public override Type CellType => ScrollDirection == UICollectionViewScrollDirection.Horizontal
			? typeof(DefaultHorizontalListCell)
			: typeof(DefaultVerticalListCell);

		public override void ConstrainTo(CGSize size)
		{
			ConstrainedDimension =
				ScrollDirection == UICollectionViewScrollDirection.Vertical ? size.Width : size.Height;
			UpdateItemSizeEstimate(size);
		}

		public override void PrepareCellForLayout(DefaultCell cell)
		{
			cell.UpdateConstrainedDimension(ConstrainedDimension);
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
				if (cells[n] is DefaultCell defaultCell)
				{
					defaultCell.UpdateConstrainedDimension(ConstrainedDimension);
				}
			}
		}

		void UpdateConstraints(CGSize size)
		{
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
			// TODO hartez 2018/06/12 08:25:05 Determine if 64 is really correct here (it seems to work)
			// and if so, make it a const	
			if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				EstimatedItemSize = new CGSize(64, size.Height);
				return;
			}

			EstimatedItemSize = new CGSize(size.Width, 64);
		}
	}
}