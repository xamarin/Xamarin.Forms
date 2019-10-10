﻿using System;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class StructuredItemsViewController<TItemsView> : ItemsViewController<TItemsView>
		where TItemsView : StructuredItemsView
	{
		bool _disposed;

		UIView _headerUIView;
		VisualElement _headerViewFormsElement;

		UIView _footerUIView;
		VisualElement _footerViewFormsElement;

		public StructuredItemsViewController(TItemsView structuredItemsView, ItemsViewLayout layout)
			: base(structuredItemsView, layout)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				_headerUIView = null;
				_headerViewFormsElement = null;
				_footerUIView = null;
				_footerViewFormsElement = null;
				
			}

			base.Dispose(disposing);
		}

		protected override bool IsHorizontal => (ItemsView?.ItemsLayout as ItemsLayout)?.Orientation == ItemsLayoutOrientation.Horizontal;

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			// This update is only relevant if you have a footer view because it's used to place the footer view
			// based on the ContentSize so we just update the positions if the ContentSize has changed
			if (_footerUIView != null)
			{
				if (IsHorizontal)
				{
					if (_footerUIView.Frame.X != ItemsViewLayout.CollectionViewContentSize.Width)
						UpdateHeaderFooterPosition();
				}
				else
				{
					if (_footerUIView.Frame.Y != ItemsViewLayout.CollectionViewContentSize.Height)
						UpdateHeaderFooterPosition();
				}
			}
		}

		internal void UpdateFooterView()
		{
			UpdateSubview(ItemsView?.Footer, ItemsView?.FooterTemplate, 
				ref _footerUIView, ref _footerViewFormsElement);
			UpdateHeaderFooterPosition();
		}

		internal void UpdateHeaderView()
		{
			UpdateSubview(ItemsView?.Header, ItemsView?.HeaderTemplate, 
				ref _headerUIView, ref _headerViewFormsElement);
			UpdateHeaderFooterPosition();
		}

		void UpdateHeaderFooterPosition()
		{
			if (IsHorizontal)
			{
				var currentInset = CollectionView.ContentInset;

				nfloat headerWidth = _headerUIView?.Frame.Width ?? 0f;
				nfloat footerWidth = _footerUIView?.Frame.Width ?? 0f;

				if (_headerUIView != null && _headerUIView.Frame.X != headerWidth)
					_headerUIView.Frame = new CoreGraphics.CGRect(-headerWidth, 0, headerWidth, CollectionView.Frame.Height);

				if (_footerUIView != null && (_footerUIView.Frame.X != ItemsViewLayout.CollectionViewContentSize.Width))
					_footerUIView.Frame = new CoreGraphics.CGRect(ItemsViewLayout.CollectionViewContentSize.Width, 0, footerWidth, CollectionView.Frame.Height);

				if (CollectionView.ContentInset.Left != headerWidth || CollectionView.ContentInset.Right != footerWidth)
				{
					var currentOffset = CollectionView.ContentOffset;
					CollectionView.ContentInset = new UIEdgeInsets(0, headerWidth, 0, footerWidth);

					var xOffset = currentOffset.X + (currentInset.Left - CollectionView.ContentInset.Left);

					if (CollectionView.ContentSize.Width + headerWidth <= CollectionView.Bounds.Width)
						xOffset = -headerWidth;

					// if the header grows it will scroll off the screen because if you change the content inset iOS adjusts the content offset so the list doesn't move
					// this changes the offset of the list by however much the header size has changed
					CollectionView.ContentOffset = new CoreGraphics.CGPoint(xOffset, CollectionView.ContentOffset.Y);
				}
			}
			else
			{
				var currentInset = CollectionView.ContentInset;
				nfloat headerHeight = _headerUIView?.Frame.Height ?? 0f;
				nfloat footerHeight = _footerUIView?.Frame.Height ?? 0f;

				if (CollectionView.ContentInset.Top != headerHeight || CollectionView.ContentInset.Bottom != footerHeight)
				{
					var currentOffset = CollectionView.ContentOffset;
					CollectionView.ContentInset = new UIEdgeInsets(headerHeight, 0, footerHeight, 0);

					// if the header grows it will scroll off the screen because if you change the content inset iOS adjusts the content offset so the list doesn't move
					// this changes the offset of the list by however much the header size has changed

					var yOffset = currentOffset.Y + (currentInset.Top - CollectionView.ContentInset.Top);

					if (CollectionView.ContentSize.Height + headerHeight <= CollectionView.Bounds.Height)
						yOffset = -headerHeight;

					CollectionView.ContentOffset = new CoreGraphics.CGPoint(CollectionView.ContentOffset.X, yOffset);
				}

				if (_headerUIView != null && _headerUIView.Frame.Y != headerHeight)
				{
					_headerUIView.Frame = new CoreGraphics.CGRect(0, -headerHeight, CollectionView.Frame.Width, headerHeight);
				}

				if (_footerUIView != null && (_footerUIView.Frame.Y != ItemsViewLayout.CollectionViewContentSize.Height))
				{
					_footerUIView.Frame = new CoreGraphics.CGRect(0, ItemsViewLayout.CollectionViewContentSize.Height, CollectionView.Frame.Width, footerHeight);
				}
			}
		}

		protected override void HandleFormsElementMeasureInvalidated(VisualElement formsElement)
		{
			base.HandleFormsElementMeasureInvalidated(formsElement);
			UpdateHeaderFooterPosition();
		}

		internal void UpdateLayoutMeasurements()
		{
			if (_headerViewFormsElement != null)
				HandleFormsElementMeasureInvalidated(_headerViewFormsElement);

			if (_footerViewFormsElement != null)
				HandleFormsElementMeasureInvalidated(_footerViewFormsElement);
		}
	}
}