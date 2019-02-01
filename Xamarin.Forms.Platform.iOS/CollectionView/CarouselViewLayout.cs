using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class CarouselViewLayout : ItemsViewLayout
	{
		CarouselView _carouselView;

		public CarouselViewLayout(ListItemsLayout itemsLayout, CarouselView carouselView) : base(itemsLayout)
		{
			_carouselView = carouselView;
		}

		public override void ConstrainTo(CGSize size)
		{
			//TODO: Should we scale the items 
			var aspectRation = size.Width / size.Height;

			var width = (size.Width - _carouselView.Padding.Left - _carouselView.Padding.Right ) / _carouselView.NumberOfVisibleItems;
			var height = size.Height / _carouselView.NumberOfVisibleItems;

			if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				ItemSize = new CGSize(width, size.Height);
			}
			else
			{
				ItemSize = new CGSize(size.Width, height);
			}
		}

		public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return base.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
		}

		public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			var insets = base.GetInsetForSection(collectionView, layout, section);
			if (_carouselView.NumberOfVisibleItems == 1)
			{
				return insets;
			}

			//We give some insets so the user can be able to scroll to the first and last item
			if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				return new UIEdgeInsets(insets.Top, ItemSize.Width, insets.Bottom, ItemSize.Width);
			}
			else
			{
				return new UIEdgeInsets(ItemSize.Height, insets.Left, ItemSize.Height, insets.Right);
			}

		}
	}
}
