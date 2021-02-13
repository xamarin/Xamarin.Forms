using System;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform.Hosting.Internal
{
	public class DefaultServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
	{
		public IServiceCollection CreateBuilder(IServiceCollection services)
		{
			return services;
		}

		public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
		{
			return containerBuilder.BuildServiceProvider();
		}
	}
}
