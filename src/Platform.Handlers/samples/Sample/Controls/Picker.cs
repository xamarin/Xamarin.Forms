using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Platform;

namespace Sample
{
	public class Picker : Xamarin.Forms.View, IPicker
	{
		IList _oldItemsSource;
		IList _newItemsSource;
		object _selectedItem;

		public Picker()
		{
			((INotifyCollectionChanged)Items).CollectionChanged += OnItemsCollectionChanged;
		}

		public string Title { get; set; }

		public Color TitleColor { get; set; }

		public IList<string> Items { get; } = new LockableObservableListWrapper();

		public IList ItemsSource
		{
			get { return _newItemsSource; }
			set
			{
				_newItemsSource = value;
				UpdateItemsSource(_oldItemsSource, _newItemsSource);
				_oldItemsSource = _newItemsSource;
			}
		}

		public int SelectedIndex { get; set; } = -1;

		public object SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				UpdateSelectedIndex(_selectedItem);
			}
		}

		public string Text { get; set; }

		public Color TextColor { get; set; }

		void UpdateItemsSource(IList oldValue, IList newValue)
		{
			if (oldValue is INotifyCollectionChanged oldObservable)
				oldObservable.CollectionChanged -= OnCollectionChanged;

			if (newValue is INotifyCollectionChanged newObservable)
				newObservable.CollectionChanged += OnCollectionChanged;
			
			if (newValue != null)
			{
				((LockableObservableListWrapper)Items).IsLocked = true;
				ResetItems();
			}
			else
			{
				((LockableObservableListWrapper)Items).InternalClear();
				((LockableObservableListWrapper)Items).IsLocked = false;
			}
		}

		void UpdateSelectedIndex(object selectedItem)
		{
			if (ItemsSource != null)
			{
				SelectedIndex = ItemsSource.IndexOf(selectedItem);
				return;
			}
			SelectedIndex = Items.IndexOf(selectedItem);
		}

		void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var oldIndex = SelectedIndex;
			var newIndex = SelectedIndex = SelectedIndex.Clamp(-1, Items.Count - 1);

			// If the index has not changed, still need to change the selected item
			if (newIndex == oldIndex)
				UpdateSelectedItem(newIndex);
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					AddItems(e);
					break;
				case NotifyCollectionChangedAction.Remove:
					RemoveItems(e);
					break;
				default: //Move, Replace, Reset
					ResetItems();
					break;
			}
		}

		void AddItems(NotifyCollectionChangedEventArgs e)
		{
			int index = e.NewStartingIndex < 0 ? Items.Count : e.NewStartingIndex;

			foreach (object newItem in e.NewItems)
				((LockableObservableListWrapper)Items).InternalInsert(index++, GetDisplayMember(newItem));
		}

		void RemoveItems(NotifyCollectionChangedEventArgs e)
		{
			int index = e.OldStartingIndex < Items.Count ? e.OldStartingIndex : Items.Count;

			foreach (object _ in e.OldItems)
				((LockableObservableListWrapper)Items).InternalRemoveAt(index--);
		}

		void ResetItems()
		{
			if (ItemsSource == null)
				return;

			((LockableObservableListWrapper)Items).InternalClear();

			foreach (object item in ItemsSource)
				((LockableObservableListWrapper)Items).InternalAdd(GetDisplayMember(item));

			UpdateSelectedItem(SelectedIndex);
		}

		string GetDisplayMember(object item)
		{
			return item == null ? string.Empty : item.ToString();
		}

		void UpdateSelectedItem(int index)
		{
			if (index == -1)
			{
				SelectedItem = null;
				return;
			}

			if (ItemsSource != null)
			{
				SelectedItem = ItemsSource[index];
				return;
			}

			SelectedItem = Items[index];
		}
	}
}