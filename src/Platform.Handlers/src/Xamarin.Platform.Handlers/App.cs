using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform
{
	//MAUIApp

	//public IHandlersContext? Context { get; }

	//IWindow GetWindowFor(Dictionary<string, string> state);
	public abstract class App : IApp
	{
		IServiceProvider? _serviceProvider;
		IHandlersContext? _context;

		protected App()
		{
			Current = this;
		}

		public static App? Current { get; private set; }

		public IServiceProvider? Services => _serviceProvider;

		public IHandlersContext? Context => _context;

		//move to abstract
		public virtual IAppHostBuilder CreateBuilder() => CreateDefaultBuilder();

		public abstract IWindow GetWindowFor(Dictionary<string, string> state);

		internal void SetServiceProvider(IServiceProvider provider)
		{
			_serviceProvider = provider;
			SetHandlerContext(provider.GetService<IHandlersContext>());
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
