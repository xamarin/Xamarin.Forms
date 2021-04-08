using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Hosting;
using Tizen.Applications;

namespace Microsoft.Maui
{

	public class MauiApplication<TApplication> : CoreUIApplication where TApplication : MauiApp
	{
		protected override void OnCreate()
		{
			base.OnCreate();

			if(!(Activator.CreateInstance(typeof(TApplication)) is TApplication app))
				throw new InvalidOperationException($"We weren't able to create the App {typeof(TApplication)}");

			var host = app.CreateBuilder().ConfigureServices(ConfigureNativeServices).Build(app);

			if (MauiApp.Current == null || MauiApp.Current.Services == null)
				throw new InvalidOperationException("App was not intialized");

			var context = new CoreUIAppContext(this);
			var mauiContext = new MauiContext(MauiApp.Current.Services, context);
			var window = app.CreateWindow(new ActivationState(mauiContext));

			window.MauiContext = mauiContext;

			//Hack for now we set this on the App Static but this should be on IFrameworkElement
			App.Current.SetHandlerContext(window.MauiContext);

			var content = (window.Page as IView) ?? window.Page.View;

			context.SetContent(content.ToNative(window.MauiContext));
		}

		void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{
		}
	}
}
