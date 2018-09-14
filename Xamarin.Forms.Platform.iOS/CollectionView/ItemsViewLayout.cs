using System;
using System.Runtime.CompilerServices;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class ItemsViewLayout : UICollectionViewFlowLayout, IUICollectionViewDelegateFlowLayout
	{
		public nfloat ConstrainedDimension { get; set; }
		
		public bool RequestingEstimate { get; private set; } = true;

		public abstract void ConstrainTo(CGSize size);

		protected ItemsViewLayout(UICollectionViewScrollDirection scrollDirection)
		{
			Initialize(scrollDirection);
		}

		void Initialize(UICollectionViewScrollDirection scrollDirection)
		{
			ScrollDirection = scrollDirection;
		}

		[Export("collectionView:layout:insetForSectionAtIndex:")]
		[CompilerGenerated]
		public virtual UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout,
			int section)
		{
			return UIEdgeInsets.Zero;
		}

		[Export("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
		[CompilerGenerated]
		public virtual nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, int section)
		{
			return (nfloat)0.0;
		}

		[Export("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
		[CompilerGenerated]
		public virtual nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, int section)
		{
			return (nfloat)0.0;
		}

		public void PrepareCellForLayout(IConstrainedCell cell)
		{
			if (RequestingEstimate)
			{
				return;
			}

			if (EstimatedItemSize == CGSize.Empty)
			{
				cell.Constrain(ItemSize);
			}
			else
			{
				cell.Constrain(ConstrainedDimension);
			}
		}

		public void SetEstimate(CGSize cellSize, bool uniformSize)
		{
			// TODO hartez 2018/09/14 17:03:07 If this is still a property, just use the property and drop the parameter	
			if (uniformSize)
			{
				ItemSize = cellSize;
			}
			else
			{
				EstimatedItemSize = cellSize;
			}

			RequestingEstimate = false;
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

		protected virtual void OnNeedsEstimate()
		{
			RequestingEstimate = true;
			DetermineCellSize(ConstrainedDimension);
		}

		void UpdateCellConstraints()
		{
			var cells = CollectionView.VisibleCells;

			for (int n = 0; n < cells.Length; n++)
			{
				if (cells[n] is IConstrainedCell constrainedCell)
				{
					PrepareCellForLayout(constrainedCell);
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

		public Func<UICollectionViewCell> GetPrototype { get; set; }

		public bool UniformSize { get; set; }

		void DetermineCellSize(nfloat layoutConstrainedDimension)
		{
			if (GetPrototype == null)
			{
				return;
			}

			if (!(GetPrototype() is IConstrainedCell prototype))
			{
				return;
			}

			prototype.Constrain(layoutConstrainedDimension);

			var measure = prototype.Measure();
			SetEstimate(measure, UniformSize);
		}
	}
}