using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using AppKit;
using Foundation;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.MacOS
{
	public class ListViewRenderer : ViewRenderer<ListView, NSView>
	{

		bool _disposed;
		public const int DefaultRowHeight = 44;
		ListViewDataSource _dataSource;
		bool _estimatedRowHeight;
		IVisualElementRenderer _headerRenderer;
		IVisualElementRenderer _footerRenderer;

		ScrollToRequestedEventArgs _requestedScroll;
		bool _shouldEstimateRowHeight = true;
		IListViewController Controller => Element;
		ITemplatedItemsView<Cell> TemplatedItemsView => Element;
		NSTableView _table;

		public virtual NSTableView CreateNSTableView(ListView list)
		{
			var table = new NSTableView { SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.SourceList };

			//this is needed .. can we go around it ?
			table.AddColumn(new NSTableColumn(""));
			//this line hides the header by default
			table.HeaderView = null;

			table.Source = _dataSource = list.HasUnevenRows ? new UnevenListViewDataSource(list, _table) : new ListViewDataSource(list, _table);

			return table;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;

				var viewsToLookAt = new Stack<NSView>(Subviews);
				while (viewsToLookAt.Count > 0)
				{
					var view = viewsToLookAt.Pop();
					var viewCellRenderer = view as ViewCellNSView;
					if (viewCellRenderer != null)
						viewCellRenderer.Dispose();
					else
					{
						foreach (var child in view.Subviews)
							viewsToLookAt.Push(child);
					}
				}

				if (Element != null)
				{
					var templatedItems = TemplatedItemsView.TemplatedItems;
					templatedItems.CollectionChanged -= OnCollectionChanged;
					templatedItems.GroupedCollectionChanged -= OnGroupedCollectionChanged;
				}
			}

			if (disposing)
			{
				if (_headerRenderer != null)
				{
					var platform = _headerRenderer.Element.Platform as Platform;
					if (platform != null)
						platform.DisposeModelAndChildrenRenderers(_headerRenderer.Element);
					_headerRenderer = null;
				}
				if (_footerRenderer != null)
				{
					var platform = _footerRenderer.Element.Platform as Platform;
					if (platform != null)
						platform.DisposeModelAndChildrenRenderers(_footerRenderer.Element);
					_footerRenderer = null;
				}

				var headerView = Controller?.HeaderElement as VisualElement;
				if (headerView != null)
					headerView.MeasureInvalidated -= OnHeaderMeasureInvalidated;
				_table?.HeaderView?.Dispose();

				//var footerView = Controller?.FooterElement as VisualElement;
				//if (footerView != null)
				//	footerView.MeasureInvalidated -= OnFooterMeasureInvalidated;
				//Control?.?.Dispose();
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
		{
			_requestedScroll = null;

			if (e.OldElement != null)
			{
				var controller = (IListViewController)e.OldElement;
				var headerView = (VisualElement)controller.HeaderElement;
				if (headerView != null)
					headerView.MeasureInvalidated -= OnHeaderMeasureInvalidated;

				var footerView = (VisualElement)controller.FooterElement;
				if (footerView != null)
					footerView.MeasureInvalidated -= OnFooterMeasureInvalidated;

				controller.ScrollToRequested -= OnScrollToRequested;
				var templatedItems = ((ITemplatedItemsView<Cell>)e.OldElement).TemplatedItems;

				templatedItems.CollectionChanged -= OnCollectionChanged;
				templatedItems.GroupedCollectionChanged -= OnGroupedCollectionChanged;
			}

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var scroller = new NSScrollView
					{
						AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
						DocumentView = _table = CreateNSTableView(e.NewElement)
					};
					SetNativeControl(scroller);
				}

				_shouldEstimateRowHeight = true;

				var controller = (IListViewController)e.NewElement;

				controller.ScrollToRequested += OnScrollToRequested;
				var templatedItems = ((ITemplatedItemsView<Cell>)e.NewElement).TemplatedItems;

				templatedItems.CollectionChanged += OnCollectionChanged;
				templatedItems.GroupedCollectionChanged += OnGroupedCollectionChanged;

				UpdateRowHeight();
				UpdateHeader();
				UpdateFooter();
				UpdatePullToRefreshEnabled();
				UpdateIsRefreshing();
				UpdateSeparatorColor();
				UpdateSeparatorVisibility();

				var selected = e.NewElement.SelectedItem;
				if (selected != null)
					_dataSource.OnItemSelected(null, new SelectedItemChangedEventArgs(selected));
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == ListView.RowHeightProperty.PropertyName)
				UpdateRowHeight();
			else if (e.PropertyName == ListView.IsGroupingEnabledProperty.PropertyName)
				_dataSource.UpdateGrouping();
			else if (e.PropertyName == ListView.HasUnevenRowsProperty.PropertyName)
			{
				_estimatedRowHeight = false;
				_table.Source = _dataSource = Element.HasUnevenRows ? new UnevenListViewDataSource(_dataSource) : new ListViewDataSource(_dataSource);
			}
			else if (e.PropertyName == ListView.IsPullToRefreshEnabledProperty.PropertyName)
				UpdatePullToRefreshEnabled();
			else if (e.PropertyName == ListView.IsRefreshingProperty.PropertyName)
				UpdateIsRefreshing();
			else if (e.PropertyName == ListView.SeparatorColorProperty.PropertyName)
				UpdateSeparatorColor();
			else if (e.PropertyName == ListView.SeparatorVisibilityProperty.PropertyName)
				UpdateSeparatorVisibility();
			else if (e.PropertyName == "HeaderElement")
				UpdateHeader();
			else if (e.PropertyName == "FooterElement")
				UpdateFooter();
			else if (e.PropertyName == "RefreshAllowed")
				UpdatePullToRefreshEnabled();
		}

		NSIndexSet[] GetPaths(int section, int index, int count)
		{
			var paths = new NSIndexSet[count];
			for (var i = 0; i < paths.Length; i++)
				paths[i] = NSIndexSet.FromIndex(index + i);

			return paths;
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateItems(e, 0, true);
		}

		void OnFooterMeasureInvalidated(object sender, EventArgs eventArgs)
		{
			double width = Bounds.Width;
			if (width == 0)
				return;

			var footerView = (VisualElement)sender;
			var request = footerView.Measure(width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
			Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(footerView, new Rectangle(0, 0, width, request.Request.Height));

			//Control.TableFooterView = _footerRenderer.NativeView;
		}

		void OnGroupedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var til = (TemplatedItemsList<ItemsView<Cell>, Cell>)sender;

			var templatedItems = TemplatedItemsView.TemplatedItems;
			var groupIndex = templatedItems.IndexOf(til.HeaderContent);
			UpdateItems(e, groupIndex, false);
		}

		void OnHeaderMeasureInvalidated(object sender, EventArgs eventArgs)
		{
			double width = Bounds.Width;
			if (width == 0)
				return;

			var headerView = (VisualElement)sender;
			var request = headerView.Measure(width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
			Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(headerView, new Rectangle(0, 0, width, request.Request.Height));

			_table.HeaderView = (NSTableHeaderView)_headerRenderer.NativeView;
		}

		void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
		{
			if (Superview == null)
			{
				_requestedScroll = e;
				return;
			}

			System.Diagnostics.Debug.WriteLine("OnScrollToRequested not implemented yet");
		}

		void UpdateFooter()
		{
			System.Diagnostics.Debug.WriteLine("Footer not implemented yet");
		}

		void UpdateHeader()
		{
			var header = Controller.HeaderElement;
			var headerView = (View)header;

			if (headerView != null)
			{
				if (_headerRenderer != null)
				{
					_headerRenderer.Element.MeasureInvalidated -= OnHeaderMeasureInvalidated;
					if (header != null && _headerRenderer.GetType() == Registrar.Registered.GetHandlerType(header.GetType()))
					{
						_headerRenderer.SetElement(headerView);
						return;
					}
					_table.HeaderView = null;
					var platform = _headerRenderer.Element.Platform as Platform;
					if (platform != null)
						platform.DisposeModelAndChildrenRenderers(_headerRenderer.Element);
					_headerRenderer = null;
				}

				_headerRenderer = Platform.CreateRenderer(headerView);
				// This will force measure to invalidate, which we haven't hooked up to yet because we are smarter!
				Platform.SetRenderer(headerView, _headerRenderer);

				double width = Bounds.Width;
				var request = headerView.Measure(width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
				Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(headerView, new Rectangle(0, 0, width, request.Request.Height));

				//Control.HeaderView = _headerRenderer.NativeView;
				headerView.MeasureInvalidated += OnHeaderMeasureInvalidated;
			}
			else if (_headerRenderer != null)
			{
				_table.HeaderView = null;
				var platform = _headerRenderer.Element.Platform as Platform;
				if (platform != null)
					platform.DisposeModelAndChildrenRenderers(_headerRenderer.Element);
				_headerRenderer.Dispose();
				_headerRenderer = null;
			}
		}

		void UpdateIsRefreshing()
		{
			var refreshing = Element.IsRefreshing;
			System.Diagnostics.Debug.WriteLine("Is Refreshing not implemented");
		}

		void UpdateItems(NotifyCollectionChangedEventArgs e, int section, bool resetWhenGrouped)
		{
			var exArgs = e as NotifyCollectionChangedEventArgsEx;
			if (exArgs != null)
				_dataSource.Counts[section] = exArgs.Count;

			var groupReset = resetWhenGrouped && Element.IsGroupingEnabled;

			//switch (e.Action)
			//{
			//	case NotifyCollectionChangedAction.Add:
			//		UpdateEstimatedRowHeight();
			//		if (e.NewStartingIndex == -1 || groupReset)
			//			goto case NotifyCollectionChangedAction.Reset;
			//		Control.BeginUpdates();
			//		Control.InsertRows(GetPaths(section, e.NewStartingIndex, e.NewItems.Count), NSTableViewAnimation.SlideUp);

			//		Control.EndUpdates();

			//		break;

			//	case NotifyCollectionChangedAction.Remove:
			//		if (e.OldStartingIndex == -1 || groupReset)
			//			goto case NotifyCollectionChangedAction.Reset;
			//		Control.BeginUpdates();
			//		Control.RemoveRows(GetPaths(section, e.OldStartingIndex, e.OldItems.Count), NSTableViewAnimation.SlideDown);

			//		Control.EndUpdates();

			//		if (_estimatedRowHeight && TemplatedItemsView.TemplatedItems.Count == 0)
			//			_estimatedRowHeight = false;

			//		break;

			//	case NotifyCollectionChangedAction.Move:
			//		if (e.OldStartingIndex == -1 || e.NewStartingIndex == -1 || groupReset)
			//			goto case NotifyCollectionChangedAction.Reset;
			//		Control.BeginUpdates();
			//		for (var i = 0; i < e.OldItems.Count; i++)
			//		{
			//			var oldi = e.OldStartingIndex;
			//			var newi = e.NewStartingIndex;

			//			if (e.NewStartingIndex < e.OldStartingIndex)
			//			{
			//				oldi += i;
			//				newi += i;
			//			}

			//			Control.MoveRow(oldi, newi);
			//		}
			//		Control.EndUpdates();

			//		if (_estimatedRowHeight && e.OldStartingIndex == 0)
			//			_estimatedRowHeight = false;

			//		break;

			//	case NotifyCollectionChangedAction.Replace:
			//		if (e.OldStartingIndex == -1 || groupReset)
			//			goto case NotifyCollectionChangedAction.Reset;
			//		Control.BeginUpdates();
			//		Control.ReloadData(GetPaths(section, e.OldStartingIndex, e.OldItems.Count), null);
			//		Control.EndUpdates();

			//		if (_estimatedRowHeight && e.OldStartingIndex == 0)
			//			_estimatedRowHeight = false;

			//		break;

			//	case NotifyCollectionChangedAction.Reset:
			//		_estimatedRowHeight = false;
			//		Control.ReloadData();
			//		return;
			//}
		}

		void UpdatePullToRefreshEnabled()
		{
			System.Diagnostics.Debug.WriteLine("Pull to Refresh not implemented on MacOS");
		}

		void UpdateRowHeight()
		{
			var rowHeight = Element.RowHeight;
			if (Element.HasUnevenRows && rowHeight == -1)
			{
				_table.RowHeight = NoIntrinsicMetric;
			}
			else
				_table.RowHeight = rowHeight <= 0 ? DefaultRowHeight : rowHeight;
		}

		void UpdateSeparatorColor()
		{
			var color = Element.SeparatorColor;
			// ...and Steve said to the unbelievers the separator shall be gray, and gray it was. The unbelievers looked on, and saw that it was good, and 
			// they went forth and documented the default color. The holy scripture still reflects this default.
			// Defined here: https://developer.apple.com/library/ios/documentation/UIKit/Reference/UITableView_Class/#//apple_ref/occ/instp/UITableView/separatorColor
			//Control.SeparatorColor = color.ToUIColor(UIColor.Gray);
			System.Diagnostics.Debug.WriteLine("Separator not implemented on MacOS");
		}

		void UpdateSeparatorVisibility()
		{
			var visibility = Element.SeparatorVisibility;
			switch (visibility)
			{
				case SeparatorVisibility.Default:
					//Control.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
					break;
				case SeparatorVisibility.None:
					//Control.SeparatorStyle = UITableViewCellSeparatorStyle.None;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			System.Diagnostics.Debug.WriteLine("Separator not implemented on MacOS");
		}




	}

}

