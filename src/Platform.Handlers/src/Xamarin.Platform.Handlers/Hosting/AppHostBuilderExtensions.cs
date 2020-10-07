using System;
using System.Collections.Generic;
using Xamarin.Platform.Handlers;

namespace Xamarin.Platform.Hosting
{
	public static class AppHostBuilderExtensions
	{
		public static IAppHostBuilder UseXamarinHandlers(this IAppHostBuilder builder)
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
