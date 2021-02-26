using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Maui
{
	public sealed class WindowCollection : ICollection
	{
		readonly List<IWindow> _windows;

		public WindowCollection()
		{
			_windows = new List<IWindow>();
		}

		public WindowCollection(IEnumerable<IWindow> windows)
		{
			_windows = windows.ToList();
		}

		public int Count => _windows.Count;

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public IEnumerator GetEnumerator()
		{
			return _windows.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}