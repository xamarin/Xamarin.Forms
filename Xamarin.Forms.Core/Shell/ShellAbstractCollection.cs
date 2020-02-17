using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Xamarin.Forms
{
	internal abstract class ShellAbstractCollection<T> : IList<T>, INotifyCollectionChanged where T : class
	{
		protected readonly ObservableCollection<T> _visibleContents = new ObservableCollection<T>();
		protected IList<T> _inner = new ObservableCollection<T>();

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event NotifyCollectionChangedEventHandler VisibleItemsChanged;

		internal IList<T> Inner
		{
			get => _inner;
			set => SetInnerCollection(ref _inner, value);
		}

		public ReadOnlyCollection<T> VisibleItems { get; }

		public int Count => Inner.Count;

		public bool IsReadOnly => Inner.IsReadOnly;

		protected ShellAbstractCollection()
		{
			VisibleItems = new ReadOnlyCollection<T>(_visibleContents);
			_visibleContents.CollectionChanged += (_, args) =>
			{
				VisibleItemsChanged?.Invoke(VisibleItems, args);
			};
		}

		#region IList

		public T this[int index]
		{
			get => Inner[index];
			set => Inner[index] = value;
		}

		public virtual void Clear()
		{
			var list = Inner.ToList();
			Removing(Inner);
			Inner.Clear();
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list));
		}

		public virtual void Add(T item) => Inner.Add(item);

		public virtual bool Contains(T item) => Inner.Contains(item);

		public virtual void CopyTo(T[] array, int arrayIndex) => Inner.CopyTo(array, arrayIndex);

		public virtual IEnumerator<T> GetEnumerator() => Inner.GetEnumerator();

		public virtual int IndexOf(T item) => Inner.IndexOf(item);

		public virtual void Insert(int index, T item) => Inner.Insert(index, item);

		public virtual bool Remove(T item) => Inner.Remove(item);

		public virtual void RemoveAt(int index) => Inner.RemoveAt(index);

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Inner).GetEnumerator();

		#endregion

		void InnerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (T element in e.NewItems)
				{
					if (element is IElementController controller)
						OnElementControllerInserting(controller);

					CheckVisibility(element);
				}
			}

			if (e.OldItems != null)
			{
				Removing(e.OldItems);
			}

			CollectionChanged?.Invoke(this, e);
		}

		void Removing(IEnumerable items)
		{
			foreach (T element in items)
			{
				if (_visibleContents.Contains(element))
					_visibleContents.Remove(element);

				if (element is IElementController controller)
					OnElementControllerRemoving(controller);
			}
		}

		void SetInnerCollection(ref IList<T> field, IList<T> newValue)
		{
			CheckEvents(ref field, ref newValue);
			field = newValue;
		}

		void CheckEvents(ref IList<T> oldValue, ref IList<T> newValue)
		{
			if (oldValue is INotifyCollectionChanged oldObservable)
				oldObservable.CollectionChanged -= InnerCollectionChanged;

			if (newValue is INotifyCollectionChanged newObservable)
				newObservable.CollectionChanged += InnerCollectionChanged;
		}

		protected abstract void CheckVisibility(T element);

		protected abstract void OnElementControllerInserting(IElementController controller);

		protected abstract void OnElementControllerRemoving(IElementController controller);
	}
}
