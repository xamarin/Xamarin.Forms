using System;
using Android.Content;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform
{
	public class HandlersContext : IHandlersContext
	{
		Context _context;
		IServiceProvider _provider;
		IMauiServiceProvider _mauiServiceProvider;
		public HandlersContext(IServiceProvider provider, Context context)
		{
			_context = context;
			_provider = provider;
			_mauiServiceProvider = Provider.GetRequiredService<IMauiServiceProvider>() ??
				throw new InvalidOperationException($"The Handlers provider of type {nameof(IMauiServiceProvider)} was not found");
		}
		public Context Context => _context;

		public IServiceProvider Provider => _provider;

		public IMauiServiceProvider Handlers => _mauiServiceProvider;
	}
}
