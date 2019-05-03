using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Xamarin.Forms
{
	// Used by the SelectableItemsView to keep track of (and respond to changes in) the SelectedItems property
	internal class SelectionList : IList<object>
	{
		readonly SelectableItemsView _selectableItemsView;
		static readonly IList<object> s_empty = new List<object>(0);

		readonly IList<object> _internal;
		IList<object> _shadow;
		bool _fromRenderer;

		public SelectionList(SelectableItemsView selectableItemsView, IList<object> items = null)
		{
			_selectableItemsView = selectableItemsView ?? throw new ArgumentNullException(nameof(selectableItemsView));
			_internal = items ?? new List<object>();
			_shadow = Copy();

			if (items is INotifyCollectionChanged incc)
			{
				incc.CollectionChanged += OnCollectionChanged;
			}
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (_fromRenderer)
			{
				return;
			}

			_selectableItemsView.SelectedItemsPropertyChanged(_shadow, _internal);
			_shadow = Copy();
		}

		public object this[int index] { get => _internal[index]; set => _internal[index] = value; }

		public int Count => _internal.Count;
		public bool IsReadOnly => false;

		public void Add(object item)
		{
			_fromRenderer = true;
			_internal.Add(item);
			_fromRenderer = false;

			_selectableItemsView.SelectedItemsPropertyChanged(_shadow, _internal);
			_shadow.Add(item);
		}

		public void Clear()
		{
			_fromRenderer = true;
			_internal.Clear();
			_fromRenderer = false;

			_selectableItemsView.SelectedItemsPropertyChanged(_shadow, s_empty);
			_shadow.Clear();
		}

		public bool Contains(object item)
		{
			return _internal.Contains(item);
		}

		public void CopyTo(object[] array, int arrayIndex)
		{
			_internal.CopyTo(array, arrayIndex);
		}

		public IEnumerator<object> GetEnumerator()
		{
			return _internal.GetEnumerator();
		}

		public int IndexOf(object item)
		{
			return _internal.IndexOf(item);
		}

		public void Insert(int index, object item)
		{
			_fromRenderer = true;
			_internal.Insert(index, item);
			_fromRenderer = false;

			_selectableItemsView.SelectedItemsPropertyChanged(_shadow, _internal);
			_shadow.Insert(index, item);
		}

		public bool Remove(object item)
		{
			_fromRenderer = true;
			var removed = _internal.Remove(item);
			_fromRenderer = false;

			if (removed)
			{
				_selectableItemsView.SelectedItemsPropertyChanged(_shadow, _internal);
				_shadow.Remove(item);
			}

			return removed;
		}

		public void RemoveAt(int index)
		{
			_fromRenderer = true;
			_internal.RemoveAt(index);
			_fromRenderer = false;

			_selectableItemsView.SelectedItemsPropertyChanged(_shadow, _internal);
			_shadow.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _internal.GetEnumerator();
		}

		List<object> Copy()
		{
			var items = new List<object>();
			for (int n = 0; n < _internal.Count; n++)
			{
				items.Add(_internal[n]);
			}

			return items;
		}
	}
}
