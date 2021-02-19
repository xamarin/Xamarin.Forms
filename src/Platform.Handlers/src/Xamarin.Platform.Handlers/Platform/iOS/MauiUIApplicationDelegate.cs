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
	public class MauiUIApplicationDelegate<TApplication> : UIApplicationDelegate, IUIApplicationDelegate where TApplication : App
	{
		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			var app = (TApplication)Activator.CreateInstance(typeof(TApplication));

			var host = app.CreateBuilder().ConfigureServices(ConfigureNativeServices).Build(app);

			if (App.Current == null || App.Current.Services == null)
				throw new InvalidOperationException("App was not intialized");

			var window = app.GetWindowFor(null!);

			window.HandlersContext = new HandlersContext(App.Current.Services);

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
