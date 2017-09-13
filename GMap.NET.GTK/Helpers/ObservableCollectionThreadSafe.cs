using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GMap.NET.GTK.Helpers
{
    public class ObservableCollectionThreadSafe<T> : ObservableCollection<T>
    {
        private NotifyCollectionChangedEventHandler _collectionChanged;

        public override event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                _collectionChanged += value;
            }
            remove
            {
                _collectionChanged -= value;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            using (BlockReentrancy())
            {
                if (_collectionChanged != null)
                {
                    Delegate[] delegates = _collectionChanged.GetInvocationList();

                    foreach (NotifyCollectionChangedEventHandler handler in delegates)
                    {
                        _collectionChanged(this, e);
                    }
                }
            }
        }
    }
}