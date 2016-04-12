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

namespace Xamarin.Forms.Platform.Android
{
	internal sealed class ListViewAdapter : CellAdapter
	{
		const int DefaultGroupHeaderTemplateId = 0;
		const int DefaultItemTemplateId = 1;

		static int s_dividerHorizontalDarkId = int.MinValue;

		internal static readonly BindableProperty IsSelectedProperty = BindableProperty.CreateAttached("IsSelected", typeof(bool), typeof(Cell), false);

		readonly Context _context;
		readonly ListView _listView;
		readonly AListView _realListView;
		readonly Dictionary<DataTemplate, int> _templateToId = new Dictionary<DataTemplate, int>();
		int _dataTemplateIncrementer = 2; // lets start at not 0 because
		Cell _enabledCheckCell;

		bool _fromNative;
		AView _lastSelected;
		WeakReference<Cell> _selectedCell;

		public ListViewAdapter(Context context, AListView realListView, ListView listView) : base(context)
		{
			_context = context;
			_realListView = realListView;
			_listView = listView;

			if (listView.SelectedItem != null)
				SelectItem(listView.SelectedItem);

			listView.TemplatedItems.CollectionChanged += OnCollectionChanged;
			listView.TemplatedItems.GroupedCollectionChanged += OnGroupedCollectionChanged;
			listView.ItemSelected += OnItemSelected;

			realListView.OnItemClickListener = this;
			realListView.OnItemLongClickListener = this;

			MessagingCenter.Subscribe<Platform>(this, Platform.CloseContextActionsSignalName, p => CloseContextAction());
		}

		public override int Count
		{
			get
			{
				int count = _listView.TemplatedItems.Count;

				if (_listView.IsGroupingEnabled)
				{
					for (var i = 0; i < _listView.TemplatedItems.Count; i++)
						count += _listView.TemplatedItems.GetGroup(i).Count;
				}

				return count;
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

				return _listView.ListProxy[index];
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
				group = _listView.TemplatedItems.GetGroupIndexFromGlobal(position, out row);

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
					item = _listView.TemplatedItems.GetGroup(group).ListProxy[row];
				else
					item = _listView.TemplatedItems.ListProxy[position];
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

			ListViewCachingStrategy cachingStrategy = _listView.CachingStrategy;
			var nextCellIsHeader = false;
			if (cachingStrategy == ListViewCachingStrategy.RetainElement || convertView == null)
			{
				if (_listView.IsGroupingEnabled)
				{
					List<Cell> cells = GetCellsFromPosition(position, 2);
					if (cells.Count > 0)
						cell = cells[0];

					if (cells.Count == 2)
						nextCellIsHeader = TemplatedItemsList<ItemsView<Cell>, Cell>.GetIsGroupHeader(cells[1]);
				}

				if (cell == null)
				{
					cell = GetCellForPosition(position);
					if (cell == null)
						return new AView(_context);
				}
			}

			var makeBline = true;
			var layout = convertView as ConditionalFocusLayout;
			if (layout != null)
			{
				makeBline = false;
				convertView = layout.GetChildAt(0);
			}
			else
				layout = new ConditionalFocusLayout(_context) { Orientation = Orientation.Vertical };

			if (cachingStrategy == ListViewCachingStrategy.RecycleElement && convertView != null)
			{
				var boxedCell = (INativeElementView)convertView;
				if (boxedCell == null)
				{
					throw new InvalidOperationException($"View for cell must implement {nameof(INativeElementView)} to enable recycling.");
				}
				cell = (Cell)boxedCell.Element;

				if (ActionModeContext == cell)
				{
					// This appears to never happen, the theory is android keeps all views alive that are currently selected for long-press (preventing them from being recycled).
					// This is convenient since we wont have to worry about the user scrolling the cell offscreen and us losing our context actions.
					ActionModeContext = null;
					ContextView = null;
				}
				// We are going to re-set the Platform here because in some cases (headers mostly) its possible this is unset and
				// when the binding context gets updated the measure passes will all fail. By applying this hear the Update call
				// further down will result in correct layouts.
				cell.Platform = _listView.Platform;

				cell.SendDisappearing();

				int row = position;
				var group = 0;
				if (_listView.IsGroupingEnabled)
					group = _listView.TemplatedItems.GetGroupIndexFromGlobal(position, out row);

				TemplatedItemsList<ItemsView<Cell>, Cell> templatedList = _listView.TemplatedItems.GetGroup(group);

				if (_listView.IsGroupingEnabled)
				{
					if (row == 0)
						templatedList.UpdateHeader(cell, group);
					else
						templatedList.UpdateContent(cell, row - 1);
				}
				else
					templatedList.UpdateContent(cell, row);

				cell.SendAppearing();

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

			if (!makeBline)
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

			AView bline;
			if (makeBline)
			{
				bline = new AView(_context) { LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 1) };

				layout.AddView(bline);
			}
			else
				bline = layout.GetChildAt(1);

			bool isHeader = TemplatedItemsList<ItemsView<Cell>, Cell>.GetIsGroupHeader(cell);

			Color separatorColor = _listView.SeparatorColor;

			if (nextCellIsHeader || _listView.SeparatorVisibility == SeparatorVisibility.None)
				bline.SetBackgroundColor(global::Android.Graphics.Color.Transparent);
			else if (isHeader || !separatorColor.IsDefault)
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

			if (list.IsGroupingEnabled)
			{
				int leftOver;
				list.TemplatedItems.GetGroupIndexFromGlobal(position, out leftOver);
				return leftOver > 0;
			}

			if (list.CachingStrategy == ListViewCachingStrategy.RecycleElement)
			{
				if (_enabledCheckCell == null)
					_enabledCheckCell = GetCellForPosition(position);
				else
					list.TemplatedItems.UpdateContent(_enabledCheckCell, position);
				return _enabledCheckCell.IsEnabled;
			}

			Cell item = GetCellForPosition(position);
			return item.IsEnabled;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				CloseContextAction();
				MessagingCenter.Unsubscribe<Platform>(this, Platform.CloseContextActionsSignalName);
				_realListView.OnItemClickListener = null;
				_realListView.OnItemLongClickListener = null;

				_listView.TemplatedItems.CollectionChanged -= OnCollectionChanged;
				_listView.TemplatedItems.GroupedCollectionChanged -= OnGroupedCollectionChanged;
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

			if (_listView.CachingStrategy == ListViewCachingStrategy.RecycleElement)
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

			Select(position, view);
			_fromNative = true;
			_listView.NotifyRowTapped(position, cell);
		}

		// TODO: We can optimize this by storing the last position, group index and global index
		// and increment/decrement from that starting place.	
		List<Cell> GetCellsFromPosition(int position, int take)
		{
			var cells = new List<Cell>(take);
			if (position < 0)
				return cells;

			if (!_listView.IsGroupingEnabled)
			{
				for (var x = 0; x < take; x++)
				{
					if (position + x >= _listView.TemplatedItems.Count)
						return cells;

					cells.Add(_listView.TemplatedItems[x + position]);
				}

				return cells;
			}

			var i = 0;
			var global = 0;
			for (; i < _listView.TemplatedItems.Count; i++)
			{
				TemplatedItemsList<ItemsView<Cell>, Cell> group = _listView.TemplatedItems.GetGroup(i);

				if (global == position || cells.Count > 0)
				{
					cells.Add(group.HeaderContent);
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
			int position = _listView.TemplatedItems.GetGlobalIndexOfItem(item);
			AView view = null;
			if (position != -1)
				view = _realListView.GetChildAt(position + 1 - _realListView.FirstVisiblePosition);

			Select(position, view);
		}

		enum CellType
		{
			Row,
			Header
		}
	}
}