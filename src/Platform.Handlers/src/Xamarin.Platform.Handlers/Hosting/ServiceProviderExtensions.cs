using System;

namespace Xamarin.Platform.Hosting
{
	public static class ServiceProviderExtensions
	{
		internal static IHandlerServiceProvider BuildHandlerServiceProvider(this HandlerServiceCollection serviceCollection)
			=> new HandlerServiceProvider(serviceCollection._handler);

		public static IViewHandler? GetHandler(this IServiceProvider services, Type type)
			=> services.GetService(type) as IViewHandler;

		public static IViewHandler? GetHandler<T>(this IServiceProvider services) where T : IView
			=> GetHandler(services, typeof(T));
	}
}