using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class ItemsViewLayout : UICollectionViewFlowLayout, IUICollectionViewDelegateFlowLayout
	{
		readonly ItemsLayout _itemsLayout;
		bool _determiningCellSize;
		bool _disposed;

		protected ItemsViewLayout(ItemsLayout itemsLayout)
		{
			_itemsLayout = itemsLayout;
			_itemsLayout.PropertyChanged += LayoutOnPropertyChanged;

			var scrollDirection = itemsLayout.Orientation == ItemsLayoutOrientation.Horizontal
				? UICollectionViewScrollDirection.Horizontal
				: UICollectionViewScrollDirection.Vertical;

			Initialize(scrollDirection);
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				if (_itemsLayout != null)
				{
					_itemsLayout.PropertyChanged += LayoutOnPropertyChanged;
				}
			}

			base.Dispose(disposing);
		}

		void LayoutOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChanged)
		{
			HandlePropertyChanged(propertyChanged);
		}

		protected virtual void HandlePropertyChanged(PropertyChangedEventArgs  propertyChanged)
		{
			// Nothing to do here for now; may need something here when we implement Snapping
		}

		public nfloat ConstrainedDimension { get; set; }

		public Func<UICollectionViewCell> GetPrototype { get; set; }

		// TODO hartez 2018/09/14 17:24:22 Long term, this needs to use the ItemSizingStrategy enum and not be locked into bool	
		public bool UniformSize { get; set; }

		public abstract void ConstrainTo(CGSize size);

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

		public void PrepareCellForLayout(ItemsViewCell cell)
		{
			if (_determiningCellSize)
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

		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			var shouldInvalidate = base.ShouldInvalidateLayoutForBoundsChange(newBounds);

			if (shouldInvalidate)
			{
				UpdateConstraints(newBounds.Size);
			}

			return shouldInvalidate;
		}

		protected void DetermineCellSize()
		{
			if (GetPrototype == null)
			{
				return;
			}

			_determiningCellSize = true;

			if (!(GetPrototype() is ItemsViewCell prototype))
			{
				return;
			}

			prototype.Constrain(ConstrainedDimension);

			var measure = prototype.Measure();

			if (UniformSize)
			{
				ItemSize = measure;
			}
			else
			{
				EstimatedItemSize = measure;
			}

			_determiningCellSize = false;
		}

		bool ConstraintsMatchScrollDirection(CGSize size)
		{
			if (ScrollDirection == UICollectionViewScrollDirection.Vertical)
			{
				return ConstrainedDimension == size.Width;
			}

			return ConstrainedDimension == size.Height;
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
				if (cells[n] is ItemsViewCell constrainedCell)
				{
					PrepareCellForLayout(constrainedCell);
				}
			}
		}

		void UpdateConstraints(CGSize size)
		{
			if (ConstraintsMatchScrollDirection(size))
			{
				return;
			}

			ConstrainTo(size);
			UpdateCellConstraints();
		}
	}
}