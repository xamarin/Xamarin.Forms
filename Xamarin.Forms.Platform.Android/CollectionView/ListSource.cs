using System;
using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Android
{
	sealed class ListSource : IItemsViewSource
	{
		private List<object> _itemsSource;

		public ListSource()
		{
		}

		public ListSource(IEnumerable<object> enumerable) 
		{
			_itemsSource = new List<object>(enumerable);
		}

		public ListSource(IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				_itemsSource.Add(item);
			}
		}

		public int Count => _itemsSource.Count + (HasHeader ? 1 : 0) + (HasFooter ? 1 : 0);

		public bool HasHeader { get; set; }
		public bool HasFooter { get; set; }

		public void Dispose()
		{
			
		}

		public bool IsFooter(int index)
		{
			if (!HasFooter)
			{
				return false;
			}

			if (HasHeader)
			{
				return index == Count + 1;
			}

			return index == Count;
		}

		public bool IsHeader(int index)
		{
			return HasHeader && index == 0;
		}

		public int GetPosition(object item)
		{
			for (int n = 0; n < _itemsSource.Count; n++)
			{
				if (_itemsSource[n] == item)
				{
					return AdjustPosition(n);
				}
			}

			throw new IndexOutOfRangeException($"{item} not found in source.");
		}

		public object GetItem(int position)
		{
			return _itemsSource[AdjustIndexRequest(position)];
		}

		int AdjustIndexRequest(int index)
		{
			return index - (HasHeader ? 1 : 0);
		}

		int AdjustPosition(int index)
		{
			return index + (HasHeader ? 1 : 0);
		}
	}
}