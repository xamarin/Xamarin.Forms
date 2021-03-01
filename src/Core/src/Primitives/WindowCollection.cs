using System;
using System.Collections;

namespace Microsoft.Maui
{
    /// <summary>
    /// WindowCollection can be used to interate over all the windows that have been 
    /// opened in the current application.
    /// </summary>
    public sealed class WindowCollection : ICollection
    {
		readonly ArrayList _list;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public WindowCollection()
        {
            _list = new ArrayList(1);
        }

        internal WindowCollection(int count)
        {
            _list = new ArrayList(count);
        }

        /// <summary>
        /// Overloaded [] operator to access the WindowCollection list
        /// </summary>
        public IWindow? this[int index]
        {
            get
            {
                return _list[index] as IWindow;
            }
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// CopyTo
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        void ICollection.CopyTo(Array array, int index)
        {
            _list.CopyTo(array, index);
        }

        /// <summary>
        /// CopyTo
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(IWindow[] array, int index)
        {
            _list.CopyTo(array, index);
        }

        /// <summary>
        /// Count property
        /// </summary>
        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        /// <summary>
        /// IsSynchronized
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return _list.IsSynchronized;
            }
        }

        /// <summary>
        /// SyncRoot
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return _list.SyncRoot;
            }
        }

        internal WindowCollection Clone()
        {
            WindowCollection clone;

            lock (_list.SyncRoot)
            {
                clone = new WindowCollection(_list.Count);
                for (int i = 0; i < _list.Count; i++)
                {
                    clone._list.Add(_list[i]);
                }
            }

            return clone;
        }

        internal void Remove(IWindow win)
        {
            lock (_list.SyncRoot)
            {
                _list.Remove(win);
            }
        }

        internal void RemoveAt(int index)
        {
            lock (_list.SyncRoot)
            {
                _list.Remove(index);
            }
        }

        internal int Add(IWindow win)
        {
            lock (_list.SyncRoot)
            {
                return _list.Add(win);
            }
        }

        internal bool HasItem(IWindow win)
        {
            lock (_list.SyncRoot)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    if (_list[i] == win)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}