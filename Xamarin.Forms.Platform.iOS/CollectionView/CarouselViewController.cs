using System;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewController : ItemsViewController
	{
		CarouselView _carouselView;
		nfloat _previousOffSetX;
		nfloat _previousOffSetY;
		public CarouselViewController(CarouselView itemsView, ItemsViewLayout layout)
		: base(itemsView, layout)
		{
			_carouselView = itemsView;
			Delegator.CarouselViewController = this;
		}

		public override void DecelerationEnded(UIScrollView scrollView)
		{
			var cell = CollectionView.VisibleCells;

			//TODO: Handle default cell 
			var formsCell = cell[0] as TemplatedCell;

			var context = formsCell?.VisualElementRenderer?.Element?.BindingContext;

			if (context == null)
				throw new InvalidOperationException("Visible item not found");

			_carouselView.SetCurrentItem(context);
		}

		public override void Scrolled(UIScrollView scrollView)
		{
			//TODO: How to handle the inertial values it would be easy for negative values but not for overscroll 
			var newOffSetX = scrollView.ContentOffset.X;
			var newOffSetY = scrollView.ContentOffset.Y;
			//TODO: rmarinho Handle RTL
			_carouselView.SendScrolled(scrollView.ContentOffset.X, (_previousOffSetX > newOffSetX) ? ScrollDirection.Left : ScrollDirection.Right);
			_previousOffSetX = newOffSetX;
			_previousOffSetY = newOffSetY;
		}

	}
}
