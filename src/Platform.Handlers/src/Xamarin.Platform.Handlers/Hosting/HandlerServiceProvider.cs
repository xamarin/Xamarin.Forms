using System;
using System.Collections.Generic;

namespace Xamarin.Platform.Hosting
{

	class HandlerServiceProvider : IHandlerServiceProvider
	{
		readonly Dictionary<Type, Type> _handler;
		public HandlerServiceProvider(Dictionary<Type, Type> handler)
		{
			_handler = handler;
		}

		public object GetService(Type serviceType)
		{
			List<Type> types = new List<Type> { serviceType };
			foreach (var interfac in serviceType.GetInterfaces())
			{
				if (typeof(IView).IsAssignableFrom(interfac))
					types.Add(interfac);
			}

			Type baseType = serviceType.BaseType;

			while (baseType != null)
			{
				types.Add(baseType);
				baseType = baseType.BaseType;
			}

			foreach (var type in types)
			{
				if (_handler.ContainsKey(type))
				{
					var typeImplementation = _handler[type];
					return Activator.CreateInstance(typeImplementation);
				}
			}
			return default!;
		}
	}
}
