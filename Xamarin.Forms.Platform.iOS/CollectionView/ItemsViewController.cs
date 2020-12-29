﻿using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class ItemsViewController<TItemsView> : UICollectionViewController
	where TItemsView : ItemsView
	{
		public const int EmptyTag = 333;

		public IItemsViewSource ItemsSource { get; protected set; }
		public TItemsView ItemsView { get; }
		protected ItemsViewLayout ItemsViewLayout { get; set; }
		bool _initialized;
		bool _isEmpty;
		bool _emptyViewDisplayed;
		bool _disposed;

		UIView _emptyUIView;
		VisualElement _emptyViewFormsElement;
		Dictionary<NSIndexPath, TemplatedCell> _measurementCells = new Dictionary<NSIndexPath, TemplatedCell>();
		Dictionary<object, CGSize> _cellSizeCache = new Dictionary<object, CGSize>();

		protected UICollectionViewDelegateFlowLayout Delegator { get; set; }

		protected ItemsViewController(TItemsView itemsView, ItemsViewLayout layout) : base(layout)
		{
			ItemsView = itemsView;
			ItemsViewLayout = layout;
		}

		public void UpdateLayout(ItemsViewLayout newLayout)
		{
			// Ignore calls to this method if the new layout is the same as the old one
			if (CollectionView.CollectionViewLayout == newLayout)
				return;

			ItemsViewLayout = newLayout;
			_initialized = false;

			EnsureLayoutInitialized();

			if (_initialized)
			{
				// Reload the data so the currently visible cells get laid out according to the new layout
				CollectionView.ReloadData();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				ItemsSource?.Dispose();

				CollectionView.Delegate = null;
				Delegator?.Dispose();

				_emptyUIView?.Dispose();
				_emptyUIView = null;

				_emptyViewFormsElement = null;

				ItemsViewLayout?.Dispose();
				CollectionView?.Dispose();
			}

			base.Dispose(disposing);
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell(DetermineCellReuseId(), indexPath) as UICollectionViewCell;

			switch (cell)
			{
				case DefaultCell defaultCell:
					UpdateDefaultCell(defaultCell, indexPath);
					break;
				case TemplatedCell templatedCell:
					UpdateTemplatedCell(templatedCell, indexPath);
					break;
			}

			return cell;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			if (!_initialized)
			{
				return 0;
			}

			CheckForEmptySource();

			return ItemsSource.ItemCountInGroup(section);
		}

		void CheckForEmptySource()
		{
			var wasEmpty = _isEmpty;

			_isEmpty = ItemsSource.ItemCount == 0;

			if (_isEmpty)
			{
				_measurementCells.Clear();
				_cellSizeCache.Clear();
			}

			if (wasEmpty != _isEmpty)
			{
				 UpdateEmptyViewVisibility(_isEmpty);
			}
			
			if (wasEmpty && !_isEmpty)
			{
				// If we're going from empty to having stuff, it's possible that we've never actually measured
				// a prototype cell and our itemSize or estimatedItemSize are wrong/unset
				// So trigger a constraint update; if we need a measurement, that will make it happen
				ItemsViewLayout.ConstrainTo(CollectionView.Bounds.Size);
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ItemsSource = CreateItemsViewSource();

			if (!Forms.IsiOS11OrNewer)
				AutomaticallyAdjustsScrollViewInsets = false;
			else
			{
				// We set this property to keep iOS from trying to be helpful about insetting all the 
				// CollectionView content when we're in landscape mode (to avoid the notch)
				// The SetUseSafeArea Platform Specific is already taking care of this for us 
				// That said, at some point it's possible folks will want a PS for controlling this behavior
				CollectionView.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;
			}

			RegisterViewTypes();
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			// We can't set this constraint up on ViewDidLoad, because Forms does other stuff that resizes the view
			// and we end up with massive layout errors. And View[Will/Did]Appear do not fire for this controller
			// reliably. So until one of those options is cleared up, we set this flag so that the initial constraints
			// are set up the first time this method is called.
			EnsureLayoutInitialized();

			LayoutEmptyView();
		}

		void EnsureLayoutInitialized()
		{
			if (_initialized)
			{
				return;
			}

			if (!ItemsView.IsVisible)
			{
				// If the CollectionView starts out invisible, we'll get a layout pass with a size of 1,1 and everything will
				// go pear-shaped. So until the first time this CollectionView is visible, we do nothing.
				return;
			}

			_initialized = true;

			ItemsViewLayout.GetPrototype = GetPrototype;

			Delegator = CreateDelegator();
			CollectionView.Delegate = Delegator;

			ItemsViewLayout.SetInitialConstraints(CollectionView.Bounds.Size);
			CollectionView.SetCollectionViewLayout(ItemsViewLayout, false);

			UpdateEmptyView();
		}

		protected virtual UICollectionViewDelegateFlowLayout CreateDelegator()
		{
			return new ItemsViewDelegator<TItemsView, ItemsViewController<TItemsView>>(ItemsViewLayout, this);
		}

		protected virtual IItemsViewSource CreateItemsViewSource()
		{
			return ItemsSourceFactory.Create(ItemsView.ItemsSource, this);
		}

		public virtual void UpdateItemsSource()
		{
			_measurementCells.Clear();
			_cellSizeCache.Clear();
			ItemsSource = CreateItemsViewSource();
			CollectionView.ReloadData();
			CollectionView.CollectionViewLayout.InvalidateLayout();
		}

		public virtual void UpdateFlowDirection()
		{
			CollectionView.UpdateFlowDirection(ItemsView);

			if (_emptyViewDisplayed)
			{
				FlipEmptyView();
			}

			Layout.InvalidateLayout();
		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			if(!_initialized)
			{
				return 0;
			}

			CheckForEmptySource();
			return ItemsSource.GroupCount;
		}

		protected virtual void UpdateDefaultCell(DefaultCell cell, NSIndexPath indexPath)
		{
			cell.Label.Text = ItemsSource[indexPath].ToString();

			if (cell is ItemsViewCell constrainedCell)
			{
				ItemsViewLayout.PrepareCellForLayout(constrainedCell);
			}
		}

		protected virtual void UpdateTemplatedCell(TemplatedCell cell, NSIndexPath indexPath)
		{
			cell.ContentSizeChanged -= CellContentSizeChanged;
			cell.LayoutAttributesChanged -= CellLayoutAttributesChanged;

			// If we've already created a cell for this index path (for measurement), re-use the content
			if (_measurementCells.TryGetValue(indexPath, out TemplatedCell measurementCell))
			{
				_measurementCells.Remove(indexPath);
				measurementCell.ContentSizeChanged -= CellContentSizeChanged;
				measurementCell.LayoutAttributesChanged -= CellLayoutAttributesChanged;
				cell.UseContent(measurementCell);
			}
			else
			{
				cell.Bind(ItemsView.ItemTemplate, ItemsSource[indexPath], ItemsView);
			}

			cell.ContentSizeChanged += CellContentSizeChanged;
			cell.LayoutAttributesChanged += CellLayoutAttributesChanged;

			ItemsViewLayout.PrepareCellForLayout(cell);
		}

		public virtual NSIndexPath GetIndexForItem(object item)
		{
			return ItemsSource.GetIndexForItem(item);
		}

		protected object GetItemAtIndex(NSIndexPath index)
		{
			return ItemsSource[index];
		}

		void CellContentSizeChanged(object sender, EventArgs e)
		{
			if (_disposed)
				return;

			if (!(sender is TemplatedCell cell))
			{
				return;
			}

			var visibleCells = CollectionView.VisibleCells;

			for (int n = 0; n < visibleCells.Length; n++)
			{
				if (cell == visibleCells[n])
				{
					Layout?.InvalidateLayout();
					return;
				}
			}
		}

		void CellLayoutAttributesChanged(object sender, LayoutAttributesChangedEventArgs args)
		{
			CacheCellAttributes(args.NewAttributes.IndexPath, args.NewAttributes.Size);
		}

		protected virtual void CacheCellAttributes(NSIndexPath indexPath, CGSize size) 
		{
			if (!ItemsSource.IsIndexPathValid(indexPath))
			{
				// The upate might be coming from a cell that's being removed; don't cache it.
				return;
			}

			var item = ItemsSource[indexPath];
			if (item != null)
			{
				_cellSizeCache[item] = size;
			}
		}

		protected virtual string DetermineCellReuseId()
		{
			if (ItemsView.ItemTemplate != null)
			{
				return ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
					? HorizontalCell.ReuseId
					: VerticalCell.ReuseId;
			}

			return ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
				? HorizontalDefaultCell.ReuseId
				: VerticalDefaultCell.ReuseId;
		}

		UICollectionViewCell GetPrototype()
		{
			if (ItemsSource.ItemCount == 0)
			{
				return null;
			}

			var group = 0;

			if (ItemsSource.GroupCount > 1)
			{
				// If we're in a grouping situation, then we need to make sure we find an actual data item
				// to use for our prototype cell. It's possible that we have empty groups.
				for (int n = 0; n < ItemsSource.GroupCount; n++)
				{
					if (ItemsSource.ItemCountInGroup(n) > 0)
					{
						group = n;
						break;
					}
				}
			}

			var indexPath = NSIndexPath.Create(group, 0);

			return CreateMeasurementCell(indexPath);
		}

		protected virtual void RegisterViewTypes()
		{
			CollectionView.RegisterClassForCell(typeof(HorizontalDefaultCell), HorizontalDefaultCell.ReuseId);
			CollectionView.RegisterClassForCell(typeof(VerticalDefaultCell), VerticalDefaultCell.ReuseId);
			CollectionView.RegisterClassForCell(typeof(HorizontalCell), HorizontalCell.ReuseId);
			CollectionView.RegisterClassForCell(typeof(VerticalCell), VerticalCell.ReuseId);
		}

		protected abstract bool IsHorizontal { get; }

		protected virtual CGRect DetermineEmptyViewFrame() 
		{
			return new CGRect(CollectionView.Frame.X, CollectionView.Frame.Y,
				CollectionView.Frame.Width, CollectionView.Frame.Height);
		}

		protected void RemeasureLayout(VisualElement formsElement)
		{
			if (IsHorizontal)
			{
				var request = formsElement.Measure(double.PositiveInfinity, CollectionView.Frame.Height, MeasureFlags.IncludeMargins);
				Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(formsElement, new Rectangle(0, 0, request.Request.Width, CollectionView.Frame.Height));
			}
			else
			{
				var request = formsElement.Measure(CollectionView.Frame.Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
				Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(formsElement, new Rectangle(0, 0, CollectionView.Frame.Width, request.Request.Height));
			}
		}

		protected void OnFormsElementMeasureInvalidated(object sender, EventArgs e)
		{
			if (sender is VisualElement formsElement)
			{
				HandleFormsElementMeasureInvalidated(formsElement);
			}
		}

		protected virtual void HandleFormsElementMeasureInvalidated(VisualElement formsElement)
		{
			RemeasureLayout(formsElement);
        }

		internal void UpdateView(object view, DataTemplate viewTemplate, ref UIView uiView, ref VisualElement formsElement)
		{
			// Is view set on the ItemsView?
			if (view == null)
			{
				if (formsElement != null)
					Platform.GetRenderer(formsElement)?.DisposeRendererAndChildren();

				uiView?.Dispose();
				uiView = null;

				formsElement = null;
			}
			else
			{
				// Create the native renderer for the view, and keep the actual Forms element (if any)
				// around for updating the layout later
				(uiView, formsElement) = TemplateHelpers.RealizeView(view, viewTemplate, ItemsView);
			}
		}

		internal void UpdateEmptyView()
		{
			if (!_initialized)
			{
				return;
			}

			// Get rid of the old view
			TearDownEmptyView();

			// Set up the new empty view
			(_emptyUIView, _emptyViewFormsElement) = TemplateHelpers.RealizeView(ItemsView?.EmptyView, ItemsView?.EmptyViewTemplate, ItemsView);

			// We may need to show the updated empty view
			UpdateEmptyViewVisibility(ItemsSource?.ItemCount == 0);
		}

		void UpdateEmptyViewVisibility(bool isEmpty)
		{
			if (!_initialized)
			{
				return;
			}

			if (isEmpty)
			{
				ShowEmptyView();
			}
			else
			{
				HideEmptyView();
			}
		}

		void FlipEmptyView() 
		{
			if (_emptyUIView == null)
			{
				return;
			}

			// Flip the empty view 180 degrees around the X axis 
			_emptyUIView.Transform = CGAffineTransform.Scale(_emptyUIView.Transform, -1, 1);
		}

		void ShowEmptyView() 
		{
			if (_emptyViewDisplayed)
			{
				return;
			}

			CollectionView.AddSubview(_emptyUIView);

			_emptyUIView.Tag = EmptyTag;

			if (!ItemsView.LogicalChildren.Contains(_emptyViewFormsElement))
			{
				ItemsView.AddLogicalChild(_emptyViewFormsElement);
			}

			if (CollectionView.EffectiveUserInterfaceLayoutDirection == UIUserInterfaceLayoutDirection.RightToLeft)
			{
				// The UICollectionView's layout flip will also affect the empty view, so we'll have to flip it back around
				FlipEmptyView();
			}

			LayoutEmptyView();
			_emptyViewDisplayed = true;
		}

		void HideEmptyView() 
		{
			if (!_emptyViewDisplayed)
			{
				return;
			}

			_emptyUIView.RemoveFromSuperview();

			_emptyViewDisplayed = false;
		}

		void TearDownEmptyView() 
		{
			HideEmptyView();

			// RemoveLogicalChild will trigger a disposal of the native view and its content
			ItemsView.RemoveLogicalChild(_emptyViewFormsElement);
			
			_emptyUIView = null;
			_emptyViewFormsElement = null;
		}

		void LayoutEmptyView()
		{
			if (!_initialized || _emptyUIView == null || _emptyUIView.Superview == null)
			{
				return;
			}

			var frame = DetermineEmptyViewFrame();

			_emptyUIView.Frame = frame;

			if (_emptyViewFormsElement != null && ItemsView.LogicalChildren.Contains(_emptyViewFormsElement))
				_emptyViewFormsElement.Layout(frame.ToRectangle());
		}

		TemplatedCell CreateAppropriateCellForLayout()
		{
			var frame = new CGRect(0, 0, ItemsViewLayout.EstimatedItemSize.Width, ItemsViewLayout.EstimatedItemSize.Height);

			if (ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				return new HorizontalCell(frame);
			}
			
			return new VerticalCell(frame);
		}

		public TemplatedCell CreateMeasurementCell(NSIndexPath indexPath) 
		{
			if (ItemsView.ItemTemplate == null)
			{
				return null;
			}

			TemplatedCell templatedCell = CreateAppropriateCellForLayout(); 
						
			UpdateTemplatedCell(templatedCell, indexPath);

			// Keep this cell around, we can transfer the contents to the actual cell when the UICollectionView creates it
			_measurementCells[indexPath] = templatedCell;

			return templatedCell;
		}

		internal CGSize GetSizeForItem(NSIndexPath indexPath) 
		{
			if (ItemsViewLayout.EstimatedItemSize.IsEmpty)
			{
				return ItemsViewLayout.ItemSize;
			}

			if (ItemsSource.IsIndexPathValid(indexPath))
			{
				var item = ItemsSource[indexPath];

				if (item != null && _cellSizeCache.TryGetValue(item, out CGSize size))
				{
					return size;
				}
			}

			return ItemsViewLayout.EstimatedItemSize;
		}
	}
}
