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

		protected override void SetUpNewElement(ItemsView newElement)
		{
			if (newElement != null && !(newElement is CarouselView))
			{
				throw new ArgumentException($"{nameof(newElement)} must be of type {typeof(CarouselView).Name}");
			}

			base.SetUpNewElement(newElement);

			UpdateInitialPosition();
		}

		protected override void TearDownOldElement(ItemsView oldElement)
		{
			CarouselViewController?.TeardDown();
			base.TearDownOldElement(oldElement);
		}

		void UpdateInitialPosition()
		{
			//TODO:Find a better way, ViewDidLoad didn't work either
			Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
			{
				CarouselView.ScrollTo(CarouselView.Position, -1, ScrollToPosition.Center);
				return false;
			});
		}
	}
}
