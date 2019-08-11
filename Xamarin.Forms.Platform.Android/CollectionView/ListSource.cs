using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Android
{
	sealed class ListSource : IItemsViewSource
	{
		private List<object> _internal;

		public ListSource()
		{
		}

		public ListSource(IEnumerable<object> enumerable) 
		{
			_internal = new List<object>(enumerable);
		}

		public ListSource(IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				_internal.Add(item);
			}
		}

		public int Count => _internal.Count + (HasHeader ? 1 : 0) + (HasFooter ? 1 : 0);
		public object this[int index] => _internal[AdjustIndexRequest(index)];

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

		int AdjustIndexRequest(int index)
		{
			return index - (HasHeader ? 1 : 0);
		}
	}
}