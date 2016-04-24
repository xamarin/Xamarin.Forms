using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
	public abstract class ItemsView : View, IItemViewController
	{
		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(
				propertyName: "ItemsSource",
				returnType: typeof(IEnumerable),
				declaringType: typeof(ItemsView),
				propertyChanging: (b, o, n) => ((ItemsView)b).OnItemsSourceChanging((IEnumerable)o, (IEnumerable)n)
			);

		public static readonly BindableProperty ItemTemplateProperty =
			BindableProperty.Create(
				propertyName: "ItemTemplate",
				returnType: typeof(DataTemplate),
				declaringType: typeof(ItemsView)
			);

		ItemsSourceProxy _itemSourceProxy;

		public ItemsView()
		{
			_itemSourceProxy = new ItemsSourceProxy(
				itemSource: Enumerable.Empty<object>(),
				itemSourceAsList: OnInitializeItemSource(),
				onCollectionChanged: (s, e) => { }
			);
		}

		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		protected abstract IReadOnlyList<object> OnInitializeItemSource();
		protected abstract IReadOnlyList<object> OnItemsSourceChanging(
			IReadOnlyList<object> itemSource,
			ref NotifyCollectionChangedEventHandler collectionChanged);
		internal virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) { }
		protected virtual DataTemplate GetDataTemplate(object item) => null;

		internal event NotifyCollectionChangedEventHandler CollectionChanged;
		internal void OnItemsSourceChanging(IEnumerable oldValue, IEnumerable newValue)
		{
			// wrap up enumerable, IList, IList<T>, and IReadOnlyList<T>
			var itemSourceAsList = newValue?.ToReadOnlyList();

			// allow interception of itemSource
			NotifyCollectionChangedEventHandler collectionChanged = (s, e) => { };
			itemSourceAsList = OnItemsSourceChanging(itemSourceAsList, ref collectionChanged);
			if (itemSourceAsList == null)
				throw new InvalidOperationException(
					"OnItemsSourceChanging must return non-null itemSource as IReadOnlyList");

			// dispatch CollectionChangedEvent to ItemView without a strong reference to ItemView and
			// synchronize dispatch and element access via CollectionSynchronizationContext protocol
			_itemSourceProxy = new ItemsSourceProxy(newValue, itemSourceAsList, collectionChanged);

			OnItemsSourceChanged(oldValue, newValue);
		}

		event NotifyCollectionChangedEventHandler IItemViewController.CollectionChanged
		{
			add { CollectionChanged += value; }
			remove { CollectionChanged -= value; }
		}
		int IItemViewController.Count => _itemSourceProxy.Count;
		void IItemViewController.BindView(View view, object item) => view.BindingContext = item;
		object IItemViewController.GetItem(int index) => _itemSourceProxy[index];
		View IItemViewController.CreateView(object type)
		{
			object content = ((DataTemplate)type).CreateContent();
			var view = content as View;
			if (view == null)
				throw new InvalidOperationException($"DataTemplate returned non-view content: '{content}'.");

			view.Parent = this;
			return view;
		}
		object IItemViewController.GetItemType(object item)
		{
			// allow interception of DataTemplate resolution
			var dataTemplate = GetDataTemplate(item);
			if (dataTemplate != null)
				return dataTemplate;

			// resolve DataTemplate (possibly via DataTemplateSelector)
			dataTemplate = ItemTemplate;
			var dataTemplateSelector = dataTemplate as DataTemplateSelector;
			if (dataTemplateSelector != null)
				dataTemplate = dataTemplateSelector.SelectTemplate(item, this);

			if (item == null)
				throw new ArgumentException($"No DataTemplate resolved for item: '{item}'.");

			return dataTemplate;
		}

		IItemViewController Controller => this;

		sealed class ItemsSourceProxy
		{
			readonly object _itemSource;
			readonly IReadOnlyList<object> _itemSourceAsList;
			readonly WeakReference<NotifyCollectionChangedEventHandler> _onCollectionChanged;

			internal ItemsSourceProxy(
				object itemSource,
				IReadOnlyList<object> itemSourceAsList,
				NotifyCollectionChangedEventHandler onCollectionChanged)
			{
				_itemSource = itemSource;
				_itemSourceAsList = itemSourceAsList;
				_onCollectionChanged = new WeakReference<NotifyCollectionChangedEventHandler>(onCollectionChanged);

				var dynamicItemSource = itemSource as INotifyCollectionChanged;
				if (dynamicItemSource == null)
					return;

				dynamicItemSource.CollectionChanged += SynchronizeOnCollectionChanged;
			}

			public int Count => _itemSourceAsList.Count;
			public object this[int index]
			{
				get
				{
					// Device.IsInvokeRequired will be false

					if (SyncContext != null)
					{
						object value = null;
						Synchronize(() => value = _itemSourceAsList[index]);
						return value;
					}

					return _itemSourceAsList[index];
				}
			}

			CollectionSynchronizationContext SyncContext
			{
				get
				{
					if (_itemSource == null)
						return null;

					CollectionSynchronizationContext syncContext;
					BindingBase.TryGetSynchronizedCollection((IEnumerable)_itemSource, out syncContext);
					return syncContext;
				}
			}
			void Synchronize(Action action)
			{
				SyncContext.Callback(
					collection: (IEnumerable)_itemSource,
					accessMethod: action
				);
			}
			void SynchronizeOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				if (SyncContext != null)
				{
					Synchronize(() => OnCollectionChanged(sender, e));
					return;
				}

				OnCollectionChanged(sender, e);
			}
			void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				if (Device.IsInvokeRequired)
				{
					Device.BeginInvokeOnMainThread(() => OnCollectionChanged(sender, e));
					return;
				}

				NotifyCollectionChangedEventHandler onCollectionChanged;
				if (!_onCollectionChanged.TryGetTarget(out onCollectionChanged))
				{
					var dynamicItemSource = (INotifyCollectionChanged)_itemSource;
					dynamicItemSource.CollectionChanged -= SynchronizeOnCollectionChanged;
					return;
				}

				onCollectionChanged(sender, e);
			}
		}
	}
}