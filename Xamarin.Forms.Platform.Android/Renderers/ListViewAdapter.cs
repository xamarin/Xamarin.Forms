using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using AView = Android.Views.View;
using AListView = Android.Widget.ListView;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	internal class ListViewAdapter : CellAdapter
	{
		const int DefaultGroupHeaderTemplateId = 0;
		const int DefaultItemTemplateId = 1;

		static int s_dividerHorizontalDarkId = int.MinValue;

		internal static readonly BindableProperty IsSelectedProperty = BindableProperty.CreateAttached("IsSelected", typeof(bool), typeof(Cell), false);

		readonly Context _context;
		protected readonly ListView _listView;
		readonly AListView _realListView;
		readonly Dictionary<DataTemplate, int> _templateToId = new Dictionary<DataTemplate, int>();
		int _dataTemplateIncrementer = 2; // lets start at not 0 because
		int _listCount = -1; // -1 we need to get count from the list
		Cell _enabledCheckCell;

		bool _fromNative;
		AView _lastSelected;
		WeakReference<Cell> _selectedCell;

		IListViewController Controller => _listView;
		protected ITemplatedItemsView<Cell> TemplatedItemsView => _listView;

		public ListViewAdapter(Context context, AListView realListView, ListView listView) : base(context)
		{
			_context = context;
			_realListView = realListView;
			_listView = listView;

			if (listView.SelectedItem != null)
				SelectItem(listView.SelectedItem);

			var templatedItems = ((ITemplatedItemsView<Cell>)listView).TemplatedItems;
			templatedItems.CollectionChanged += OnCollectionChanged;
			templatedItems.GroupedCollectionChanged += OnGroupedCollectionChanged;
			listView.ItemSelected += OnItemSelected;

			realListView.OnItemClickListener = this;
			realListView.OnItemLongClickListener = this;

			var platform = _listView.Platform;
			if (platform.GetType() == typeof(AppCompat.Platform))
				MessagingCenter.Subscribe<AppCompat.Platform>(this, AppCompat.Platform.CloseContextActionsSignalName, p => CloseContextActions());
			else
				MessagingCenter.Subscribe<Platform>(this, Platform.CloseContextActionsSignalName, p => CloseContextActions());
			InvalidateCount();
		}

		public override int Count
		{
			get
			{
				if (_listCount == -1)
				{
					var templatedItems = TemplatedItemsView.TemplatedItems;
					int count = templatedItems.Count;

					if (_listView.IsGroupingEnabled)
					{
						for (var i = 0; i < templatedItems.Count; i++)
							count += templatedItems.GetGroup(i).Count;
					}

					_listCount = count;
				}
				return _listCount;
			}
		}

		public AView FooterView { get; set; }

		public override bool HasStableIds
		{
			get { return false; }
		}

		public AView HeaderView { get; set; }

		public bool IsAttachedToWindow { get; set; }

		public override object this[int index]
		{
			get
			{
				if (_listView.IsGroupingEnabled)
				{
					Cell cell = GetCellForPosition(index);
					return cell.BindingContext;
				}

				return TemplatedItemsView.ListProxy[index];
			}
		}

		public override int ViewTypeCount
		{
			get { return 20; }
		}

		public override bool AreAllItemsEnabled()
		{
			return false;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override int GetItemViewType(int position)
		{
			var group = 0;
			var row = 0;
			DataTemplate itemTemplate;
			if (!_listView.IsGroupingEnabled)
				itemTemplate = _listView.ItemTemplate;
			else
			{
				group = TemplatedItemsView.TemplatedItems.GetGroupIndexFromGlobal(position, out row);

				if (row == 0)
				{
					itemTemplate = _listView.GroupHeaderTemplate;
					if (itemTemplate == null)
						return DefaultGroupHeaderTemplateId;
				}
				else
				{
					itemTemplate = _listView.ItemTemplate;
					row--;
				}
			}

			if (itemTemplate == null)
				return DefaultItemTemplateId;

			var selector = itemTemplate as DataTemplateSelector;
			if (selector != null)
			{
				object item = null;
				if (_listView.IsGroupingEnabled)
					item = TemplatedItemsView.TemplatedItems.GetGroup(group).ListProxy[row];
				else
					item = TemplatedItemsView.TemplatedItems.ListProxy[position];
				itemTemplate = selector.SelectTemplate(item, _listView);
			}
			int key;
			if (!_templateToId.TryGetValue(itemTemplate, out key))
			{
				_dataTemplateIncrementer++;
				key = _dataTemplateIncrementer;
				_templateToId[itemTemplate] = key;
			}
			return key;
		}

		public override AView GetView(int position, AView convertView, ViewGroup parent)
		{
			Cell cell = null;

			Performance.Start();

			ListViewCachingStrategy cachingStrategy = Controller.CachingStrategy;
			var nextCellIsHeader = false;
			if (cachingStrategy == ListViewCachingStrategy.RetainElement || convertView == null)
			{
				if (_listView.IsGroupingEnabled)
				{
					List<Cell> cells = GetCellsFromPosition(position, 2);
					if (cells.Count > 0)
						cell = cells[0];

					if (cells.Count == 2)
						nextCellIsHeader = cells[1].GetIsGroupHeader<ItemsView<Cell>, Cell>();
				}

				if (cell == null)
				{
					cell = GetCellForPosition(position);
					if (cell == null)
						return new AView(_context);
				}
			}

			var cellIsBeingReused = false;
			var layout = convertView as ConditionalFocusLayout;
			if (layout != null)
			{
				cellIsBeingReused = true;
				convertView = layout.GetChildAt(0);
			}
			else
				layout = new ConditionalFocusLayout(_context) { Orientation = Orientation.Vertical };

			if (cachingStrategy == ListViewCachingStrategy.RecycleElement && convertView != null)
			{
				var boxedCell = convertView as INativeElementView;
				if (boxedCell == null)
				{
					throw new InvalidOperationException($"View for cell must implement {nameof(INativeElementView)} to enable recycling.");
				}
				cell = (Cell)boxedCell.Element;

				// We are going to re-set the Platform here because in some cases (headers mostly) its possible this is unset and
				// when the binding context gets updated the measure passes will all fail. By applying this here the Update call
				// further down will result in correct layouts.
				cell.Platform = _listView.Platform;

				ICellController cellController = cell;
				cellController.SendDisappearing();

				int row = position;
				var group = 0;
				var templatedItems = TemplatedItemsView.TemplatedItems;
				if (_listView.IsGroupingEnabled)
					group = templatedItems.GetGroupIndexFromGlobal(position, out row);

				var templatedList = templatedItems.GetGroup(group);

				if (_listView.IsGroupingEnabled)
				{
					if (row == 0)
						templatedList.UpdateHeader(cell, group);
					else
						templatedList.UpdateContent(cell, row - 1);
				}
				else
					templatedList.UpdateContent(cell, row);

				cellController.SendAppearing();

				if (cell.BindingContext == ActionModeObject)
				{
					ActionModeContext = cell;
					ContextView = layout;
				}

				if (ReferenceEquals(_listView.SelectedItem, cell.BindingContext))
					Select(_listView.IsGroupingEnabled ? row - 1 : row, layout);
				else if (cell.BindingContext == ActionModeObject)
					SetSelectedBackground(layout, true);
				else
					UnsetSelectedBackground(layout);

				Performance.Stop();
				return layout;
			}

			AView view = CellFactory.GetCell(cell, convertView, parent, _context, _listView);

			Performance.Start("AddView");

			if (cellIsBeingReused)
			{
				if (convertView != view)
				{
					layout.RemoveViewAt(0);
					layout.AddView(view, 0);
				}
			}
			else
				layout.AddView(view, 0);

			Performance.Stop("AddView");

			bool isHeader = cell.GetIsGroupHeader<ItemsView<Cell>, Cell>();

			AView bline;

			UpdateSeparatorVisibility(cell, cellIsBeingReused, isHeader, nextCellIsHeader, layout, out bline);

			UpdateSeparatorColor(isHeader, bline);

			if ((bool)cell.GetValue(IsSelectedProperty))
				Select(position, layout);
			else
				UnsetSelectedBackground(layout);

			layout.ApplyTouchListenersToSpecialCells(cell);

			Performance.Stop();

			return layout;
		}

		public override bool IsEnabled(int position)
		{
			ListView list = _listView;

			ITemplatedItemsView<Cell> templatedItemsView = list;
			if (list.IsGroupingEnabled)
			{
				int leftOver;
				templatedItemsView.TemplatedItems.GetGroupIndexFromGlobal(position, out leftOver);
				return leftOver > 0;
			}

			if (((IListViewController)list).CachingStrategy == ListViewCachingStrategy.RecycleElement)
			{
				if (_enabledCheckCell == null)
					_enabledCheckCell = GetCellForPosition(position);
				else
					templatedItemsView.TemplatedItems.UpdateContent(_enabledCheckCell, position);
				return _enabledCheckCell.IsEnabled;
			}

			Cell item = GetCellForPosition(position);
			return item.IsEnabled;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				CloseContextActions();

				var platform = _listView.Platform;
				if (platform.GetType() == typeof(AppCompat.Platform))
					MessagingCenter.Unsubscribe<AppCompat.Platform>(this, Platform.CloseContextActionsSignalName);
				else
					MessagingCenter.Unsubscribe<Platform>(this, Platform.CloseContextActionsSignalName);

				_realListView.OnItemClickListener = null;
				_realListView.OnItemLongClickListener = null;

				var templatedItems = TemplatedItemsView.TemplatedItems;
				templatedItems.CollectionChanged -= OnCollectionChanged;
				templatedItems.GroupedCollectionChanged -= OnGroupedCollectionChanged;
				_listView.ItemSelected -= OnItemSelected;

				if (_lastSelected != null)
				{
					_lastSelected.Dispose();
					_lastSelected = null;
				}
			}

			base.Dispose(disposing);
		}

		protected override Cell GetCellForPosition(int position)
		{
			return GetCellsFromPosition(position, 1).FirstOrDefault();
		}

		protected override void HandleItemClick(AdapterView parent, AView view, int position, long id)
		{
			Cell cell = null;

			if (Controller.CachingStrategy == ListViewCachingStrategy.RecycleElement)
			{
				AView cellOwner = view;
				var layout = cellOwner as ConditionalFocusLayout;
				if (layout != null)
					cellOwner = layout.GetChildAt(0);
				cell = (Cell)((INativeElementView)cellOwner).Element;
			}

			// All our ListView's have called AddHeaderView. This effectively becomes index 0, so our index 0 is index 1 to the listView.
			position--;

			if (position < 0 || position >= Count)
				return;

			if (_lastSelected != view)
				_fromNative = true;
			Select(position, view);
			Controller.NotifyRowTapped(position, cell);
		}

		// TODO: We can optimize this by storing the last position, group index and global index
		// and increment/decrement from that starting place.	
		List<Cell> GetCellsFromPosition(int position, int take)
		{
			var cells = new List<Cell>(take);
			if (position < 0)
				return cells;

			var templatedItems = TemplatedItemsView.TemplatedItems;
			var templatedItemsCount = templatedItems.Count;
			if (!_listView.IsGroupingEnabled)
			{
				for (var x = 0; x < take; x++)
				{
					if (position + x >= templatedItemsCount)
						return cells;

					cells.Add(templatedItems[x + position]);
				}

				return cells;
			}

			var i = 0;
			var global = 0;
			for (; i < templatedItemsCount; i++)
			{
				var group = templatedItems.GetGroup(i);

				if (global == position || cells.Count > 0)
				{
					//Always create a new cell if we are using the RecycleElement strategy
					var headerCell = _listView.CachingStrategy == ListViewCachingStrategy.RecycleElement ? GetNewGroupHeaderCell(group) : group.HeaderContent;
					cells.Add(headerCell);

					if (cells.Count == take)
						return cells;
				}

				global++;

				if (global + group.Count < position)
				{
					global += group.Count;
					continue;
				}

				for (var g = 0; g < group.Count; g++)
				{
					if (global == position || cells.Count > 0)
					{
						cells.Add(group[g]);
						if (cells.Count == take)
							return cells;
					}

					global++;
				}
			}

			return cells;
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnDataChanged();
		}

		void OnDataChanged()
		{
			InvalidateCount();
			if (ActionModeContext != null && !TemplatedItemsView.TemplatedItems.Contains(ActionModeContext))
				CloseContextActions();

			if (IsAttachedToWindow)
				NotifyDataSetChanged();
			else
			{
				// In a TabbedPage page with two pages, Page A and Page B with ListView, if A changes B's ListView,
				// we need to reset the ListView's adapter to reflect the changes on page B
				// If there header and footer are present at the reset time of the adapter
				// they will be DOUBLE added to the ViewGround (the ListView) causing indexes to be off by one. 
				_realListView.RemoveHeaderView(HeaderView);
				_realListView.RemoveFooterView(FooterView);
				_realListView.Adapter = _realListView.Adapter;
				_realListView.AddHeaderView(HeaderView);
				_realListView.AddFooterView(FooterView);
			}
		}

		void OnGroupedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnDataChanged();
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs eventArg)
		{
			if (_fromNative)
			{
				_fromNative = false;
				return;
			}

			SelectItem(eventArg.SelectedItem);
		}

		void Select(int index, AView view)
		{
			if (_lastSelected != null)
			{
				UnsetSelectedBackground(_lastSelected);
				Cell previousCell;
				if (_selectedCell.TryGetTarget(out previousCell))
					previousCell.SetValue(IsSelectedProperty, false);
			}

			_lastSelected = view;

			if (index == -1)
				return;

			Cell cell = GetCellForPosition(index);
			cell.SetValue(IsSelectedProperty, true);
			_selectedCell = new WeakReference<Cell>(cell);

			if (view != null)
				SetSelectedBackground(view);
		}

		void SelectItem(object item)
		{
			if (_listView == null)
				return;

			int position = TemplatedItemsView.TemplatedItems.GetGlobalIndexOfItem(item);
			AView view = null;
			if (position != -1)
				view = _realListView.GetChildAt(position + 1 - _realListView.FirstVisiblePosition);

			Select(position, view);
		}

		void UpdateSeparatorVisibility(Cell cell, bool cellIsBeingReused, bool isHeader, bool nextCellIsHeader, ConditionalFocusLayout layout, out AView bline)
		{
			bline = null;
			if (cellIsBeingReused)
				return;
			var makeBline = _listView.SeparatorVisibility == SeparatorVisibility.Default || isHeader && !nextCellIsHeader;
			if (makeBline)
			{
				bline = new AView(_context) { LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 1) };
				layout.AddView(bline);
			}
			else if (layout.ChildCount > 1)
			{
				layout.RemoveViewAt(1);
			}
		}


		void UpdateSeparatorColor(bool isHeader, AView bline)
		{
			if (bline == null)
				return;

			Color separatorColor = _listView.SeparatorColor;

			if (isHeader || !separatorColor.IsDefault)
				bline.SetBackgroundColor(separatorColor.ToAndroid(Color.Accent));
			else
			{
				if (s_dividerHorizontalDarkId == int.MinValue)
				{
					using (var value = new TypedValue())
					{
						int id = global::Android.Resource.Drawable.DividerHorizontalDark;
						if (_context.Theme.ResolveAttribute(global::Android.Resource.Attribute.ListDivider, value, true))
							id = value.ResourceId;
						else if (_context.Theme.ResolveAttribute(global::Android.Resource.Attribute.Divider, value, true))
							id = value.ResourceId;

						s_dividerHorizontalDarkId = id;
					}
				}

				bline.SetBackgroundResource(s_dividerHorizontalDarkId);
			}
		}

		Cell GetNewGroupHeaderCell(ITemplatedItemsList<Cell> group)
		{
			var groupHeaderCell = _listView.TemplatedItems.GroupHeaderTemplate?.CreateContent(group.ItemsSource, _listView) as Cell;

			if (groupHeaderCell != null)
			{
				groupHeaderCell.BindingContext = group.ItemsSource;
			}
			else
			{
				groupHeaderCell = new TextCell();
				groupHeaderCell.SetBinding(TextCell.TextProperty, nameof(group.Name));
				groupHeaderCell.BindingContext = group;
			}

			groupHeaderCell.Parent = _listView;
			groupHeaderCell.SetIsGroupHeader<ItemsView<Cell>, Cell>(true);
			return groupHeaderCell;
		}

		enum CellType
		{
			Row,
			Header
		}

		protected virtual void InvalidateCount()
		{
			_listCount = -1;
		}
	}

}