using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui
{
	public abstract class App : IApp
	{
		IServiceProvider? _serviceProvider;
		IMauiContext? _context;

		protected App()
		{
			if (Current != null)
				throw new InvalidOperationException($"Only one {nameof(App)} instance is allowed");
			Current = this;
		}

		public static App? Current { get; internal set; }

		public IServiceProvider? Services => _serviceProvider;

		public IMauiContext? Context => _context;

		// Move to abstract
		public virtual IAppHostBuilder CreateBuilder() => CreateDefaultBuilder();

		public virtual void Create()
		{
		}

		public virtual void Resume()
		{
		}

		public virtual void Pause()
		{
		}

		public virtual void Destroy()
		{
		}

		internal void SetServiceProvider(IServiceProvider provider)
		{
			_serviceProvider = provider;
			SetHandlerContext(provider.GetService<IMauiContext>());
		}

		internal void SetHandlerContext(IMauiContext? context)
		{
			_context = context;
		}

		public static IAppHostBuilder CreateDefaultBuilder()
		{
			var builder = new AppHostBuilder();

			builder.UseMauiHandlers();

			return builder;
		}
	}
}
