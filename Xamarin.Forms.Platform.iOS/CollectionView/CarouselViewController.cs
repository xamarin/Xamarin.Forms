using System;
using System.Linq;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewController : ItemsViewController
	{
		ICarouselViewController CarouselController => _carouselView as ICarouselViewController;
		CarouselView _carouselView;
		ItemsViewLayout _layout;
		nfloat _previousOffSetX;
		nfloat _previousOffSetY;

		public CarouselViewController(CarouselView itemsView, ItemsViewLayout layout) : base(itemsView, layout)
		{
			_carouselView = itemsView;
			_carouselView.ScrollToRequested += ScrollToRequested;
			_layout = layout;
			Delegator.CarouselViewController = this;
		}

		internal void TeardDown()
		{
			_carouselView.ScrollToRequested -= ScrollToRequested;
		}

		public override void DecelerationStarted(UIScrollView scrollView)
		{

		}

		public override void DraggingStarted(UIScrollView scrollView)
		{
			CarouselController.SetIsDragging(true);
		}

		public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
		{
			CarouselController.SetIsDragging(false);
		}

		public override void ScrollAnimationEnded(UIScrollView scrollView)
		{
			CarouselController.SetIsScrolling(false);
		}

		object _currentItem;
		NSIndexPath _currentItemIdex;
		public override void DecelerationEnded(UIScrollView scrollView)
		{
			//TODO: Handle default cell 
			TemplatedCell formsCell = FindCenteredCell();

			var context = formsCell?.VisualElementRenderer?.Element?.BindingContext;

			_currentItem = context;

			_currentItemIdex = GetIndexForItem(_currentItem);
			if (context != null)
				CarouselController.SetCurrentItem(context);

		}

		public override void Scrolled(UIScrollView scrollView)
		{
			//TODO: How to handle the inertial values it would be easy for negative values but not for overscroll 
			var newOffSetX = scrollView.ContentOffset.X;
			var newOffSetY = scrollView.ContentOffset.Y;
			//TODO: rmarinho Handle RTL
			if (_layout.ScrollDirection == UICollectionViewScrollDirection.Horizontal)
				CarouselController.SendScrolled(scrollView.ContentOffset.X, (_previousOffSetX > newOffSetX) ? ScrollDirection.Left : ScrollDirection.Right);
			else
				CarouselController.SendScrolled(scrollView.ContentOffset.Y, (_previousOffSetY > newOffSetY) ? ScrollDirection.Up : ScrollDirection.Down);

			_previousOffSetX = newOffSetX;
			_previousOffSetY = newOffSetY;
		}


		void ScrollToRequested(object sender, ScrollToRequestEventArgs e)
		{
			//We are ending dragging and position is being update
			if (e.Item == _currentItem || e.Index == _currentItemIdex.Row)
				return;
			CarouselController.SetIsScrolling(true);
		}

		TemplatedCell FindCenteredCell()
		{
			var cells = CollectionView.VisibleCells;

			var x = CollectionView.Center.X + CollectionView.ContentOffset.X;
			var y = CollectionView.Center.Y + CollectionView.ContentOffset.Y;

			var formsCell = cells.FirstOrDefault(c => c.Center.X == x && c.Center.Y == y) as TemplatedCell;

			return formsCell;
		}

	}
}
