using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform.Hosting
{
	class MauiServiceCollection : IHandlerServiceCollection
	{
		static string s_error => "This Collection is based on a non ordered Dictionary";
		internal Dictionary<Type, Type> _handler;
		internal Dictionary<Type, object> _implementations;
		internal Dictionary<Type, Func<IServiceProvider, object>> _factories;
		internal List<ServiceDescriptor> _descriptors;

		public MauiServiceCollection()
		{
			_handler = new Dictionary<Type, Type>();
			_implementations = new Dictionary<Type, object>();
			_factories = new Dictionary<Type, Func<IServiceProvider, object>>();
			_descriptors = new List<ServiceDescriptor>();
		}

		public int Count => _descriptors.Count;

		public bool IsReadOnly => false;

		public void Add(ServiceDescriptor item)
		{
			if (!_descriptors.Contains(item))
				_descriptors.Add(item);
			if (item.ImplementationType != null)
				_handler[item.ServiceType] = item.ImplementationType;
			else if (item.ImplementationInstance != null)
				_implementations[item.ServiceType] = item.ImplementationInstance;
			else if (item.ImplementationFactory != null)
				_factories[item.ServiceType] = item.ImplementationFactory;
			else
				throw new InvalidOperationException($"You need to provide an {nameof(item.ImplementationType)} or a {nameof(item.ImplementationInstance)} ");
		}

		public void Clear()
		{
			_descriptors.Clear();
			_handler.Clear();
			_implementations.Clear();
			_factories.Clear();
		}

		public bool Contains(ServiceDescriptor item) => _descriptors.Contains(item);

		public IEnumerator<ServiceDescriptor> GetEnumerator() => _descriptors.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _descriptors.GetEnumerator();

		public bool Remove(ServiceDescriptor item)
		{
			if (_descriptors.Contains(item))
			{
				_descriptors.Remove(item);
			}
			if (_handler.ContainsKey(item.ServiceType))
			{
				return _handler.Remove(item.ServiceType);
			}
			if (_implementations.ContainsKey(item.ServiceType))
			{
				return _implementations.Remove(item.ServiceType);
			}
			if (_factories.ContainsKey(item.ServiceType))
			{
				return _factories.Remove(item.ServiceType);
			}

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
	}
}
