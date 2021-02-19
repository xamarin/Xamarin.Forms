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
			_mauiServiceProvider = Provider.GetService<IMauiServiceProvider>() ?? throw new NullReferenceException(nameof(IMauiServiceProvider));
		}
		public Context Context => _context;

		public IServiceProvider Provider => _provider;

		public IMauiServiceProvider Handlers => _mauiServiceProvider;
	}
}
