using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class ItemsViewDelegator<TItemsView, TViewController> : UICollectionViewDelegateFlowLayout
		where TItemsView : ItemsView
		where TViewController : ItemsViewController<TItemsView>
	{

		private IDictionary<Type, CGSize> _sizeChache = new Dictionary<Type,CGSize>();
		public ItemsViewLayout ItemsViewLayout { get; }
		public TViewController ViewController { get; }
		public IItemsViewSource ItemsSource { get; }

		protected float PreviousHorizontalOffset, PreviousVerticalOffset;

		public ItemsViewDelegator(ItemsViewLayout itemsViewLayout, TViewController itemsViewController, IItemsViewSource itemsViewSource)
		{
			ItemsViewLayout = itemsViewLayout;
			ViewController = itemsViewController;
			ItemsSource = itemsViewSource;
		}


		/// <summary>
		/// Per default this method returns the Estimated size when its not overriden.
		/// This method is called before the rendering process and sizes the cell correctly before it is displayed in the collectionview
		/// Calling the base implementation of this method will throw an exception when overriding the method
		/// </summary>
		/// <returns></returns>
		public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			if (ItemsViewLayout.ItemSizingStrategy == ItemSizingStrategy.MeasureFirstItem)
				return ItemsViewLayout.EstimatedItemSize;

			// ".CellForItem" is not reliable here because when the cell at "indexpath" is not visible it will return null
			//var cell = collectionView.CellForItem(indexPath);
			//if (cell is ItemsViewCell itemsViewCell)
			//{
			//	var size = itemsViewCell.Measure();
			//	return size;
			//}
			if (ViewController.ItemsView.ItemTemplate is DataTemplateSelector templateSelector)
			{
				var source = collectionView.Source;
				var vm = ItemsSource[indexPath];
				var viewTemplate = templateSelector.SelectTemplate(vm, ViewController.ItemsView);
				if (!(viewTemplate.CreateContent() is VisualElement visualElement))
					return ItemsViewLayout.EstimatedItemSize;
				var height = visualElement.HeightRequest;
				var s = new CGSize(ItemsViewLayout.EstimatedItemSize.Width, height + 40);

				var t = VerticalCell.MeasureInternal(visualElement, ItemsViewLayout.EstimatedItemSize.Width);

				return t;
			}
			else if (ViewController.ItemsView.ItemTemplate is DataTemplate dataTemplate)
			{

			}
			else
			{

			}

			return ItemsViewLayout.EstimatedItemSize; // This is basically a Fallback when ".CellForItem" return null
		}

		public override void Scrolled(UIScrollView scrollView)
		{
			var indexPathsForVisibleItems = ViewController.CollectionView.IndexPathsForVisibleItems.OrderBy(x => x.Row).ToList();

			if (indexPathsForVisibleItems.Count == 0)
				return;
	
			var contentInset = scrollView.ContentInset;
			var contentOffsetX = scrollView.ContentOffset.X + contentInset.Left;
			var contentOffsetY = scrollView.ContentOffset.Y + contentInset.Top;

			var firstVisibleItemIndex = (int)indexPathsForVisibleItems.First().Item;
			var centerItemIndex = ViewController.CollectionView.GetCenteredIndex();
			var lastVisibleItemIndex = (int)indexPathsForVisibleItems.Last().Item;
			var itemsViewScrolledEventArgs = new ItemsViewScrolledEventArgs
			{
				HorizontalDelta = contentOffsetX - PreviousHorizontalOffset,
				VerticalDelta = contentOffsetY - PreviousVerticalOffset,
				HorizontalOffset = contentOffsetX,
				VerticalOffset = contentOffsetY,
				FirstVisibleItemIndex = firstVisibleItemIndex,
				CenterItemIndex = centerItemIndex,
				LastVisibleItemIndex = lastVisibleItemIndex
			};

			var itemsView = ViewController.ItemsView;
			var source = ViewController.ItemsSource;
			itemsView.SendScrolled(itemsViewScrolledEventArgs);

			PreviousHorizontalOffset = (float)contentOffsetX;
			PreviousVerticalOffset = (float)contentOffsetY;

			switch (itemsView.RemainingItemsThreshold)
			{
				case -1:
					return;
				case 0:
					if (lastVisibleItemIndex == source.ItemCount - 1)
						itemsView.SendRemainingItemsThresholdReached();
					break;
				default:
					if (source.ItemCount - 1 - lastVisibleItemIndex <= itemsView.RemainingItemsThreshold)
						itemsView.SendRemainingItemsThresholdReached();
					break;
			}
		}

		public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout,
			nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default;
			}

			return ItemsViewLayout.GetInsetForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default;
			}

			return ItemsViewLayout.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default;
			}

			return ItemsViewLayout.GetMinimumLineSpacingForSection(collectionView, layout, section);
		}

		public override void CellDisplayingEnded(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			if (ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				var actualWidth = collectionView.ContentSize.Width - collectionView.Bounds.Size.Width;
				if (collectionView.ContentOffset.X >= actualWidth || collectionView.ContentOffset.X < 0)
					return;
			}
			else
			{
				var actualHeight = collectionView.ContentSize.Height - collectionView.Bounds.Size.Height;

				if (collectionView.ContentOffset.Y >= actualHeight || collectionView.ContentOffset.Y < 0)
					return;
			}
		}
	}
}