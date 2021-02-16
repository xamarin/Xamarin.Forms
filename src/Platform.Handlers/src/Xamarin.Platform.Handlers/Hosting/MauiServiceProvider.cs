using System;
using System.Collections.Generic;

namespace Xamarin.Platform.Hosting
{
	class MauiServiceProvider : IMauiServiceProvider
	{
		IDictionary<Type, Func<IServiceProvider, object>>? _implementationsFactories;

		public MauiServiceProvider(IMauiServiceCollection collection)
		{
			_implementationsFactories = collection;
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
