using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class ItemsViewLayout : UICollectionViewFlowLayout, IUICollectionViewDelegateFlowLayout
	{
		public abstract string CellReuseId { get; }

		public abstract Type CellType { get; }

		public abstract void UpdateItemSizeEstimate(CGSize size);

		public nfloat ConstrainedDimension { get; set; }

		public abstract void ConstrainTo(CGSize size);

		public abstract void ShrinkConstraints(CGSize size, UICollectionViewCell[] cells);

		public abstract void ForceConstraints(CGSize size, UICollectionViewCell[] cells);
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

		public override void UpdateItemSizeEstimate(CGSize size)
		{
			// TODO hartez 2018/06/12 08:25:05 Determine if 64 is really correct here (it seems to work) and if so, make it a const	
			if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				EstimatedItemSize = new CGSize(64, size.Height);
				return;
			}

			EstimatedItemSize = new CGSize(size.Width, 64);
		}

		void Initialize(UICollectionViewScrollDirection scrollDirection)
		{
			ScrollDirection = scrollDirection;
		}

		public override void ConstrainTo(CGSize size)
		{
			ConstrainedDimension = ScrollDirection == UICollectionViewScrollDirection.Vertical ? size.Width : size.Height;
		}

		void UpdateConstraints(CGSize size, UICollectionViewCell[] cells)
		{
			ConstrainTo(size);

			for (int n = 0; n < cells.Length; n++)
			{
				if (cells[n] is DefaultCell defaultCell)
				{
					defaultCell.UpdateConstrainedDimension(ConstrainedDimension);
				}
			}

			// TODO hartez 2018/06/12 08:38:18 Are these calls needed? If so, can we take them out if the constrainted dimension isn't changing?	
			InvalidateLayout();
			CollectionView.LayoutIfNeeded();
		}

		public override void ShrinkConstraints(CGSize size, UICollectionViewCell[] cells)
		{
			if (ScrollDirection == UICollectionViewScrollDirection.Vertical &&
				ConstrainedDimension > size.Width)
			{
				UpdateConstraints(size, CollectionView.VisibleCells);
				UpdateItemSizeEstimate(size);
			}
			else if (ScrollDirection == UICollectionViewScrollDirection.Horizontal &&
					ConstrainedDimension > size.Height)
			{
				UpdateConstraints(size, CollectionView.VisibleCells);
				UpdateItemSizeEstimate(size);
			}
		}

		public override void ForceConstraints(CGSize size, UICollectionViewCell[] cells)
		{
			UpdateItemSizeEstimate(size);

			if (ScrollDirection == UICollectionViewScrollDirection.Vertical
				&& ConstrainedDimension != size.Width)
			{
				UpdateConstraints(size, CollectionView.VisibleCells);
			}
			else if (ScrollDirection == UICollectionViewScrollDirection.Horizontal
					&& ConstrainedDimension != size.Height)
			{
				UpdateConstraints(size, CollectionView.VisibleCells);
			}
		}

		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			var x = base.ShouldInvalidateLayoutForBoundsChange(newBounds);

			// TODO hartez 2018/06/12 10:05:09 Catch whatever nonsense titlebar resize is happening on Back and make sure to update any height constraints for it so we don't straight-up crash	

			Debug.WriteLine($">>>>> ListViewLayout ShouldInvalidateLayoutForBoundsChange newBounds: {newBounds}, shouldInvalidate: {x}");
			Debug.WriteLine($">>>>> ListViewLayout ShouldInvalidateLayoutForBoundsChange ConstrainedDimension: {ConstrainedDimension}");

			return x;
		}
	}
}