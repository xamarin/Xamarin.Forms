using System;
using System.Collections.Generic;
using Xamarin.Forms.Internals;
#if NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Xamarin.Forms
{
	public class FormsBuilder : IFormsBuilder
	{
		readonly List<Action> _post = new List<Action>();
		readonly List<Action> _pre = new List<Action>();
		Action _init;

		public FormsBuilder(Action init)
		{
			_init = init;
		}

		public IFormsBuilder Init()
		{
			foreach (Action initAction in _pre)
			{
				initAction();
			}

			_init();

			foreach (Action initAction in _post)
			{
				initAction();
			}

			_init = null;
			_pre.Clear();
			_post.Clear();

			return this;
		}

		public IFormsBuilder PostInit(Action action)
		{
			_post.Add(action);
			return this;
		}

		public IFormsBuilder PreInit(Action action)
		{
			_pre.Add(action);
			return this;
		}

#if NETSTANDARD2_0
		Action<HostBuilderContext, IServiceCollection> _nativeConfigureServices;
		Action<IConfigurationBuilder> _nativeConfigureHostConfiguration;
		Action<HostBuilderContext, IConfigurationBuilder> _nativeConfigureAppConfiguration;
#endif
		IStartup _startup;

		public Application Build(Type app)
		{
			BuildHost(app);
			Application formsApp = null;
#if NETSTANDARD2_0
			formsApp = Application.ServiceProvider?.GetService(app) as Application;
#endif
			return formsApp ?? Activator.CreateInstance(app) as Application;
		}

		public TApp Build<TApp>() where TApp : Application
		{
			BuildHost(typeof(TApp));
			TApp formsApp = null;
#if NETSTANDARD2_0
			formsApp = Application.ServiceProvider?.GetService<TApp>();
#endif
			return formsApp ?? Activator.CreateInstance<TApp>();
		}

		public TApp Build<TApp>(Func<TApp> createApp) where TApp : Application
		{
			BuildHost(typeof(TApp));
			return createApp();
		}

#if NETSTANDARD2_0

		public IFormsBuilder NativeConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
		{
			_nativeConfigureServices = configureDelegate;
			return this;
		}

		public IFormsBuilder NativeConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
		{
			_nativeConfigureHostConfiguration = configureDelegate;
			return this;
		}

		public IFormsBuilder NativeConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
		{
			_nativeConfigureAppConfiguration = configureDelegate;
			return this;
		}

#endif
		public IFormsBuilder UseStartup(Type startupType)
		{
			_startup = Activator.CreateInstance(startupType) as IStartup;
			return this;
		}

		public IFormsBuilder UseStartup<TStartup>() where TStartup : IStartup, new()
		{
			_startup = new TStartup();
			return this;
		}

		public IFormsBuilder UseStartup<TStartup>(Func<TStartup> createStartup) where TStartup : IStartup
		{
			_startup = createStartup();
			return this;
		}

		void BuildHost(Type app)
		{
			Init();

#if NETSTANDARD2_0
			EmbeddedResourceLoader.SetExecutingAssembly(app.Assembly);
			IHost host = new HostBuilder()
				.ConfigureAppConfiguration((context, builder) =>
				{
					_nativeConfigureAppConfiguration?.Invoke(context, builder);
				})
				.ConfigureHostConfiguration(context =>
				{
					context.AddJsonStream(EmbeddedResourceLoader.GetEmbeddedResourceStream("appsettings.json"));
					_nativeConfigureHostConfiguration?.Invoke(context);
					_startup?.ConfigureHostConfiguration(context);
				})
				.ConfigureServices((h, s) =>
				{
					_nativeConfigureServices?.Invoke(h, s);
					_startup?.ConfigureServices(h, s);
				})
				.Build();

			Application.ServiceProvider = host.Services;
#endif
		}
	}
}