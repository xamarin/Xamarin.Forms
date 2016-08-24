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
	public class ListViewRenderer : ViewRenderer<ListView, NSTableView>
	{

		bool _disposed;
		const int DefaultRowHeight = 44;
		ListViewDataSource _dataSource;
		bool _estimatedRowHeight;
		IVisualElementRenderer _headerRenderer;
		IVisualElementRenderer _footerRenderer;

		ScrollToRequestedEventArgs _requestedScroll;
		bool _shouldEstimateRowHeight = true;
		IListViewController Controller => Element;
		ITemplatedItemsView<Cell> TemplatedItemsView => Element;
		protected override void Dispose(bool disposing)
		{
			// check inset tracker for null to 
			if (disposing && !_disposed)
			{
				_disposed = true;

				var viewsToLookAt = new Stack<NSView>(Subviews);
				while (viewsToLookAt.Count > 0)
				{
					var view = viewsToLookAt.Pop();
					//var viewCellRenderer = view as ViewCellRenderer.ViewTableCell;
					//if (viewCellRenderer != null)
					//	viewCellRenderer.Dispose();
					//else
					//{
					//	foreach (var child in view.Subviews)
					//		viewsToLookAt.Push(child);
					//}
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
				Control?.HeaderView?.Dispose();

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
					SetNativeControl(new NSTableView { SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.SourceList });
					//this is needed .. can we go around it ?
					Control.AddColumn(new NSTableColumn("Values"));
					Control.Source = _dataSource = e.NewElement.HasUnevenRows ? new UnevenListViewDataSource(e.NewElement, Control) : new ListViewDataSource(e.NewElement, Control);

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
				Control.Source = _dataSource = Element.HasUnevenRows ? new UnevenListViewDataSource(_dataSource) : new ListViewDataSource(_dataSource);
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

			Control.HeaderView = (NSTableHeaderView)_headerRenderer.NativeView;
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
					Control.HeaderView = null;
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
				Control.HeaderView = null;
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
				Control.RowHeight = NoIntrinsicMetric;
			}
			else
				Control.RowHeight = rowHeight <= 0 ? DefaultRowHeight : rowHeight;
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

		internal class UnevenListViewDataSource : ListViewDataSource
		{
			IVisualElementRenderer _prototype;


			public UnevenListViewDataSource(ListView list, NSTableView tableView) : base(list, tableView)
			{
			}

			public UnevenListViewDataSource(ListViewDataSource source) : base(source)
			{
			}

			internal nfloat CalculateHeightForCell(NSTableView tableView, Cell cell)
			{
				var viewCell = cell as ViewCell;
				if (viewCell != null && viewCell.View != null)
				{
					var target = viewCell.View;
					if (_prototype == null)
					{
						_prototype = Platform.CreateRenderer(target);
						Platform.SetRenderer(target, _prototype);
					}
					else
					{
						_prototype.SetElement(target);
						Platform.SetRenderer(target, _prototype);
					}

					var req = target.Measure(tableView.Frame.Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);

					target.ClearValue(Platform.RendererProperty);
					foreach (var descendant in target.Descendants())
						descendant.ClearValue(Platform.RendererProperty);

					return (nfloat)req.Request.Height;
				}
				var renderHeight = cell.RenderHeight;
				return renderHeight > 0 ? (nfloat)renderHeight : DefaultRowHeight;
			}
		}

		internal class ListViewDataSource : NSTableViewSource
		{
			const int DefaultItemTemplateId = 1;
			static int s_dataTemplateIncrementer = 2; // lets start at not 0 because
			readonly nfloat _defaultSectionHeight;
			readonly Dictionary<DataTemplate, int> _templateToId = new Dictionary<DataTemplate, int>();
			readonly NSTableView _uiTableView;
			protected readonly ListView List;
			IListViewController Controller => List;
			ITemplatedItemsView<Cell> TemplatedItemsView => List;
			bool _isDragging;
			bool _selectionFromNative;

			public ListViewDataSource(ListViewDataSource source)
			{
				List = source.List;
				_uiTableView = source._uiTableView;
				_defaultSectionHeight = source._defaultSectionHeight;
				_selectionFromNative = source._selectionFromNative;

				Counts = new Dictionary<int, int>();
			}

			public ListViewDataSource(ListView list, NSTableView tableView)
			{
				_uiTableView = tableView;

				List = list;
				List.ItemSelected += OnItemSelected;
				UpdateShortNameListener();

				Counts = new Dictionary<int, int>();
			}

			public Dictionary<int, int> Counts { get; set; }


			public void OnItemSelected(object sender, SelectedItemChangedEventArgs eventArg)
			{
				if (_selectionFromNative)
				{
					_selectionFromNative = false;
					return;
				}

				var location = TemplatedItemsView.TemplatedItems.GetGroupAndIndexOfItem(eventArg.SelectedItem);
				if (location.Item1 == -1 || location.Item2 == -1)
				{
					//	var selectedIndexPath = _uiTableView.IndexPathForSelectedRow;

					//	var animate = true;

					//	if (selectedIndexPath != null)
					//	{
					//		var cell = _uiTableView.CellAt(selectedIndexPath) as ContextActionsCell;
					//		if (cell != null)
					//		{
					//			cell.PrepareForDeselect();
					//			if (cell.IsOpen)
					//				animate = false;
					//		}
					//	}

					//	if (selectedIndexPath != null)
					//		_uiTableView.DeselectRow(selectedIndexPath, animate);
					return;
				}

				//_uiTableView.SelectRow(NSIndexPath.FromRowSection(location.Item2, location.Item1), true, UITableViewScrollPosition.Middle);
			}

			public override nint GetRowCount(NSTableView tableView)
			{
				int section = 1;
				int countOverride;
				if (Counts.TryGetValue((int)section, out countOverride))
				{
					Counts.Remove((int)section);
					return countOverride;
				}

				var templatedItems = TemplatedItemsView.TemplatedItems;
				if (List.IsGroupingEnabled)
				{
					var group = (IList)((IList)templatedItems)[(int)section];
					return group.Count;
				}

				return templatedItems.Count;
			}

			const string identifer = "myCellIdentifier";

			public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				var indexPath = NSIndexPath.FromItemSection(row, 1);
				var id = TemplateIdForPath(indexPath);
				var cell = GetCellForPath(indexPath);
				var nativeCell = CellNSView.GetNativeCell(tableView, cell, true, id.ToString());
				return nativeCell;
			}

			public void UpdateGrouping()
			{
				UpdateShortNameListener();
				_uiTableView.ReloadData();
			}

			protected Cell GetCellForPath(NSIndexPath indexPath)
			{
				var templatedItems = TemplatedItemsView.TemplatedItems;
				if (List.IsGroupingEnabled)
					templatedItems = (TemplatedItemsList<ItemsView<Cell>, Cell>)((IList)templatedItems)[(int)indexPath.Section];

				var cell = templatedItems[(int)indexPath.Item];
				return cell;
			}

			void OnShortNamesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				_uiTableView.ReloadData();
			}

			int TemplateIdForPath(NSIndexPath indexPath)
			{
				var itemTemplate = List.ItemTemplate;
				var selector = itemTemplate as DataTemplateSelector;
				if (selector == null)
					return DefaultItemTemplateId;

				var templatedList = TemplatedItemsView.TemplatedItems;
				if (List.IsGroupingEnabled)
					templatedList = (TemplatedItemsList<ItemsView<Cell>, Cell>)((IList)templatedList)[(int)indexPath.Section];

				var item = templatedList.ListProxy[(int)indexPath.Item];

				itemTemplate = selector.SelectTemplate(item, List);
				int key;
				if (!_templateToId.TryGetValue(itemTemplate, out key))
				{
					s_dataTemplateIncrementer++;
					key = s_dataTemplateIncrementer;
					_templateToId[itemTemplate] = key;
				}
				return key;
			}

			void UpdateShortNameListener()
			{
				var templatedList = TemplatedItemsView.TemplatedItems;
				if (List.IsGroupingEnabled)
				{
					if (templatedList.ShortNames != null)
						((INotifyCollectionChanged)templatedList.ShortNames).CollectionChanged += OnShortNamesCollectionChanged;
				}
				else
				{
					if (templatedList.ShortNames != null)
						((INotifyCollectionChanged)templatedList.ShortNames).CollectionChanged -= OnShortNamesCollectionChanged;
				}
			}
		}
	}

}

