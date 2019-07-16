using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class CarouselViewLayout : ItemsViewLayout
	{
		CarouselView _carouselView;

		public CarouselViewLayout(ListItemsLayout itemsLayout, ItemSizingStrategy itemSizingStrategy, CarouselView carouselView) : base(itemsLayout, itemSizingStrategy)
		{
			_carouselView = carouselView;
		}

		public override bool ShouldInvalidateLayout(UICollectionViewLayoutAttributes preferredAttributes, UICollectionViewLayoutAttributes originalAttributes)
		{
			return base.ShouldInvalidateLayout(preferredAttributes, originalAttributes);
		}

		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			return false;
		}

		public override void ConstrainTo(CGSize size)
		{
			//TODO: Should we scale the items 
			var aspectRation = size.Width / size.Height;

			var width = (size.Width - _carouselView.Padding.Left - _carouselView.Padding.Right) / _carouselView.NumberOfVisibleItems;
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
			var left = insets.Left + (float)_carouselView.Padding.Left;
			var right = insets.Right + (float)_carouselView.Padding.Right;
			var top = insets.Top + (float)_carouselView.Padding.Top;
			var bottom = insets.Bottom + (float)_carouselView.Padding.Bottom;

			//We give some insets so the user can be able to scroll to the first and last item
			if (_carouselView.NumberOfVisibleItems > 1)
			{
				if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
				{
					left = left + ItemSize.Width;
					right = right + ItemSize.Width;
					return new UIEdgeInsets(insets.Top, ItemSize.Width, insets.Bottom, ItemSize.Width);
				}
				else
				{
					return new UIEdgeInsets(ItemSize.Height, insets.Left, ItemSize.Height, insets.Right);
				}
			}

			return new UIEdgeInsets(top, left, bottom, right);

		}
	}
}
