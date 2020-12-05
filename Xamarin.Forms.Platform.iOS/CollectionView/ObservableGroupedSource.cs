using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class ObservableGroupedSource : IItemsViewSource
	{
		readonly UICollectionView _collectionView;
		readonly UICollectionViewController _collectionViewController;
		readonly IList _originalItemsSource;
		bool _disposed;
		readonly List<IItemsViewSource> _groups = new List<IItemsViewSource>();

		public ObservableGroupedSource(IEnumerable groupSource, UICollectionViewController collectionViewController)
		{
			_collectionViewController = collectionViewController;
			_collectionView = _collectionViewController.CollectionView;
			
			_originalItemsSource = groupSource as IList ?? new ListSource(groupSource);

			if (_originalItemsSource is INotifyCollectionChanged incc)
			{
				incc.CollectionChanged += CollectionChanged;
			}

			ResetGroups();
		}

		public object this[NSIndexPath indexPath] => _groups[indexPath.Section][indexPath];

		public int GroupCount => _groups.Count;

		public int ItemCount => _groups.Sum(g => g.ItemCount);

		public NSIndexPath GetIndexForItem(object item)
		{
			foreach (var group in _groups)
			{
				var index = group.GetIndexForItem(item);
				if (index.Item == -1)
				{
					continue;
				}

				return index;
			}

			return NSIndexPath.Create(-1, -1);
		}

		public object Group(NSIndexPath indexPath) => _originalItemsSource[indexPath.Section];

		public int ItemCountInGroup(nint group) => _groups[(int)group].ItemCount;

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				ClearGroups();
				if (_originalItemsSource is INotifyCollectionChanged incc)
				{
					incc.CollectionChanged -= CollectionChanged;
				}
			}
		}

		void ClearGroups()
		{
			for (int n = _groups.Count - 1; n >= 0; n--)
			{
				_groups[n].Dispose();
				_groups.RemoveAt(n);
			}
		}

		void ResetGroups()
		{
			ClearGroups();

			var index = 0;
			foreach (var group in _originalItemsSource)
			{
				if (group is IEnumerable list)
				{
					if (group is INotifyCollectionChanged)
					{
						_groups.Add(new ObservableItemsSource(list, _collectionViewController, index));
					}
					else
					{
						_groups.Add(new ListSource(list, index));
					}
				}
				else
				{
					_groups.Add(new ListSource(index));
				}
				index++;
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
			ResetGroups();

			// update the collection view without using animation to avoid concurrency
			// code source: https://stackoverflow.com/a/64146094/13005218
			UIView.PerformWithoutAnimation(() =>
			{
				// [!] Do not use BatchUpdate here, it will cause concurrency problems
				_collectionView.ReloadData();
			});
			_collectionView.CollectionViewLayout.InvalidateLayout();
		}

		NSIndexSet CreateIndexSetFrom(int startIndex, int count)
		{
			return NSIndexSet.FromNSRange(new NSRange(startIndex, count));
		}

		bool NotLoadedYet()
		{
			// If the UICollectionView hasn't actually been loaded, then calling InsertSections or DeleteSections is 
			// going to crash or get in an unusable state; instead, ReloadData should be used
			return !_collectionViewController.IsViewLoaded || _collectionViewController.View?.Window == null;
		}

		void Add(NotifyCollectionChangedEventArgs args)
		{
			if (_collectionView.NumberOfSections() == 0 || _collectionView.VisibleCells.Length == 0)
			{
				Reload();
				return;
			}

			var startIndex = args.NewStartingIndex > -1
				? args.NewStartingIndex
				: _originalItemsSource.IndexOf(args.NewItems[0]);
			var count = args.NewItems.Count;

			// Adding a group will change the section index for all subsequent groups, so the easiest thing to do
			// is to reset all the group tracking to get it up-to-date
			ResetGroups();

			// apply the updates to the UICollectionView
			// [!] Do not use BatchUpdate here, it will cause concurrency problems
			_collectionView.InsertSections(CreateIndexSetFrom(startIndex, count));
		}

		void Remove(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.OldStartingIndex;

			if (startIndex < 0)
			{
				// INCC implementation isn't giving us enough information to know where the removed items were in the
				// collection. So the best we can do is a complete reload
				Reload();
				return;
			}

			// Removing a group will change the section index for all subsequent groups, so the easiest thing to do
			// is to reset all the group tracking to get it up-to-date
			ResetGroups();

			// Since we have a start index, we can be more clever about removing the item(s) (and get the nifty animations)
			var count = args.OldItems.Count;

			// apply the updates to the UICollectionView
			// [!] Do not use BatchUpdate here, it will cause concurrency problems
			_collectionView.DeleteSections(CreateIndexSetFrom(startIndex, count));
		}

		void Replace(NotifyCollectionChangedEventArgs args)
		{
			var newCount = args.NewItems.Count;

			if (newCount == args.OldItems.Count)
			{
				ResetGroups();

				var startIndex = args.NewStartingIndex > -1
					? args.NewStartingIndex
					: _originalItemsSource.IndexOf(args.NewItems[0]);

				// We are replacing one set of items with a set of equal size; we can do a simple item range update
				// [!] Do not use BatchUpdate here, it will cause concurrency problems
				_collectionView.ReloadSections(CreateIndexSetFrom(startIndex, newCount));
				return;
			}

			// The original and replacement sets are of unequal size; this means that everything currently in view will 
			// have to be updated. So we just have to use ReloadData and let the UICollectionView update everything
			Reload();
		}

		void Move(NotifyCollectionChangedEventArgs args)
		{
			var count = args.NewItems.Count;

			ResetGroups();

			if (count == 1)
			{
				// For a single item, we can use MoveSection and get the animation
				// [!] Do not use BatchUpdate here, it will cause concurrency problems
				_collectionView.MoveSection(args.OldStartingIndex, args.NewStartingIndex);
				return;
			}

			var start = Math.Min(args.OldStartingIndex, args.NewStartingIndex);
			var end = Math.Max(args.OldStartingIndex, args.NewStartingIndex) + count;

			// [!] Do not use BatchUpdate here, it will cause concurrency problems
			_collectionView.ReloadSections(CreateIndexSetFrom(start, end));
		}
	}
}
