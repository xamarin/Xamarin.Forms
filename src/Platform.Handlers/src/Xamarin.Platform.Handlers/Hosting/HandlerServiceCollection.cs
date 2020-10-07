using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform.Hosting
{
	class HandlerServiceCollection : IHandlerServiceCollection
	{
		static string s_error => "This Collection is based on a non ordered Dictionary";
		internal Dictionary<Type, Type> _handler;

		public HandlerServiceCollection()
		{
			_handler = new Dictionary<Type, Type>();
		}

		public int Count => _handler.Count;

		public bool IsReadOnly => false;

		public void Add(ServiceDescriptor item)
		{
			if (item.ImplementationType == null)
				throw new InvalidOperationException($"You need to provide an {item.ImplementationType}");

			_handler[item.ServiceType] = item.ImplementationType;
		}

		public void Clear() => _handler.Clear();

		public bool Contains(ServiceDescriptor item) => _handler.ContainsKey(item.ServiceType);

		public IEnumerator<ServiceDescriptor> GetEnumerator() => _handler.Select(c => new ServiceDescriptor(c.Key, c.Value)).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _handler.GetEnumerator();

		public bool Remove(ServiceDescriptor item)
		{
			if (_handler.ContainsKey(item.ServiceType))
			{
				var element = _handler[item.ServiceType];
				return _handler.Remove(element);
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
