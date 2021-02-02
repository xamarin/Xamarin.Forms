using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace Xamarin.Platform.Hosting
{
	public interface IAppHostBuilder : IHostBuilder
	{
		IHostBuilder ConfigureHandlers(Action<HostBuilderContext, IHandlerServiceCollection> configureDelegate);
		TApplication Init<TApplication>() where TApplication : class, IApp;
		IHostBuilder RegisterHandlers(Dictionary<Type, Type> handlers);
	}
}
