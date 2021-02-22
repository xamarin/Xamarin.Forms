using System;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform
{ 
	public class MauiContext : IMauiContext
	{
		IServiceProvider _provider;
		public MauiContext(IServiceProvider provider)
		{
			_provider = provider;
		}
		public IServiceProvider Provider => _provider;

		public IMauiServiceProvider Handlers => Provider.GetService<IMauiServiceProvider>() ?? throw new NullReferenceException(nameof(IMauiServiceProvider));
	}
}
