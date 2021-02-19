using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Platform.Handlers;

namespace Xamarin.Platform.Hosting
{
	public static class AppHostBuilderExtensions
	{
		//BuildApp will call Build(), we should use one or the other.
		//public static TApplication Build<TApplication>(this IHostBuilder builder) where TApplication : class, IApp
		//{
		//	if (!(builder is IAppHostBuilder appBuilder))
		//		throw new InvalidOperationException($"You can only Init a Maui app with a {nameof(IAppHostBuilder)} builder");

		//	//we create the app here because users might need to register services
		//	if (!(Activator.CreateInstance(typeof(TApplication)) is TApplication app))
		//		throw new Exception($"Could not create a new Application class of type {typeof(TApplication)}");
			
		//	appBuilder.BuildApp(app);
			
		//	return app;
		//}

		public static IAppHostBuilder RegisterHandlers(this IAppHostBuilder builder, Dictionary<Type, Type> handlers)
		{
			foreach (var handler in handlers)
			{
				builder?.ConfigureHandlers((context, handlersCollection) => handlersCollection.AddTransient(handler.Key, handler.Value));
			}

			return builder;
		}

		public static IAppHostBuilder RegisterHandler<TType, TTypeRender>(this IAppHostBuilder builder)
			where TType : IFrameworkElement
			where TTypeRender : IViewHandler
		{
			builder.ConfigureHandlers((context, handlersCollection) => handlersCollection.AddTransient(typeof(TType), typeof(TTypeRender)));

			return builder;
		}

		public static IAppHostBuilder UseMauiHandlers(this IAppHostBuilder builder)
		{
			builder.RegisterHandlers(new Dictionary<Type, Type>
			{
				{  typeof(IButton), typeof(ButtonHandler) },
				{  typeof(ILayout), typeof(LayoutHandler) },
				{  typeof(ILabel), typeof(LabelHandler) },
				{  typeof(ISlider), typeof(SliderHandler) }
			});
			return builder;
		}
	}
}
