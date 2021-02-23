using System;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform.Handlers.Benchmarks
{
	class HandlersContextStub : IMauiContext
	{
		readonly IServiceProvider _provider;
		readonly IMauiServiceProvider _handlersServiceProvider;

		public HandlersContextStub(IServiceProvider provider)
		{
			_provider = provider;
			_handlersServiceProvider = Provider.GetRequiredService<IMauiServiceProvider>();
		}

		public IServiceProvider Provider => _provider;

		public IMauiServiceProvider Handlers => _handlersServiceProvider;
	}
}
