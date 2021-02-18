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
		readonly List<Action<HostBuilderContext, IServiceCollection>> _configureHandlersActions = new List<Action<HostBuilderContext, IServiceCollection>>();
		readonly List<Action<IConfigurationBuilder>> _configureHostConfigActions = new List<Action<IConfigurationBuilder>>();
		readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions = new List<Action<HostBuilderContext, IConfigurationBuilder>>();
		readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new List<Action<HostBuilderContext, IServiceCollection>>();
		readonly List<IConfigureContainerAdapter> _configureContainerActions = new List<IConfigureContainerAdapter>();
		readonly Func<IServiceCollection> _serviceColectionFactory = new Func<IServiceCollection>(() => new MauiServiceCollection());
		IServiceFactoryAdapter _serviceProviderFactory = new ServiceFactoryAdapter<IServiceCollection>(new MauiServiceProviderFactory());
		bool _hostBuilt;
		HostBuilderContext? _hostBuilderContext;
		IHostEnvironment? _hostEnvironment;
		IServiceProvider? _serviceProvider;
		IServiceCollection? _services;
		IApp? _app;

		public AppBuilder()
		{

		}
		public IDictionary<object, object> Properties => new Dictionary<object, object>();

		public IApp BuildApp(IApp app)
		{
			_app = app;
			Build();
			return _app;
		}

		public IHost Build()
		{
			_services = _serviceColectionFactory();

			if (_hostBuilt)
				throw new InvalidOperationException("Build can only be called once.");

			_hostBuilt = true;

			// the order is important here
			CreateHostingEnvironment();
			CreateHostBuilderContext();

			if (_services == null)
				throw new InvalidOperationException("The ServiceCollection cannot be null");

			ConfigureHandlers(_services);
			ConfigureAppServices(_services);
			CreateServiceProvider(_services);

			if (_serviceProvider == null)
				throw new InvalidOperationException($"The ServiceProvider cannot be null");

			//we do this here because we can't inject the provider on the App ctor
			//before we register the user ConfigureServices should this live in IApp ?
			(_app as App)?.SetServiceProvider(_serviceProvider);

			return new AppHost(_serviceProvider, null);
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

		public IHostBuilder ConfigureHandlers(Action<HostBuilderContext, IServiceCollection> configureDelegate)
		{
			_configureHandlersActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
			return this;
		}

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning disable CS8603 // Possible null reference return.
		public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
		{
			_serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory ?? throw new ArgumentNullException(nameof(factory)));
			return this;
		}

		public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
		{
			_serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(() => _hostBuilderContext, factory ?? throw new ArgumentNullException(nameof(factory)));

			return this;
		}
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint

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
		}

		void CreateHostBuilderContext()
		{
			_hostBuilderContext = new HostBuilderContext(Properties)
			{
				HostingEnvironment = _hostEnvironment,
			};
		}

		void CreateServiceProvider(IServiceCollection services)
		{
			foreach (Action<HostBuilderContext, IServiceCollection> configureServicesAction in _configureServicesActions)
			{
				if (_hostBuilderContext != null)
					configureServicesAction(_hostBuilderContext, services);
			}

			_serviceProvider = ConfigureContainerAndGetProvider(services);

			if (_serviceProvider == null)
			{
				throw new InvalidOperationException($"The IServiceProviderFactory returned a null IServiceProvider.");
			}
		}

		IServiceProvider ConfigureContainerAndGetProvider(IServiceCollection services)
		{
			object containerBuilder = _serviceProviderFactory.CreateBuilder(services);

			foreach (IConfigureContainerAdapter containerAction in _configureContainerActions)
			{
				if (_hostBuilderContext != null)
					containerAction.ConfigureContainer(_hostBuilderContext, containerBuilder);
			}

			return _serviceProviderFactory.CreateServiceProvider(containerBuilder);
		}

		void ConfigureHandlers(IServiceCollection? services)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			//we need to use our own ServiceCollction because the default ServiceCollection
			//enforces the instance to implement the servicetype
			var _handlersCollection = new MauiServiceCollection();
			foreach (var configureHandlersAction in _configureHandlersActions)
			{
				if (_hostBuilderContext != null)
					configureHandlersAction(_hostBuilderContext, _handlersCollection);
			}
			services.AddSingleton((IMauiServiceProvider)_handlersCollection.BuildServiceProvider());
		}

		void ConfigureAppServices(IServiceCollection services)
		{
			//Call ConfigureServices methoda on the users App class
			if (_app != null)
				AppLoader.ConfigureAppServices(_hostBuilderContext, services, _app);
		}
	}
}
