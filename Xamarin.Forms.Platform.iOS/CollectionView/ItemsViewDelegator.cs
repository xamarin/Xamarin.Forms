using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	public class ItemsViewDelegator<TItemsView, TViewController> : UICollectionViewDelegateFlowLayout
		where TItemsView : ItemsView
		where TViewController : ItemsViewController<TItemsView>
	{
		public ItemsViewLayout ItemsViewLayout { get; }
		public TViewController ViewController { get; }

		protected float PreviousHorizontalOffset, PreviousVerticalOffset;
		readonly Dictionary<DataTemplate, CGSize> _templateSizeEstimates = new Dictionary<DataTemplate, CGSize>();

		public ItemsViewDelegator(ItemsViewLayout itemsViewLayout, TViewController itemsViewController)
		{
			ItemsViewLayout = itemsViewLayout;
			ViewController = itemsViewController;
		}

		public override void Scrolled(UIScrollView scrollView)
		{
			var (visibleItems, firstVisibleItemIndex, centerItemIndex, lastVisibleItemIndex) = GetVisibleItemsIndex();

			if (!visibleItems)
				return;

			var contentInset = scrollView.ContentInset;
			var contentOffsetX = scrollView.ContentOffset.X + contentInset.Left;
			var contentOffsetY = scrollView.ContentOffset.Y + contentInset.Top;

			var itemsViewScrolledEventArgs = new ItemsViewScrolledEventArgs
			{
				HorizontalDelta = contentOffsetX - PreviousHorizontalOffset,
				VerticalDelta = contentOffsetY - PreviousVerticalOffset,
				HorizontalOffset = contentOffsetX,
				VerticalOffset = contentOffsetY,
				FirstVisibleItemIndex = firstVisibleItemIndex,
				CenterItemIndex = centerItemIndex,
				LastVisibleItemIndex = lastVisibleItemIndex
			};

			var itemsView = ViewController.ItemsView;
			var source = ViewController.ItemsSource;
			itemsView.SendScrolled(itemsViewScrolledEventArgs);

			PreviousHorizontalOffset = (float)contentOffsetX;
			PreviousVerticalOffset = (float)contentOffsetY;

			switch (itemsView.RemainingItemsThreshold)
			{
				case -1:
					return;
				case 0:
					if (lastVisibleItemIndex == source.ItemCount - 1)
						itemsView.SendRemainingItemsThresholdReached();
					break;
				default:
					if (source.ItemCount - 1 - lastVisibleItemIndex <= itemsView.RemainingItemsThreshold)
						itemsView.SendRemainingItemsThresholdReached();
					break;
			}
		}

		public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout,
			nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default;
			}

			return ItemsViewLayout.GetInsetForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default;
			}

			return ItemsViewLayout.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default;
			}

			return ItemsViewLayout.GetMinimumLineSpacingForSection(collectionView, layout, section);
		}

		public override void CellDisplayingEnded(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			if (ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				var actualWidth = collectionView.ContentSize.Width - collectionView.Bounds.Size.Width;
				if (collectionView.ContentOffset.X >= actualWidth || collectionView.ContentOffset.X < 0)
					return;
			}
			else
			{
				var actualHeight = collectionView.ContentSize.Height - collectionView.Bounds.Size.Height;

				if (collectionView.ContentOffset.Y >= actualHeight || collectionView.ContentOffset.Y < 0)
					return;
			}
		}

		protected virtual (bool VisibleItems, NSIndexPath First, NSIndexPath Center, NSIndexPath Last) GetVisibleItemsIndexPath()
		{
			var indexPathsForVisibleItems = ViewController.CollectionView.IndexPathsForVisibleItems.OrderBy(x => x.Row).ToList();

			var visibleItems = indexPathsForVisibleItems.Count > 0;
			NSIndexPath firstVisibleItemIndex = null, centerItemIndex = null, lastVisibleItemIndex = null;

			if (visibleItems)
			{
				firstVisibleItemIndex = indexPathsForVisibleItems.First();
				centerItemIndex = GetCenteredIndexPath(ViewController.CollectionView);
				lastVisibleItemIndex = indexPathsForVisibleItems.Last();
			}

			return (visibleItems, firstVisibleItemIndex, centerItemIndex, lastVisibleItemIndex);
		}

		protected virtual (bool VisibleItems, int First, int Center, int Last) GetVisibleItemsIndex()
		{
			var (VisibleItems, First, Center, Last) = GetVisibleItemsIndexPath();
			int firstVisibleItemIndex = -1, centerItemIndex = -1, lastVisibleItemIndex = -1;
			if (VisibleItems)
			{
				firstVisibleItemIndex = (int)First.Item;
				centerItemIndex = (int)Center.Item;
				lastVisibleItemIndex = (int)Last.Item;
			}
			return (VisibleItems, firstVisibleItemIndex, centerItemIndex, lastVisibleItemIndex);
		}

		static NSIndexPath GetCenteredIndexPath(UICollectionView collectionView)
		{
			NSIndexPath centerItemIndex = null;

			var indexPathsForVisibleItems = collectionView.IndexPathsForVisibleItems.OrderBy(x => x.Row).ToList();

			if (indexPathsForVisibleItems.Count == 0)
				return centerItemIndex;

			var firstVisibleItemIndex = indexPathsForVisibleItems.First();

			var centerPoint = new CGPoint(collectionView.Center.X + collectionView.ContentOffset.X, collectionView.Center.Y + collectionView.ContentOffset.Y);
			var centerIndexPath = collectionView.IndexPathForItemAtPoint(centerPoint);
			centerItemIndex = centerIndexPath ?? firstVisibleItemIndex;
			return centerItemIndex;
		}

		// Note: we are deliberately avoiding calculating the exact size of every item that the UICollectionViewFlowLayout
		// requests from us; instead, we use the exact ItemSize (when possible) or the EstimatedItemSize.
		// UICollectionViewFlowLayout will request the size for _every single item_ in our datasource, even if it's not going
		// to be on screen yet. For small datasources, realizing the Forms content and measuring it is no problem. 
		// But for large datasets (hundreds or thousands of items), we'd be realizing a Forms datatemplate and binding it for 
		// every single item, which defeats virtualization almost entirely. 
		// So we only create a measurement cell and measure it in this method if we don't already have a cached estimate for 
		// that item's data template. 

		public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			if (ItemsViewLayout.EstimatedItemSize.IsEmpty)
			{
				return ItemsViewLayout.ItemSize;
			}

			var itemTemplate = ViewController.ItemsView.ItemTemplate;

			if (!(itemTemplate is DataTemplateSelector dataTemplateSelector))
			{
				// If the DataTemplate only maps to a single template, then our original size estimate will be fine
				return ItemsViewLayout.EstimatedItemSize;
			}

			// Determine the template type for the current item 
			var targetTemplate = dataTemplateSelector.SelectDataTemplate(ViewController.ItemsSource[indexPath], ViewController.ItemsView);

			if (_templateSizeEstimates.TryGetValue(targetTemplate, out CGSize templateSizeEstimate))
			{
				// We've seen this template before; use the cached estimate
				return templateSizeEstimate;
			}
		
			var measurementCell = ViewController.CreateMeasurementCell(indexPath);

			if (measurementCell == null)
			{
				// If we couldn't get a measurement cell for some reason, fall back to the old estimate
				return ItemsViewLayout.EstimatedItemSize;
			}

			// Measure the cell and cache the result as our estimate for this template
			var size = measurementCell.Measure();
			_templateSizeEstimates[measurementCell.CurrentTemplate] = size;

			return size;
		}
	}
}