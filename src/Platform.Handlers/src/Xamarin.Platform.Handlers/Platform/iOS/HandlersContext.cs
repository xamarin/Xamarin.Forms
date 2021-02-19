using System;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform
{ 
	public class HandlersContext : IHandlersContext
	{
		IServiceProvider _provider;
		public HandlersContext(IServiceProvider provider)
		{
			_provider = provider;
		}
		public IServiceProvider Provider => _provider;

		public IMauiServiceProvider Handlers => Provider.GetService<IMauiServiceProvider>() ?? throw new NullReferenceException(nameof(IMauiServiceProvider));
	}
}
