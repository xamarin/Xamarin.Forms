using System;
using Microsoft.Extensions.DependencyInjection;

namespace Xamarin.Platform.Hosting
{
	public static class ServiceProviderExtensions
	{
		internal static IServiceProvider BuildServiceProvider(this IServiceCollection serviceCollection)
		{
			return new HandlerServiceProvider(serviceCollection);
		}

		public static IViewHandler? GetHandler(this IServiceProvider services, Type type)
			=> services.GetService(type) as IViewHandler;

		public static IViewHandler? GetHandler<T>(this IServiceProvider services) where T : IView
			=> GetHandler(services, typeof(T));

	}
}