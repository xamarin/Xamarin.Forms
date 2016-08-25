using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using AppKit;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
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
			return renderHeight > 0 ? (nfloat)renderHeight : ListViewRenderer.DefaultRowHeight;
		}
	}
}
