using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Xamarin.Forms
{
	public class ResourceDictionary : IResourceDictionary, IDictionary<string, object>
	{
		static readonly ConditionalWeakTable<Type, ResourceDictionary> s_instances = new ConditionalWeakTable<Type, ResourceDictionary>();
		readonly Dictionary<string, object> _innerDictionary = new Dictionary<string, object>();
		ResourceDictionary _mergedInstance;
		Type _mergedWith;

		[TypeConverter (typeof(TypeTypeConverter))]
		public Type MergedWith {
			get => _mergedWith;
			set {
				if (_mergedWith == value)
					return;

				if (!typeof(ResourceDictionary).GetTypeInfo().IsAssignableFrom(value.GetTypeInfo()))
					throw new ArgumentException("MergedWith should inherit from ResourceDictionary");

				_mergedWith = value;
				if (_mergedWith == null)
					return;

				_mergedInstance = s_instances.GetValue(_mergedWith, (key) => (ResourceDictionary)Activator.CreateInstance(key));
				OnValuesChanged(_mergedInstance);
			}
		}

		ICollection<ResourceDictionary> _mergedDictionaries;
		public ICollection<ResourceDictionary> MergedDictionaries => 
			_mergedDictionaries ?? (_mergedDictionaries = CreateMergedDictionariesCollection());

		public ICollection<ResourceDictionary> CreateMergedDictionariesCollection()
		{
			var collection = new ObservableCollection<ResourceDictionary>();
			collection.CollectionChanged += MergedDictionaries_CollectionChanged;
			return collection;
		}

		IList<ResourceDictionary> _collectionTrack;

		void MergedDictionaries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// Move() isn't exposed by ICollection
			if (e.Action == NotifyCollectionChangedAction.Move)
				return;

			_collectionTrack = _collectionTrack ?? new List<ResourceDictionary>();
			// Collection has been cleared
			if (e.Action == NotifyCollectionChangedAction.Reset) {
				foreach (var dictionary in _collectionTrack)
					dictionary.ValuesChanged -= Item_ValuesChanged;

				_collectionTrack.Clear();
				return;
			}

			// New Items
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var rd = (ResourceDictionary)item;
					_collectionTrack.Add(rd);
					rd.ValuesChanged += Item_ValuesChanged;
					OnValuesChanged(rd);
				}
			}

			// Old Items
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var rd = (ResourceDictionary)item;
					rd.ValuesChanged -= Item_ValuesChanged;
					_collectionTrack.Remove(rd);
				}
			}
		}

		void Item_ValuesChanged(object sender, ResourcesChangedEventArgs e) => OnValuesChanged(e.Values);

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			((ICollection<KeyValuePair<string, object>>)_innerDictionary).Add(item);
			OnValuesChanged(item);
		}

		public void Clear() => _innerDictionary.Clear();

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) =>
			((ICollection<KeyValuePair<string, object>>) _innerDictionary).Contains(item)
			|| _mergedInstance != null && _mergedInstance.Contains(item);

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => 
			((ICollection<KeyValuePair<string, object>>)_innerDictionary).CopyTo(array, arrayIndex);

		public int Count => _innerDictionary.Count;

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly => 
			((ICollection<KeyValuePair<string, object>>)_innerDictionary).IsReadOnly;

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) => 
			((ICollection<KeyValuePair<string, object>>)_innerDictionary).Remove(item);

		public void Add(string key, object value)
		{
			if (ContainsKey(key))
				throw new ArgumentException($"A resource with the key '{key}' is already present in the ResourceDictionary.");
			_innerDictionary.Add(key, value);
			OnValueChanged(key, value);
		}

		public bool ContainsKey(string key) => _innerDictionary.ContainsKey(key);

		[IndexerName("Item")]
		public object this[string index]
		{
			get => TryGetValue(index, out var value)
				? value
				: throw new KeyNotFoundException($"The resource '{index}' is not present in the dictionary.");
			set
			{
				_innerDictionary[index] = value;
				OnValueChanged(index, value);
			}
		}

		public ICollection<string> Keys => _innerDictionary.Keys;

		public bool Remove(string key) => _innerDictionary.Remove(key);

		public ICollection<object> Values => _innerDictionary.Values;

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _innerDictionary.GetEnumerator();

		internal IEnumerable<KeyValuePair<string, object>> MergedResources {
			get {
				if (_mergedDictionaries != null)
					foreach (var r in _mergedDictionaries.Reverse().SelectMany(x => x.MergedResources))
						yield return r;
				if (_mergedInstance != null)
					foreach (var r in _mergedInstance.MergedResources)
						yield return r;
				foreach (var r in _innerDictionary)
					yield return r;
			}
		}

		public bool TryGetValue(string key, out object value)
		{
			var containsKey = _innerDictionary.TryGetValue(key, out var outValue)
			             || _mergedInstance != null && _mergedInstance.TryGetValue(key, out outValue)
			             || _mergedDictionaries != null && _mergedDictionaries.Reverse().Any(d => d.TryGetValue(key, out outValue));
			value = outValue;
			return containsKey;
		}

		event EventHandler<ResourcesChangedEventArgs> IResourceDictionary.ValuesChanged
		{
			add => ValuesChanged += value;
			remove => ValuesChanged -= value;
		}

		public void Add(Style style)
		{
			if (string.IsNullOrEmpty(style.Class))
				Add(style.TargetType.FullName, style);
			else
			{
				IList<Style> classes;
				if (!TryGetValue(Style.StyleClassPrefix + style.Class, out var outClasses)
				    || (classes = outClasses as IList<Style>) == null)
					classes = new List<Style>();
				classes.Add(style);
				this[Style.StyleClassPrefix + style.Class] = classes;
			}
		}

		void OnValueChanged(string key, object value) =>
			OnValuesChanged(new KeyValuePair<string, object>(key, value));

		void OnValuesChanged(KeyValuePair<string, object> value) =>
			ValuesChanged?.Invoke(this, new ResourcesChangedEventArgs(new[] {value}));

		void OnValuesChanged(IEnumerable<KeyValuePair<string, object>> values) =>
			ValuesChanged?.Invoke(this, new ResourcesChangedEventArgs(values.Select(v => v)));

		event EventHandler<ResourcesChangedEventArgs> ValuesChanged;
	}
}