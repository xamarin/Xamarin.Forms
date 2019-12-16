﻿using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewLayout : ItemsViewLayout
	{
		readonly CarouselView _carouselView;
		readonly ItemsLayout _itemsLayout;

		public CarouselViewLayout(ItemsLayout itemsLayout, CarouselView carouselView) : base(itemsLayout)
		{
			_carouselView = carouselView;
			_itemsLayout = itemsLayout;
		}

		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			return false;
		}

		public override void ConstrainTo(CGSize size)
		{
			//TODO: Should we scale the items 
			var width = size.Width - _carouselView.PeekAreaInsets.Left - _carouselView.PeekAreaInsets.Right;
			var height = size.Height - _carouselView.PeekAreaInsets.Top - _carouselView.PeekAreaInsets.Bottom;

			if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				ItemSize = new CGSize(width, size.Height);
			}
			else
			{
				ItemSize = new CGSize(size.Width, height);
			}
		}

		internal void UpdateConstraints(CGSize size)
		{
			ConstrainTo(size);
			UpdateCellConstraints();
		}

		public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			if (_itemsLayout is LinearItemsLayout linearItemsLayout)
				return (nfloat)linearItemsLayout.ItemSpacing;

			return base.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
		}

		public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			var insets = base.GetInsetForSection(collectionView, layout, section);
			var left = insets.Left + (float)_carouselView.PeekAreaInsets.Left;
			var right = insets.Right + (float)_carouselView.PeekAreaInsets.Right;
			var top = insets.Top + (float)_carouselView.PeekAreaInsets.Top;
			var bottom = insets.Bottom + (float)_carouselView.PeekAreaInsets.Bottom;

			return new UIEdgeInsets(top, left, bottom, right);
		}
	}
}