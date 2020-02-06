using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class CarouselUICollectionView : UICollectionView
	{
		readonly CarouselViewDataSource _carouselViewDataSource;
		readonly bool _isHorizontal;
		nfloat _cellPadding = 0.0f;
		nfloat _cellWidth = 0.0f;
		nfloat _cellHeight = 0.0f;
		bool _isSized;
		
		public CarouselUICollectionView(CGRect frame, ItemsViewLayout layout, CarouselViewDataSource carouselViewDataSource)
			: base(frame, layout)
		{
			DataSource = _carouselViewDataSource = carouselViewDataSource;
			AllowsSelection = false;
			AllowsMultipleSelection = false;
			_isHorizontal = layout.ScrollDirection == UICollectionViewScrollDirection.Horizontal;
		}

		internal void SetupCellDimensions()
		{
			var layout = CollectionViewLayout as UICollectionViewFlowLayout;
			_cellPadding = 0;
			_cellWidth = layout.ItemSize.Width;
			_cellHeight = layout.ItemSize.Height;
			_isSized = true;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (!_isSized)
				return;

			if (_isHorizontal)
				CentreHorizontalIfNeeded();
			else
				CentreVerticallyIfNeeded();
		}

		void CentreVerticallyIfNeeded()
		{
			var currentOffset = ContentOffset;

			var contentHeight = GetTotalContentHeight();

			var centerOffsetY = (3 * contentHeight) / 2;

			var distFromCentre = centerOffsetY - currentOffset.Y;

			if (Math.Abs(distFromCentre) > (contentHeight / 4))
			{
				var cellcount = distFromCentre / (_cellHeight + _cellPadding);

				var shiftCells = (int)((cellcount > 0) ? Math.Floor(cellcount) : Math.Ceiling(cellcount));

				var offsetCorrection = (Math.Abs(cellcount) % 1.0) * (_cellHeight + _cellPadding);

				if (ContentOffset.Y < centerOffsetY)
				{
					ContentOffset = new CGPoint(currentOffset.X, centerOffsetY - offsetCorrection);

				}
				else if (ContentOffset.Y > centerOffsetY)
				{
					ContentOffset = new CGPoint(currentOffset.X, centerOffsetY + offsetCorrection);
				}

				_carouselViewDataSource.ShiftContentArray(shiftCells);

				ReloadData();
			}
		}

		void CentreHorizontalIfNeeded()
		{
			var currentOffset = ContentOffset;

			nfloat contentWidth = GetTotalContentWidth();

			var centerOffsetX = (3 * contentWidth - Bounds.Size.Width) / 2;

			var distFromCentre = centerOffsetX - currentOffset.X;

			if (Math.Abs(distFromCentre) > (contentWidth / 4))
			{
				var cellcount = distFromCentre / (_cellWidth + _cellPadding);

				var shiftCells = (int)((cellcount > 0) ? Math.Floor(cellcount) : Math.Ceiling(cellcount));

				var offsetCorrection = (Math.Abs(cellcount % 1.0f)) * (_cellWidth + _cellPadding);

				if (ContentOffset.X < centerOffsetX)
				{
					ContentOffset = new CGPoint(centerOffsetX - offsetCorrection, currentOffset.Y);
				}
				else if (ContentOffset.X > centerOffsetX)
				{
					ContentOffset = new CGPoint(centerOffsetX + offsetCorrection, currentOffset.Y);
				}

				_carouselViewDataSource.ShiftContentArray(shiftCells);

				ReloadData();
			}
		}

		nfloat GetTotalContentWidth()
		{
			var numberOfCells = _carouselViewDataSource.GetItemsSourceCount();
			return numberOfCells * (_cellWidth + _cellPadding);
		}

		nfloat GetTotalContentHeight()
		{
			var numberOfCells = _carouselViewDataSource.GetItemsSourceCount();
			return ((numberOfCells) * (_cellHeight + _cellPadding)) - _cellPadding;
		}
	}
}