using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform.Hosting
{
	class MauiServiceCollection : IMauiServiceCollection
	{
		static string s_error => "This Collection is based on a non ordered Dictionary";
		Dictionary<Type, Func<IServiceProvider, object>> Factories = new Dictionary<Type, Func<IServiceProvider, object>>();
		List<ServiceDescriptor> _descriptors = new List<ServiceDescriptor>();

		public int Count => _descriptors.Count;

		public bool IsReadOnly => false;

		public ICollection<Type> Keys => Factories.Keys;

		public ICollection<Func<IServiceProvider, object>> Values => Factories.Values;

		public Func<IServiceProvider, object> this[Type key] { get => Factories[key]; set => Factories[key] = value; }

		public void Add(ServiceDescriptor item)
		{
			if (!_descriptors.Contains(item))
				_descriptors.Add(item);
			if (item.ImplementationType != null)
				Factories[item.ServiceType] = new Func<IServiceProvider, object>(provider => Activator.CreateInstance(item.ImplementationType));
			else if (item.ImplementationInstance != null)
				Factories[item.ServiceType] = new Func<IServiceProvider, object>(provider => item.ImplementationInstance);
			else if (item.ImplementationFactory != null)
				Factories[item.ServiceType] = item.ImplementationFactory;
			else
				throw new InvalidOperationException($"You need to provide an {nameof(item.ImplementationType)} or a {nameof(item.ImplementationInstance)} ");
		}

		public void Clear()
		{
			_descriptors.Clear();
			Factories.Clear();
		}

		public bool Contains(ServiceDescriptor item) => _descriptors.Contains(item);

		public IEnumerator<ServiceDescriptor> GetEnumerator() => _descriptors.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _descriptors.GetEnumerator();

		public bool Remove(ServiceDescriptor item)
		{
			if (_descriptors.Contains(item))
				_descriptors.Remove(item);
			if (Factories.ContainsKey(item.ServiceType))
				return Factories.Remove(item.ServiceType);

			return false;
		}

		public ServiceDescriptor this[int index]
		{
			get
			{
				throw new NotImplementedException(s_error);
			}
			set
			{
				throw new NotImplementedException(s_error);
			}

		}

		public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => throw new NotImplementedException(s_error);

		public int IndexOf(ServiceDescriptor item) => throw new NotImplementedException(s_error);

		public void Insert(int index, ServiceDescriptor item) => throw new NotImplementedException(s_error);

		public void RemoveAt(int index) => throw new NotImplementedException(s_error);

		public void Add(Type key, Func<IServiceProvider, object> value)
			=> Factories.Add(key, value);

		public bool ContainsKey(Type key) => Factories.ContainsKey(key);

		public bool Remove(Type key) => Factories.Remove(key);

		public bool TryGetValue(Type key, out Func<IServiceProvider, object> value)
			=> Factories.TryGetValue(key, out value);

		public void Add(KeyValuePair<Type, Func<IServiceProvider, object>> item)
			=> Factories.Add(item.Key, item.Value);

		public bool Contains(KeyValuePair<Type, Func<IServiceProvider, object>> item)
			=> Factories.Contains(item);

		public void CopyTo(KeyValuePair<Type, Func<IServiceProvider, object>>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<Type, Func<IServiceProvider, object>> item)
			=> Factories.Remove(item.Key);

		IEnumerator<KeyValuePair<Type, Func<IServiceProvider, object>>> IEnumerable<KeyValuePair<Type, Func<IServiceProvider, object>>>.GetEnumerator()
			=> Factories.GetEnumerator();
	}
}
