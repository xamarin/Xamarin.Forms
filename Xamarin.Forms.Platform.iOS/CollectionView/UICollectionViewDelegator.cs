using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class UICollectionViewDelegator : UICollectionViewDelegateFlowLayout
	{
		float _previousHorizontalOffset, _previousVerticalOffset;

		public ItemsViewLayout ItemsViewLayout { get; private set; }
		public ItemsViewController ItemsViewController { get; private set; }
		public SelectableItemsViewController SelectableItemsViewController
		{
			get => ItemsViewController as SelectableItemsViewController;
		}

		public GroupableItemsViewController GroupableItemsViewController
		{
			get => ItemsViewController as GroupableItemsViewController;
		}

		public UICollectionViewDelegator(ItemsViewLayout itemsViewLayout, ItemsViewController itemsViewController)
		{
			ItemsViewLayout = itemsViewLayout;
			ItemsViewController = itemsViewController;
		}

		public override void DraggingStarted(UIScrollView scrollView)
		{
			_previousHorizontalOffset = (float)scrollView.ContentOffset.X;
			_previousVerticalOffset = (float)scrollView.ContentOffset.Y;
		}

		public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
		{
			_previousHorizontalOffset = 0;
			_previousVerticalOffset = 0;
		}

		public override void Scrolled(UIScrollView scrollView)
		{
			var indexPathsForVisibleItems = ItemsViewController.CollectionView.IndexPathsForVisibleItems.OrderBy(x => x.Row);
			var firstVisibleItemIndex = (int)indexPathsForVisibleItems.First().Item;
			var lastVisibleItemIndex = (int)indexPathsForVisibleItems.Last().Item;
			var scrolledEventArgs = new Core.Items.ScrolledEventArgs(scrollView.ContentOffset.X - _previousHorizontalOffset, scrollView.ContentOffset.Y - _previousVerticalOffset, scrollView.ContentOffset.X, scrollView.ContentOffset.Y, firstVisibleItemIndex, lastVisibleItemIndex);

			ItemsViewController.ItemsView.SendScrolled(scrolledEventArgs);

			_previousHorizontalOffset = (float)scrollView.ContentOffset.X;
			_previousVerticalOffset = (float)scrollView.ContentOffset.Y;

			switch (ItemsViewController.ItemsView.RemainingItemsThreshold)
			{
				case -1:
					return;
				case 0:
					if (lastVisibleItemIndex== ItemsViewController.ItemsViewSource.Count - 1)
						ItemsViewController.ItemsView.SendRemainingItemsThresholdReached();
					break;
				default:
					if (ItemsViewController.ItemsViewSource.Count - 1 - lastVisibleItemIndex <= ItemsViewController.ItemsView.RemainingItemsThreshold)
						ItemsViewController.ItemsView.SendRemainingItemsThresholdReached();
					break;
			}
		}

		public override void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath path)
		{
			ItemsViewLayout?.WillDisplayCell(collectionView, cell, path);
		}

		public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout,
			nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default(UIEdgeInsets);
			}

			return ItemsViewLayout.GetInsetForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default(nfloat);
			}

			return ItemsViewLayout.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default(nfloat);
			}

			return ItemsViewLayout.GetMinimumLineSpacingForSection(collectionView, layout, section);
		}

		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			SelectableItemsViewController?.ItemSelected(collectionView, indexPath);
		}

		public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			SelectableItemsViewController?.ItemDeselected(collectionView, indexPath);
		}

		public override void CellDisplayingEnded(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			ItemsViewController.PrepareCellForRemoval(cell);
		}

		public override CGSize GetReferenceSizeForHeader(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			if (GroupableItemsViewController == null)
			{
				return CGSize.Empty;
			}

			return GroupableItemsViewController.GetReferenceSizeForHeader(collectionView, layout, section);
		}

		public override CGSize GetReferenceSizeForFooter(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			if (GroupableItemsViewController == null)
			{
				return CGSize.Empty;
			}

			return GroupableItemsViewController.GetReferenceSizeForFooter(collectionView, layout, section);
		}
	}
}