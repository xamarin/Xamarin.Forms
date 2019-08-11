using System;
using System.Collections;
using System.Collections.Specialized;
using Android.Support.V7.Widget;

namespace Xamarin.Forms.Platform.Android
{
	internal class ObservableItemsSource : IItemsViewSource
	{
		readonly RecyclerView.Adapter _adapter;
		readonly IList _itemsSource;
		bool _disposed;

		public ObservableItemsSource(IList itemSource, RecyclerView.Adapter adapter)
		{
			_itemsSource = itemSource;
			_adapter = adapter;

			((INotifyCollectionChanged)itemSource).CollectionChanged += CollectionChanged;
		}

		public int Count => _itemsSource.Count + (HasHeader ? 1 : 0) + (HasFooter ? 1 : 0);
		public object this[int index] => _itemsSource[AdjustIndexRequest(index)];

		public bool HasHeader { get; set; }
		public bool HasFooter { get; set; }

		public void Dispose()
		{
			Dispose(true);
		}

		public bool IsFooter(int index)
		{
			if (!HasFooter)
			{
				return false;
			}

			if (HasHeader)
			{
				return index == _itemsSource.Count + 1;
			}

			return index == _itemsSource.Count;
		}

		public bool IsHeader(int index)
		{
			return HasHeader && index == 0;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					((INotifyCollectionChanged)_itemsSource).CollectionChanged -= CollectionChanged;
				}

				_disposed = true;
			}
		}

		int AdjustIndexRequest(int index)
		{
			return index - (HasHeader ? 1 : 0);
		}

		int AdjustNotifyIndex(int index)
		{
			return index + (HasHeader ? 1 : 0);
		}

		void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					Add(args);
					break;
				case NotifyCollectionChangedAction.Remove:
					Remove(args);
					break;
				case NotifyCollectionChangedAction.Replace:
					Replace(args);
					break;
				case NotifyCollectionChangedAction.Move:
					Move(args);
					break;
				case NotifyCollectionChangedAction.Reset:
					_adapter.NotifyDataSetChanged();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void Move(NotifyCollectionChangedEventArgs args)
		{
			var count = args.NewItems.Count;

			if (count == 1)
			{
				// For a single item, we can use NotifyItemMoved and get the animation
				_adapter.NotifyItemMoved(AdjustNotifyIndex(args.OldStartingIndex), AdjustNotifyIndex(args.NewStartingIndex));
				return;
			}

			var start = AdjustNotifyIndex(Math.Min(args.OldStartingIndex, args.NewStartingIndex));
			var end = AdjustNotifyIndex(Math.Max(args.OldStartingIndex, args.NewStartingIndex) + count);
			_adapter.NotifyItemRangeChanged(start, end);
		}

		void Add(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : _itemsSource.IndexOf(args.NewItems[0]);
			startIndex = AdjustNotifyIndex(startIndex);
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
			var startIndex = args.OldStartingIndex;

			if (startIndex < 0)
			{
				// INCC implementation isn't giving us enough information to know where the removed items were in the
				// collection. So the best we can do is a NotifyDataSetChanged()
				_adapter.NotifyDataSetChanged();
				return;
			}

			startIndex = AdjustNotifyIndex(startIndex);

			// If we have a start index, we can be more clever about removing the item(s) (and get the nifty animations)
			var count = args.OldItems.Count;

			if (count == 1)
			{
				_adapter.NotifyItemRemoved(startIndex);
				return;
			}

			_adapter.NotifyItemRangeRemoved(startIndex, count);
		}

		void Replace(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : _itemsSource.IndexOf(args.NewItems[0]);
			startIndex = AdjustNotifyIndex(startIndex);
			var newCount = args.NewItems.Count;

			if (newCount == args.OldItems.Count)
			{
				// We are replacing one set of items with a set of equal size; we can do a simple item or range 
				// notification to the adapter
				if (newCount == 1)
				{
					_adapter.NotifyItemChanged(startIndex);
				}
				else
				{
					_adapter.NotifyItemRangeChanged(startIndex, newCount);
				}

				return;
			}
			
			// The original and replacement sets are of unequal size; this means that everything currently in view will 
			// have to be updated. So we just have to use NotifyDataSetChanged and let the RecyclerView update everything
			_adapter.NotifyDataSetChanged();
		}
	}
}