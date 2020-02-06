using System;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class CarouselViewDataSource : UICollectionViewDataSource
	{
		readonly ItemsView _itemsView;
		readonly ItemsViewLayout _itemsViewLayout;
		readonly IItemsViewSource _source;
		readonly string _reuseId;
		int _indexOffset = 0;
		
		public CarouselViewDataSource(ItemsView itemsView, ItemsViewLayout itemsViewLayout, IItemsViewSource source, string reuseId)
		{
			_itemsView = itemsView;
			_itemsViewLayout = itemsViewLayout;
			_source = source;
			_reuseId = reuseId;
		}

		public UICollectionViewCell CellForItemAtIndexPath(UICollectionView collectionView, NSIndexPath dequeueIndexPath, NSIndexPath usableIndexPath)
		{
			var cell = collectionView.DequeueReusableCell(_reuseId, dequeueIndexPath) as UICollectionViewCell;
			UpdateTemplatedCell(cell as TemplatedCell, usableIndexPath);
			return cell;
		}

		public void ShiftContentArray(int shiftCells)
		{
			var correctedIndex = GetCorrectedIndex(shiftCells);
			_indexOffset += correctedIndex;
		}

		protected virtual void UpdateTemplatedCell(TemplatedCell cell, NSIndexPath indexPath)
		{
			//cell.ContentSizeChanged -= CellContentSizeChanged;

			cell.Bind(_itemsView.ItemTemplate, _source[indexPath], _itemsView);

			//cell.ContentSizeChanged += CellContentSizeChanged;

			_itemsViewLayout.PrepareCellForLayout(cell);
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			return CellForItemAtIndexPath(collectionView, indexPath, NSIndexPath.FromRowSection(GetCorrectedIndex(indexPath.Row - _indexOffset), 0));
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			var itemsCount = GetItemsSourceCount();

			return 3 * itemsCount;
		}

		public int GetItemsSourceCount() => _source.ItemCountInGroup(0);

		public int GetCorrectedIndex(int indexToCorrect)
		{
			var itemsCount = GetItemsSourceCount();
			if (indexToCorrect < itemsCount && indexToCorrect >= 0)
				return indexToCorrect;

			var countInIndex = (double)(indexToCorrect / itemsCount);
			var flooredValue = (int)(Math.Floor(countInIndex));
			var offset = itemsCount * flooredValue;
			var newIndex = indexToCorrect - offset;
			if (newIndex < 0)
				return (itemsCount - Math.Abs(newIndex));
			return newIndex;
		}
	}
}
