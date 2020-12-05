using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;

namespace Xamarin.Forms.Platform.iOS
{
	sealed class ListSource : List<object>, IItemsViewSource
	{
		readonly int _section;

		public ListSource(int section = 0)
		{
			_section = section;
		}

		public ListSource(IEnumerable<object> enumerable, int section = 0) : base(enumerable)
		{
			_section = section;
		}

		public ListSource(IEnumerable enumerable, int section = 0)
		{
			foreach (object item in enumerable)
			{
				Add(item);
			}
			_section = section;
		}

		public void Dispose()
		{

		}

		public object this[NSIndexPath indexPath]
		{
			get
			{
				if (indexPath.Section != _section)
				{
					throw new ArgumentOutOfRangeException(nameof(indexPath));
				}

				return this[(int)indexPath.Item];
			}
		}

		public int GroupCount => 1;

		public int ItemCount => Count;

		public NSIndexPath GetIndexForItem(object item)
		{
			for (int n = 0; n < Count; n++)
			{
				if (this[n] == item)
				{
					return NSIndexPath.Create(0, n);
				}
			}

			return NSIndexPath.Create(-1, -1);
		}

		public object Group(NSIndexPath indexPath)
		{
			return null;
		}

		public int ItemCountInGroup(nint group)
		{
			if (group > 0)
			{
				throw new ArgumentOutOfRangeException(nameof(group));
			}

			return Count;
		}
	}
}