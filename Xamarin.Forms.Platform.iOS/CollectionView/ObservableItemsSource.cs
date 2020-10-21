using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	class ObservableItemsSource : IItemsViewSource
	{
		readonly UICollectionViewController _collectionViewController;
		readonly UICollectionView _collectionView;
		readonly bool _grouped;
		readonly int _section;
		readonly List<object> _internalItemsSource;
		readonly IEnumerable _originalItemsSource;
		bool _disposed;

		public ObservableItemsSource(IEnumerable itemSource, UICollectionViewController collectionViewController,
			int group = -1)
		{
			_collectionViewController = collectionViewController;
			_collectionView = _collectionViewController.CollectionView;

			_section = group < 0 ? 0 : group;
			_grouped = group >= 0;

			// we basically keep a copy of the original items source, so that we have control over
			// when updates are applied. This is necessary when updates to the collection
			// view are performed asynchronously -> they may lead to an inconsistent state.
			_internalItemsSource = itemSource.Cast<object>().ToList();
			_originalItemsSource = itemSource;

			Count = ItemsCount();

			((INotifyCollectionChanged)itemSource).CollectionChanged += CollectionChanged;
		}

		internal event NotifyCollectionChangedEventHandler CollectionItemsSourceChanged
		{
			add
			{
				if (_originalItemsSource is INotifyCollectionChanged incc) incc.CollectionChanged += value;
			}
			remove
			{
				if (_originalItemsSource is INotifyCollectionChanged incc) incc.CollectionChanged -= value;
			}
		}

		public int Count { get; private set; }

		public object this[int index] => ElementAt(index);

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
					((INotifyCollectionChanged)_originalItemsSource).CollectionChanged -= CollectionChanged;
				}

				_disposed = true;
			}
		}

		public int ItemCountInGroup(nint group)
		{
			return Count;
		}

		public object Group(NSIndexPath indexPath)
		{
			return null;
		}

		public NSIndexPath GetIndexForItem(object item)
		{
			for (int n = 0; n < Count; n++)
			{
				if (this[n] == item)
				{
					return NSIndexPath.Create(_section, n);
				}
			}

			return NSIndexPath.Create(-1, -1);
		}

		public int GroupCount => 1;

		public int ItemCount => Count;

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

		async void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (Device.IsInvokeRequired)
			{
				await Device.InvokeOnMainThreadAsync(() => CollectionChanged(args));
			}
			else
			{
				CollectionChanged(args);
			}
		}

		void CollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (NotLoadedYet())
			{
				Reload();
				return;
			}

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
			// update the internal items source
			_internalItemsSource.Clear();
			_internalItemsSource.AddRange(_originalItemsSource.Cast<object>());
			Count = ItemsCount();

			// update the collection view without using animation to avoid concurrency
			// code source: https://stackoverflow.com/a/64146094/13005218
			UIView.PerformWithoutAnimation(() =>
			{
				// [!] Do not use BatchUpdate here, it will cause concurrency problems
				_collectionView.ReloadData();
			});
			_collectionView.CollectionViewLayout.InvalidateLayout();
		}

		NSIndexPath[] CreateIndexesFrom(int startIndex, int count)
		{
			var result = new NSIndexPath[count];

			for (int n = 0; n < count; n++)
			{
				result[n] = NSIndexPath.Create(_section, startIndex + n);
			}

			return result;
		}

		void Add(NotifyCollectionChangedEventArgs args)
		{
			// UICollectionView doesn't like when we insert items into a completely empty un-grouped CV,
			// and it doesn't like when we insert items into a grouped CV with no actual cells (just empty groups)
			// In those circumstances, we just need to ask it to reload the data so it can get its internal
			// accounting in order

			if (!_grouped && _collectionView.NumberOfItemsInSection(_section) == 0 ||
			    _collectionView.VisibleCells.Length == 0)
			{
				Reload();
				return;
			}

			var count = args.NewItems.Count;
			Count += count;
			var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : IndexOf(args.NewItems[0]);

			// update the internal items source
			_internalItemsSource.InsertRange(args.NewStartingIndex, args.NewItems.Cast<object>());
			// update the collection view
			// [!] Do not use BatchUpdate here, it will cause concurrency problems
			_collectionView.InsertItems(CreateIndexesFrom(startIndex, count));
		}

		void Remove(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.OldStartingIndex;
			if (startIndex < 0)
			{
				startIndex = _internalItemsSource.IndexOf(args.OldItems[0]);
			}

			// If we have a start index, we can be more clever about removing the item(s) (and get the nifty animations)
			var count = args.OldItems.Count;
			Count -= count;

			// update the internal items source
			_internalItemsSource.RemoveRange(args.OldStartingIndex, args.OldItems.Count);
			// update the collection view
			// [!] Do not use BatchUpdate here, it will cause concurrency problems
			_collectionView.DeleteItems(CreateIndexesFrom(startIndex, count));
		}

		void Replace(NotifyCollectionChangedEventArgs args)
		{
			var newCount = args.NewItems.Count;
			if (newCount == args.OldItems.Count)
			{
				// We are replacing one set of items with a set of equal size; we can do a simple item range update
				var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : IndexOf(args.NewItems[0]);

				// update the internal items source
				_internalItemsSource.RemoveRange(args.OldStartingIndex, args.OldItems.Count);
				_internalItemsSource.InsertRange(args.OldStartingIndex, args.NewItems.Cast<object>());

				// update the collection view
				// [!] Do not use BatchUpdate here, it will cause concurrency problems
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
				var oldPath = NSIndexPath.Create(_section, args.OldStartingIndex);
				var newPath = NSIndexPath.Create(_section, args.NewStartingIndex);

				// update the internal items source
				var item = _internalItemsSource[args.OldStartingIndex];
				_internalItemsSource.Remove(item);
				_internalItemsSource.Insert(args.NewStartingIndex, item);

				// update the collection view
				// [!] Do not use BatchUpdate here, it will cause concurrency problems
				_collectionView.MoveItem(oldPath, newPath);
				return;
			}

			var start = Math.Min(args.OldStartingIndex, args.NewStartingIndex);
			var end = Math.Max(args.OldStartingIndex + args.OldItems.Count, args.NewStartingIndex + count);
			// [!] Do not use BatchUpdate here, it will cause concurrency problems
			_collectionView.ReloadItems(CreateIndexesFrom(start, end));
		}

		internal int ItemsCount() => _internalItemsSource.Count;

		internal object ElementAt(int index) => _internalItemsSource[index];

		internal int IndexOf(object item) => _internalItemsSource.IndexOf(item);

		bool NotLoadedYet()
		{
			// If the UICollectionView hasn't actually been loaded, then calling InsertItems or DeleteItems is 
			// going to crash or get in an unusable state; instead, ReloadData should be used
			return !_collectionViewController.IsViewLoaded || _collectionViewController.View?.Window is null;
		}
	}
}