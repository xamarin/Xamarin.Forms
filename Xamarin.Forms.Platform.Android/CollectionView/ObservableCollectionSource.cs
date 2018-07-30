using System;
using System.Collections;
using System.Collections.Specialized;
using Android.Support.V7.Widget;

namespace Xamarin.Forms.Platform.Android
{
	internal class ObservableCollectionSource : ICollectionViewSource
	{
		// TODO hartez 2018/07/30 14:40:11 We may need to implement IDisposable to make sure this all gets cleaned up	
		readonly RecyclerView.Adapter _adapter;
		readonly IList _itemsSource;

		public ObservableCollectionSource(IEnumerable itemSource, RecyclerView.Adapter adapter)
		{
			_itemsSource = (IList)itemSource;
			_adapter = adapter;

			((INotifyCollectionChanged)itemSource).CollectionChanged += CollectionChanged;
		}

		void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					break;
				case NotifyCollectionChangedAction.Remove:
					Remove(args);
					break;
				case NotifyCollectionChangedAction.Replace:
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Reset:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void Remove(NotifyCollectionChangedEventArgs args)
		{
			if (args.OldStartingIndex > -1)
			{
				_adapter.NotifyItemRangeRemoved(args.OldStartingIndex, args.OldItems.Count);
				return;
			}

			var startIndex = _itemsSource.IndexOf(args.OldItems[0]);

			if (args.OldItems.Count == 1)
			{
				_adapter.NotifyItemRemoved(startIndex);
				return;
			}

			_adapter.NotifyItemRangeRemoved(startIndex, args.OldItems.Count);
		}


		public int Count => _itemsSource.Count;

		public object this[int index] => _itemsSource[index];
	}
}