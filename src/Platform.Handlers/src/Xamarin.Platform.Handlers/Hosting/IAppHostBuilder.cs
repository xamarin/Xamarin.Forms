using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Xamarin.Platform.Hosting
{
	public interface IAppHostBuilder : IHostBuilder
	{
		IHostBuilder ConfigureHandlers(Action<HostBuilderContext, IServiceCollection> configureDelegate);
		IApp BuildApp(IApp app);
	}
}
