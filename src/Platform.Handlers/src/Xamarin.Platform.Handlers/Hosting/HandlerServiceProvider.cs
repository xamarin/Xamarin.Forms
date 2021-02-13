using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform.Hosting
{
	class HandlerServiceProvider : IHandlerServiceProvider
	{
		readonly Dictionary<Type, Type>? _implementationsType;
		readonly Dictionary<Type, object>? _implementationsInstances;
		internal Dictionary<Type, Func<IServiceProvider, object>>? _implementationsFactories;
		IServiceCollection _serviceCollection;

		public HandlerServiceProvider(IServiceCollection collection)
		{
			_serviceCollection = collection;
			if (collection is MauiServiceCollection handlerServiCollection)
			{
				_implementationsType = handlerServiCollection._handler;
				_implementationsInstances = handlerServiCollection._implementations;
				_implementationsFactories = handlerServiCollection._factories;
			}
		}

		public object? GetService(Type serviceType)
		{
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));

			List<Type> types = new List<Type> { serviceType };
			foreach (var interfac in serviceType.GetInterfaces())
			{
				if (typeof(IView).IsAssignableFrom(interfac))
					types.Add(interfac);
			}

			Type? baseType = serviceType.BaseType;

			while (baseType != null)
			{
				types.Add(baseType);
				baseType = baseType.BaseType;
			}

			foreach (var type in types)
			{
				if (_implementationsType != null && _implementationsType.ContainsKey(type))
				{
					var typeImplementation = _implementationsType[type];
					//ctor only for IApp for performance reasons
					return Activator.CreateInstance(typeImplementation);
				}
				if (_implementationsInstances != null && _implementationsInstances.ContainsKey(type))
				{
					var typeInstance = _implementationsInstances[type];
					return typeInstance;
				}
				if (_implementationsFactories != null && _implementationsFactories.ContainsKey(type))
				{
					var typeInstance = _implementationsFactories[type].Invoke(this);
					return typeInstance;
				}
			}

			return default!;
		}
	}
}
