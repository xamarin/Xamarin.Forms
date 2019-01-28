using System.ComponentModel;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewRenderer : ItemsViewRenderer
	{
		CarouselView CarouselView => (CarouselView)Element;

		CarouselViewController CarouselViewController => (CarouselViewController)ItemsViewController;

		protected override ItemsViewController CreateController(ItemsView newElement, ItemsViewLayout layout)
		{
			return new CarouselViewController(newElement as CarouselView, layout);
		}

		public CarouselViewRenderer()
		{
			CollectionView.VerifyCollectionViewFlagEnabled(nameof(CarouselViewRenderer));
		}

		protected override ItemsViewLayout SelectLayout(IItemsLayout layoutSpecification)
		{
			if (layoutSpecification is ListItemsLayout listItemsLayout)
			{
				return new CarouselViewLayout(listItemsLayout);
			}

			// Fall back to horizontal carousel
			return new CarouselViewLayout(new ListItemsLayout(ItemsLayoutOrientation.Horizontal));
		}
	}
}
