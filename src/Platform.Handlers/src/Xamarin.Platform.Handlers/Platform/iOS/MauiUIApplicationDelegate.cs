using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UIKit;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform
{
	public class MauiUIApplicationDelegate<TApplication> : UIApplicationDelegate, IUIApplicationDelegate where TApplication : MauiApp
	{
		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			TApplication app;
			var instance = Activator.CreateInstance(typeof(TApplication));
			if (instance is TApplication app)
				throw new InvalidOperationException($"We weren't able to create the App {typeof(TApplication)}");

			var host = app.CreateBuilder().ConfigureServices(ConfigureNativeServices).Build(app);

			if (MauiApp.Current == null || MauiApp.Current.Services == null)
				throw new InvalidOperationException("App was not intialized");

			var window = app.GetWindowFor(null!);

			window.HandlersContext = new HandlersContext(MauiApp.Current.Services);

			var content = window.Page.View;

			var uiWindow = new UIWindow
			{
				RootViewController = new UIViewController
				{
					View = content.ToNative(window.HandlersContext)
				}
			};

			uiWindow.MakeKeyAndVisible();

			return true;
		}

		void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{
			
		}
	}
}
