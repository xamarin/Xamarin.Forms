using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Xamarin.Platform.Hosting
{
	public interface IAppHostBuilder : IHostBuilder
	{
		IHostBuilder ConfigureHandlers(Action<HostBuilderContext, IServiceCollection> configureDelegate);
		TApplication Init<TApplication>() where TApplication : class, IApp;
	}
}
