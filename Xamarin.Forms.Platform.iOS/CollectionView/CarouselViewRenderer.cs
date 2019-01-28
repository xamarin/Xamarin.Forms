using System.ComponentModel;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewRenderer : ItemsViewRenderer
	{
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

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

			if (changedProperty.Is(CarouselView.SelectedItemProperty))
			{
				UpdateNativeSelection();
			}
		}

		void UpdateNativeSelection()
		{
			var selectedItem = (Element as CarouselView).SelectedItem;

			if (selectedItem == null)
			{
				//if we set to null just do nothing
				return;
			}

			MarkItemSelected(selectedItem);
		}

		void MarkItemSelected(object selectedItem)
		{

		}
	}
}
