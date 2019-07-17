using System;
using System.ComponentModel;

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

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

			if (changedProperty.Is(CarouselView.PeekAreaInsetsProperty))
			{
				(CarouselViewController.Layout as CarouselViewLayout).UpdateConstraints(Frame.Size);
				CarouselViewController.Layout.InvalidateLayout();
			}
		}

		protected override ItemsViewLayout SelectLayout(IItemsLayout layoutSpecification, ItemSizingStrategy itemSizingStrategy)
		{
			if (layoutSpecification is ListItemsLayout listItemsLayout)
			{
				return new CarouselViewLayout(listItemsLayout, itemSizingStrategy, CarouselView);
			}

			// Fall back to horizontal carousel
			return new CarouselViewLayout(new ListItemsLayout(ItemsLayoutOrientation.Horizontal), itemSizingStrategy, CarouselView);
		}

		protected override void TearDownOldElement(ItemsView oldElement)
		{
			CarouselViewController?.TearDown();
			base.TearDownOldElement(oldElement);
		}
	}
}
