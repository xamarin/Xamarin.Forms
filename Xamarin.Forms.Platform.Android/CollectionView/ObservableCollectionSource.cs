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
			// TODO hartez 2018/07/31 16:02:50 Handle the reset of these cases (implementing selection will make them much easier to test)	
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					Add(args);
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

		void Add(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : _itemsSource.IndexOf(args.NewItems[0]);
			var count = args.NewItems.Count;

			if (count == 1)
			{
				_adapter.NotifyItemInserted(startIndex);
				return;
			}

			_adapter.NotifyItemRangeInserted(startIndex, count);
		}

		void Remove(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.OldStartingIndex > -1 ? args.OldStartingIndex : _itemsSource.IndexOf(args.OldItems[0]);
			var count = args.OldItems.Count;

			if (count == 1)
			{
				_adapter.NotifyItemRemoved(startIndex);
				return;
			}

			_adapter.NotifyItemRangeRemoved(startIndex, count);
		}


		public int Count => _itemsSource.Count;

		public object this[int index] => _itemsSource[index];
	}
}