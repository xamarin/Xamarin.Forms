using System;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform.Handlers.Tests
{ 
	class HandlersContextStub : IMauiContext
	{
		IServiceProvider _provider;
		IMauiServiceProvider _handlersServiceProvider;
		public HandlersContextStub(IServiceProvider provider)
		{
			_provider = provider;
			_handlersServiceProvider = Provider.GetService<IMauiServiceProvider>() ?? throw new NullReferenceException(nameof(IMauiServiceProvider));
		}
		
		public IServiceProvider Provider => _provider;

		public IMauiServiceProvider Handlers => _handlersServiceProvider;
	}
}
