using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Android
{
	internal class ListSource : List<object>, IItemsViewSource
	{
		bool _disposed;

		public ListSource()
		{
		}

		public ListSource(IEnumerable<object> enumerable) : base(enumerable)
		{
			
		}

		public ListSource(IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				Add(item);
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}


		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{

				}

				_disposed = true;
			}
		}
	}
}