using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Maui.Hosting
{
	public interface IAppHostBuilder : IHostBuilder
	{
		IHostBuilder ConfigureHandlers(Action<HostBuilderContext, IServiceCollection> configureDelegate);
		new IAppHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate);
		new IAppHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate);
		new IAppHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate);
		new IAppHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		new IAppHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory);
		new IAppHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		IHost Build(IApp app);
	}
}
