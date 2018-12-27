using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class MultiTestObservableCollection<T> : List<T>, INotifyCollectionChanged
	{
		// This is a testing class which implements INotifyCollectionChanged and, unlike the regular
		// ObservableCollection, will actually fire Add and Remove with multiple items at once

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public void TestAddWithList(IEnumerable<T> newItems, int insertAt)
		{
			List<T> list = newItems.ToList();
			InsertRange(insertAt, list);
			OnNotifyCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
		}

		public void TestRemoveWithList(int removeStart, int count)
		{
			List<T> list = new List<T>(GetRange(removeStart, count));
			RemoveRange(removeStart, count);
			OnNotifyCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list));
		}

		public void TestAddWithListAndIndex(IEnumerable<T> newItems, int insertAt)
		{
			List<T> list = newItems.ToList();
			InsertRange(insertAt, newItems);
			OnNotifyCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, insertAt));
		}

		public void TestRemoveWithListAndIndex(int removeStart, int count)
		{
			List<T> list = new List<T>(GetRange(removeStart, count));
			RemoveRange(removeStart, count);
			OnNotifyCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list, removeStart));
		}

		private void OnNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
		{
			CollectionChanged?.Invoke(this, eventArgs);
		}
	}
}