using System;

namespace Xamarin.Forms.Platform.Android
{
	sealed internal class EmptySource : IItemsViewSource
	{
		public int Count => 0;

		public bool HasHeader { get; set; }
		public bool HasFooter { get; set; }

		public object this[int index] => throw new IndexOutOfRangeException("IItemsViewSource is empty");

		public void Dispose()
		{

		}

		public bool IsHeader(int index)
		{
			return HasHeader && index == 0;
		}

		public bool IsFooter(int index)
		{
			if (!HasFooter)
			{
				return false;
			}

			if (HasHeader)
			{
				return index == 1;
			}

			return index == 0;
		}
	}
}