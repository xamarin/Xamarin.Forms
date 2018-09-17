using System;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewRenderer
	{
	}

	// TODO hartez 2018/05/30 08:58:42 This follows the same basic scheme as RecyclerView.Adapter; you should be able to reuse the same wrapper class for the IEnumerable	
	//// TODO hartez 2018/05/30 09:05:38 Think about whether this Controller and/or the new Adapter should be internal or public
	public class CollectionViewRenderer : ViewRenderer<CollectionView, UIView>
	{
		CollectionViewController _collectionViewController;
		ItemsViewLayout _layout;
		bool _disposed;

		public override UIViewController ViewController => _collectionViewController;

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return Control.GetSizeRequest(widthConstraint, heightConstraint, 0, 0);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CollectionView> e)
		{
			TearDownOldElement(e.OldElement);
			SetUpNewElement(e.NewElement);
			
			base.OnElementChanged(e);
		}

		protected virtual ItemsViewLayout SelectLayout(IItemsLayout layoutSpecification)
		{
			if (layoutSpecification is GridItemsLayout gridItemsLayout)
			{
				return new GridViewLayout(gridItemsLayout);
			}

			if (layoutSpecification is ListItemsLayout listItemsLayout)
			{
				return new ListViewLayout(listItemsLayout);
			}

			// Fall back to vertical list
			return new ListViewLayout(new ListItemsLayout(ItemsLayoutOrientation.Vertical));
		}

		void TearDownOldElement(CollectionView oldElement)
		{
			if (oldElement == null)
			{
				return;
			}

			// Stop listening for ScrollTo requests
			oldElement.ScrollToRequested -= ScrollToRequested;
		}

		void SetUpNewElement(CollectionView newElement)
		{
			if (newElement == null)
			{
				return;
			}

			_layout = SelectLayout(newElement.ItemsLayout);
			_collectionViewController = new CollectionViewController(newElement, _layout);
			SetNativeControl(_collectionViewController.View);
			_collectionViewController.CollectionView.BackgroundColor = UIColor.Clear;

			// Listen for ScrollTo requests
			newElement.ScrollToRequested += ScrollToRequested;
		}

		void ScrollToRequested(object sender, ScrollToRequestEventArgs args)
		{
			if (args.Mode == ScrollToMode.Position)
			{
				// TODO hartez 2018/09/17 16:42:54 This will need to be overridden to account for grouping	

				var indexPath = NSIndexPath.Create(0, args.Index);

				// TODO hartez 2018/09/17 16:21:19 Handle LTR	

				_collectionViewController.CollectionView.ScrollToItem(indexPath, 
					args.ScrollToPosition.ToCollectionViewScrollPosition(_layout.ScrollDirection),
					args.Animate);
			}

			// TODO hartez 2018/09/17 16:21:38 Handle scrollTo Item	
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
				if (Element != null)
				{
					TearDownOldElement(Element);
				}
			}

			base.Dispose(disposing);
		}
	}

	public static class ScrollToPositionExtensions
	{
		public static UICollectionViewScrollPosition ToCollectionViewScrollPosition(this ScrollToPosition scrollToPosition, 
			UICollectionViewScrollDirection scrollDirection = UICollectionViewScrollDirection.Vertical, bool isLtr = false)
		{
			if (scrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				return scrollToPosition.ToHorizontalCollectionViewScrollPosition(isLtr);
			}

			return scrollToPosition.ToVerticalCollectionViewScrollPosition();
		}

		public static UICollectionViewScrollPosition ToHorizontalCollectionViewScrollPosition(this ScrollToPosition scrollToPosition, bool isLtr)
		{
			switch (scrollToPosition)
			{
				case ScrollToPosition.MakeVisible:
				case ScrollToPosition.Start:
					return isLtr ? UICollectionViewScrollPosition.Right : UICollectionViewScrollPosition.Left;
				case ScrollToPosition.End:
					return isLtr ? UICollectionViewScrollPosition.Left : UICollectionViewScrollPosition.Right;
				case ScrollToPosition.Center:
				default:
					return UICollectionViewScrollPosition.CenteredHorizontally;
			}
		}

		public static UICollectionViewScrollPosition ToVerticalCollectionViewScrollPosition(this ScrollToPosition scrollToPosition)
		{
			switch (scrollToPosition)
			{
				case ScrollToPosition.MakeVisible:
				case ScrollToPosition.Start:
					return UICollectionViewScrollPosition.Top;
				case ScrollToPosition.End:
					return UICollectionViewScrollPosition.Bottom;
				case ScrollToPosition.Center:
				default:
					return UICollectionViewScrollPosition.CenteredVertically;
			}
		}
	}
}