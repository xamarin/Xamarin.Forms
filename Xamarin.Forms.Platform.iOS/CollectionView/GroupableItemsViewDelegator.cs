using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class GroupableItemsViewDelegator<TItemsView, TViewController> : SelectableItemsViewDelegator<TItemsView, TViewController>
		where TItemsView : GroupableItemsView
		where TViewController : GroupableItemsViewController<TItemsView>
	{
		public GroupableItemsViewDelegator(ItemsViewLayout itemsViewLayout, TViewController itemsViewController, IItemsViewSource itemsSource) 
			: base(itemsViewLayout, itemsViewController, itemsSource)
		{
		}

		// we have to override this method to guarantee that we return the default implementation
		// TODO: implement dynamic sizing issue also for the the collection view when it is used as a grid
		public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			return ItemsViewLayout.EstimatedItemSize;
		}

		public override CGSize GetReferenceSizeForHeader(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return ViewController.GetReferenceSizeForHeader(collectionView, layout, section);
		}

		public override CGSize GetReferenceSizeForFooter(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return ViewController.GetReferenceSizeForFooter(collectionView, layout, section);
		}

		public override void ScrollAnimationEnded(UIScrollView scrollView)
		{
			ViewController?.HandleScrollAnimationEnded();
		}

		public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default;
			}

			return ViewController.GetInsetForSection(ItemsViewLayout, collectionView, section);
		}
	}
}