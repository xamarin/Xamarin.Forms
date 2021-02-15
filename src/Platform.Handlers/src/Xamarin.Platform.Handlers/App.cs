using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform
{
	public abstract class App :
#if __ANDROID__
		global::Android.App.Application,
#elif __IOS__
		global::UIKit.UIApplicationDelegate,
#endif
		IApp
	{
		IServiceProvider? _serviceProvider;
		IHandlerServiceProvider? _handlerServiceProvider;
		
		protected App()
		{
			Current = this;
		}

		public static App? Current { get; private set; }

		public IServiceProvider? Services => _serviceProvider;

		public IServiceProvider? Handlers => _handlerServiceProvider;

		public virtual IEnumerable<IWindow>? Windows
		{
			get
			{
				var windows = Services?.GetService<IWindow>();
				if(windows != null)
					return new IWindow[] { windows };
				return null;
			}
		}

		internal void SetServiceProvider(IServiceProvider provider)
		{
			_serviceProvider = provider;
			SetHandlerServiceProvider(provider.GetService<IHandlerServiceProvider>());
		}

		internal void SetHandlerServiceProvider(IHandlerServiceProvider? provider)
		{
			_handlerServiceProvider = provider;
		}

		public static IAppHostBuilder CreateDefaultBuilder()
		{
			var builder = new AppBuilder();

			builder.UseMauiHandlers();

			return builder;
		}
	}
}
