namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewRenderer : CollectionViewRenderer
	{
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

			// Fall back to horizontal list
			return new CarouselViewLayout(new ListItemsLayout(ItemsLayoutOrientation.Horizontal));
		}
	}
}
