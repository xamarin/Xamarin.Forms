using System;
using System.Linq;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewController : ItemsViewController
	{
		CarouselView _carouselView;
		ItemsViewLayout _layout;
		nfloat _previousOffSetX;
		nfloat _previousOffSetY;
		public CarouselViewController(CarouselView itemsView, ItemsViewLayout layout)
		: base(itemsView, layout)
		{
			_carouselView = itemsView;
			_layout = layout;
			Delegator.CarouselViewController = this;
		}

		public override void DecelerationEnded(UIScrollView scrollView)
		{
			//TODO: Handle default cell 
			TemplatedCell formsCell = FindCenteredCell();

			var context = formsCell?.VisualElementRenderer?.Element?.BindingContext;

			if (context != null)
				_carouselView.SetCurrentItem(context);

		}

		public override void Scrolled(UIScrollView scrollView)
		{
			//TODO: How to handle the inertial values it would be easy for negative values but not for overscroll 
			var newOffSetX = scrollView.ContentOffset.X;
			var newOffSetY = scrollView.ContentOffset.Y;
			//TODO: rmarinho Handle RTL
			if (_layout.ScrollDirection == UICollectionViewScrollDirection.Horizontal)
				_carouselView.SendScrolled(scrollView.ContentOffset.X, (_previousOffSetX > newOffSetX) ? ScrollDirection.Left : ScrollDirection.Right);
			else
				_carouselView.SendScrolled(scrollView.ContentOffset.Y, (_previousOffSetY > newOffSetY) ? ScrollDirection.Up : ScrollDirection.Down);

			_previousOffSetX = newOffSetX;
			_previousOffSetY = newOffSetY;
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
