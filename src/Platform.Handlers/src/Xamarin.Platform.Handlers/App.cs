using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform
{
	public abstract class App : IApp
	{
		IServiceProvider? _serviceProvider;
		IMauiServiceProvider? _handlerServiceProvider;
		IHandlersContext? _context;

		protected App()
		{
			Current = this;
		}

		public static App? Current { get; private set; }

		public IServiceProvider? Services => _serviceProvider;

		//public IServiceProvider? Handlers => _handlerServiceProvider;

		public IHandlersContext? Context => _context;

		public virtual IAppHostBuilder CreateBuilder() => CreateDefaultBuilder();

		public abstract IWindow GetWindowFor(Dictionary<string, string> state);

		internal void SetServiceProvider(IServiceProvider provider)
		{
			_serviceProvider = provider;
			SetHandlerServiceProvider(provider.GetService<IMauiServiceProvider>());
			SetHandlerContext(provider.GetService<IHandlersContext>());
		}

		internal void SetHandlerServiceProvider(IMauiServiceProvider? provider)
		{
			_handlerServiceProvider = provider;
		}

		internal void SetHandlerContext(IHandlersContext? context)
		{
			_context = context;
		}

		public static IAppHostBuilder CreateDefaultBuilder()
		{
			var builder = new AppBuilder();

			builder.UseMauiHandlers();

			return builder;
		}
	}
}
