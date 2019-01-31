using System;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewRenderer : ItemsViewRenderer
	{
		CarouselView CarouselView => (CarouselView)Element;

		CarouselViewController CarouselViewController => (CarouselViewController)ItemsViewController;

		public CarouselViewRenderer()
		{
			CollectionView.VerifyCollectionViewFlagEnabled(nameof(CarouselViewRenderer));
		}

		protected override ItemsViewController CreateController(ItemsView newElement, ItemsViewLayout layout)
		{
			return new CarouselViewController(newElement as CarouselView, layout);
		}

		protected override ItemsViewLayout SelectLayout(IItemsLayout layoutSpecification)
		{
			if (layoutSpecification is ListItemsLayout listItemsLayout)
			{
				return new CarouselViewLayout(listItemsLayout, CarouselView);
			}

			// Fall back to horizontal carousel
			return new CarouselViewLayout(new ListItemsLayout(ItemsLayoutOrientation.Horizontal), CarouselView);
		}

		protected override void TearDownOldElement(ItemsView oldElement)
		{
			CarouselViewController?.TeardDown();
			base.TearDownOldElement(oldElement);
		}
	}
}
