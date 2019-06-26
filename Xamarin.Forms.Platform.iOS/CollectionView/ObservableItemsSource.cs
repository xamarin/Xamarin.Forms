using System;
using System.Collections;
using System.Collections.Specialized;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class ObservableItemsSource : IItemsViewSource
	{
		readonly UICollectionView _collectionView;
		readonly int _group;
		readonly IList _itemsSource;
		bool _disposed;

		public ObservableItemsSource(IList itemSource, UICollectionView collectionView, int group = 0)
		{
			_collectionView = collectionView;
			_group = group;
			_itemsSource = itemSource;

			((INotifyCollectionChanged)itemSource).CollectionChanged += CollectionChanged;
		}

		public int Count => _itemsSource.Count;

		public object this[int index] => _itemsSource[index];

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
					((INotifyCollectionChanged)_itemsSource).CollectionChanged -= CollectionChanged;
				}

				_disposed = true;
			}
		}

		public int ItemCountInGroup(nint group)
		{
			return _itemsSource.Count;
		}

		public object Group(NSIndexPath indexPath)
		{
			return null;
		}

		public NSIndexPath GetIndexForItem(object item)
		{
			for (int n = 0; n < _itemsSource.Count; n++)
			{
				if (this[n] == item)
				{
					return NSIndexPath.Create(0, n);
				}
			}

			return NSIndexPath.Create(-1, -1);
		}

		public int GroupCount => _itemsSource.Count == 0 ? 0 : 1;

		public int ItemCount => _itemsSource.Count;

		public object this[NSIndexPath indexPath]
		{
			get
			{
				if (indexPath.Section != _group)
				{
					throw new ArgumentOutOfRangeException(nameof(indexPath));
				}

				return this[indexPath.Row];
			}
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
					Reload();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void Reload()
		{
			_collectionView.ReloadData();
			_collectionView.CollectionViewLayout.InvalidateLayout();
		}

		NSIndexPath[] CreateIndexesFrom(int startIndex, int count)
		{
			var result = new NSIndexPath[count];

			for (int n = 0; n < count; n++)
			{
				result[n] = NSIndexPath.Create(_group, startIndex + n);
			}

			return result;
		}

		void Add(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : _itemsSource.IndexOf(args.NewItems[0]);
			var count = args.NewItems.Count;

			_collectionView.InsertItems(CreateIndexesFrom(startIndex, count));			
		}

		void Remove(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.OldStartingIndex;

			if (startIndex < 0)
			{
				// INCC implementation isn't giving us enough information to know where the removed items were in the
				// collection. So the best we can do is a ReloadData()
				Reload();
				return;
			}
	
			// If we have a start index, we can be more clever about removing the item(s) (and get the nifty animations)
			var count = args.OldItems.Count;
			_collectionView.DeleteItems(CreateIndexesFrom(startIndex, count));
		}

		void Replace(NotifyCollectionChangedEventArgs args)
		{
			var newCount = args.NewItems.Count;

			if (newCount == args.OldItems.Count)
			{
				var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : _itemsSource.IndexOf(args.NewItems[0]);

				// We are replacing one set of items with a set of equal size; we can do a simple item range update
				_collectionView.ReloadItems(CreateIndexesFrom(startIndex, newCount));
				return;
			}

			// The original and replacement sets are of unequal size; this means that everything currently in view will 
			// have to be updated. So we just have to use ReloadData and let the UICollectionView update everything
			Reload();
		}

		void Move(NotifyCollectionChangedEventArgs args)
		{
			var count = args.NewItems.Count;

			if (count == 1)
			{
				// For a single item, we can use MoveItem and get the animation
				var oldPath = NSIndexPath.Create(_group, args.OldStartingIndex);
				var newPath = NSIndexPath.Create(_group, args.NewStartingIndex);

				_collectionView.MoveItem(oldPath, newPath);
				return;
			}

			var start = Math.Min(args.OldStartingIndex, args.NewStartingIndex);
			var end = Math.Max(args.OldStartingIndex, args.NewStartingIndex) + count;
			_collectionView.ReloadItems(CreateIndexesFrom(start, end));
		}
	}
}
