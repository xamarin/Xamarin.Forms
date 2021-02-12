using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Platform.Hosting.Internal;

namespace Xamarin.Platform.Hosting
{
	public class AppBuilder : IAppHostBuilder
	{
		List<Action<HostBuilderContext, IHandlerServiceCollection>> _configureHandlersActions = new List<Action<HostBuilderContext, IHandlerServiceCollection>>();
		List<Action<IConfigurationBuilder>> _configureHostConfigActions = new List<Action<IConfigurationBuilder>>();
		List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions = new List<Action<HostBuilderContext, IConfigurationBuilder>>();
		List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new List<Action<HostBuilderContext, IServiceCollection>>();
		List<IConfigureContainerAdapter> _configureContainerActions = new List<IConfigureContainerAdapter>();
		IServiceFactoryAdapter _serviceProviderFactory = new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());
		bool _hostBuilt;
		//IConfiguration? _hostConfiguration;
		//IConfiguration? _appConfiguration;
		HostBuilderContext? _hostBuilderContext;
		IHostEnvironment? _hostEnvironment;
		IServiceProvider? _appServices;
		IHandlerServiceProvider? _handlersServiceProvider;
		
		public IDictionary<object, object> Properties => new Dictionary<object, object>();

		public IHostBuilder RegisterHandlers(Dictionary<Type, Type> handlers)
		{
			foreach (var handler in handlers)
			{
				ConfigureHandlers((context, handlersCollection) => handlersCollection.AddTransient(handler.Key, handler.Value));
			}

			return this;
		}

		public IHostBuilder RegisterHandler<TType, TTypeRender>()
			where TType : IFrameworkElement
			where TTypeRender : IViewHandler
		{
			ConfigureHandlers((context, handlersCollection) => handlersCollection.AddTransient(typeof(TType), typeof(TTypeRender)));

			return this;
		}

		public TApplication Init<TApplication>() where TApplication : class, IApp
		{
			//User services so its almost empty and fast
			ServiceCollection servicesCollection = new ServiceCollection();

			var host = Build();

			if (_handlersServiceProvider != null)
				servicesCollection.AddSingleton<IHandlerServiceProvider>(_handlersServiceProvider);

			var app = (TApplication)Activator.CreateInstance(typeof(TApplication));

			if (_hostBuilderContext != null)
				AppLoader.ConfigureAppServices<TApplication>(_hostBuilderContext, servicesCollection, app);

			var services = servicesCollection.BuildServiceProvider();

			(app as App)?.SetServiceProvider(services);

			return app;
		}

		public IHost Build()
		{
			if (_hostBuilt)
			{
				throw new InvalidOperationException("Build can only be called once.");
			}
			_hostBuilt = true;

			// the order is important here
			BuildHostConfiguration();
			CreateHostingEnvironment();
			CreateHostBuilderContext();
			BuildAppConfiguration();
			CreateHandlersServiceProvider();
			CreateServiceProvider();
			if (_appServices == null)
			{
				throw new InvalidOperationException($"The (_appServices) cannot be null");
			}

			return new AppHost(_appServices, null);
		}

		public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
		{
			_configureAppConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
			return this;
		}

		public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
		{
			_configureContainerActions.Add(new ConfigureContainerAdapter<TContainerBuilder>(configureDelegate
			 ?? throw new ArgumentNullException(nameof(configureDelegate))));
			return this;
		}

		public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
		{
			_configureHostConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
			return this;
		}

		public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
		{
			_configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
			return this;
		}

		public IHostBuilder ConfigureHandlers(Action<HostBuilderContext, IHandlerServiceCollection> configureDelegate)
		{
			_configureHandlersActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
			return this;
		}

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
		{
			_serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory ?? throw new ArgumentNullException(nameof(factory)));
			return this;
		}

		public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
		{
			if (_hostBuilderContext != null)
				_serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(() => _hostBuilderContext, factory ?? throw new ArgumentNullException(nameof(factory)));
			return this;
		}
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

		void BuildHostConfiguration()
		{
			//IConfigurationBuilder configBuilder = new ConfigurationBuilder()
			//	.AddInMemoryCollection(); // Make sure there's some default storage since there are no default providers

			//foreach (Action<IConfigurationBuilder> buildAction in _configureHostConfigActions)
			//{
			//	buildAction(configBuilder);
			//}
			//_hostConfiguration = configurationBuilder.Build();
		}

		void CreateHostingEnvironment()
		{
		
			_hostEnvironment = new AppHostEnvironment()
			{
				//ApplicationName = _hostConfiguration[HostDefaults.ApplicationKey],
				//EnvironmentName = _hostConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production,
				//ContentRootPath = ResolveContentRootPath(_hostConfiguration[HostDefaults.ContentRootKey], AppContext.BaseDirectory),
			};

			if (string.IsNullOrEmpty(_hostEnvironment.ApplicationName))
			{
				// Note GetEntryAssembly returns null for the net4x console test runner.
				_hostEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;
			}

			//_hostEnvironment.ContentRootFileProvider = new PhysicalFileProvider(_hostEnvironment.ContentRootPath);
		}

		//string ResolveContentRootPath(string contentRootPath, string basePath)
		//{
		//	if (string.IsNullOrEmpty(contentRootPath))
		//	{
		//		return basePath;
		//	}
		//	if (Path.IsPathRooted(contentRootPath))
		//	{
		//		return contentRootPath;
		//	}
		//	return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
		//}

		void CreateHostBuilderContext()
		{
			_hostBuilderContext = new HostBuilderContext(Properties)
			{
				HostingEnvironment = _hostEnvironment,
				//Configuration = _hostConfiguration
			};
		}

		void BuildAppConfiguration()
		{
			if (_hostBuilderContext == null)
				return;

			//IConfigurationBuilder configBuilder = new ConfigurationBuilder()
			//	.SetBasePath(_hostEnvironment?.ContentRootPath)
			//	.AddConfiguration(_hostConfiguration, shouldDisposeConfiguration: true);

			//foreach (Action<HostBuilderContext, IConfigurationBuilder> buildAction in _configureAppConfigActions)
			//{
			//	buildAction(_hostBuilderContext, configBuilder);
			//}
			//_appConfiguration = configBuilder.Build();
			//_hostBuilderContext.Configuration = _appConfiguration;
		}

		void CreateServiceProvider()
		{
			var services = new ServiceCollection();

			//if (_hostEnvironment != null)
			//	services.AddSingleton<IHostEnvironment>(_hostEnvironment);
			//if (_hostBuilderContext != null)
			//	services.AddSingleton(_hostBuilderContext);
			// register configuration as factory to make it dispose with the service provider
			//if (_appConfiguration != null)
			//	services.AddSingleton(_ => _appConfiguration);
			//services.AddSingleton<IHostApplicationLifetime, AppLifetime>();
			//services.AddSingleton<IHostLifetime, AppHostLifetime>();
			//services.AddSingleton<IHost, AppHost>();
			//services.AddOptions();
			//services.AddLogging();

			foreach (Action<HostBuilderContext, IServiceCollection> configureServicesAction in _configureServicesActions)
			{
				if (_hostBuilderContext != null)
					configureServicesAction(_hostBuilderContext, services);
			}

			////TODO: Do we need a factory here
			if (_handlersServiceProvider != null)
				services.AddSingleton<IHandlerServiceProvider>(_handlersServiceProvider);

			object containerBuilder = _serviceProviderFactory.CreateBuilder(services);

			foreach (IConfigureContainerAdapter containerAction in _configureContainerActions)
			{
				if (_hostBuilderContext != null)
					containerAction.ConfigureContainer(_hostBuilderContext, containerBuilder);
			}

			_appServices = _serviceProviderFactory.CreateServiceProvider(containerBuilder);

			if (_appServices == null)
			{
				throw new InvalidOperationException($"The IServiceProviderFactory returned a null IServiceProvider.");
			}
		}

		void CreateHandlersServiceProvider()
		{
			var handlersServices = new HandlerServiceCollection();
			foreach (var configureHandlersAction in _configureHandlersActions)
			{
				if (_hostBuilderContext != null)
					configureHandlersAction(_hostBuilderContext, handlersServices);
			}
			_handlersServiceProvider = handlersServices.BuildHandlerServiceProvider();
		}
	}
}
