using System;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UIKit;

namespace Microsoft.Maui
{
	public class MauiUIApplicationDelegate<TApplication> : UIApplicationDelegate, IUIApplicationDelegate where TApplication : MauiApp
	{
		bool _isSuspended;
		MauiApp? _app;
		IWindow? _window;

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			if (!(Activator.CreateInstance(typeof(TApplication)) is TApplication app))
				throw new InvalidOperationException($"We weren't able to create the App {typeof(TApplication)}");

			var host = app.CreateBuilder().ConfigureServices(ConfigureNativeServices).Build(app);

			_app = App.Current as MauiApp;

			if (_app == null || _app.Services == null)
				throw new InvalidOperationException("App was not intialized");

			_app.OnCreated();

			_window = app.GetWindowFor(null!);

			_window.OnCreated();

			_window.MauiContext = new MauiContext(_app.Services);

			_app.MainWindow = _window;

			//Hack for now we set this on the App Static but this should be on IFrameworkElement
			App.Current?.SetHandlerContext(_window.MauiContext);

			var content = _window.Content?.View;

			var uiWindow = new UIWindow
			{
				RootViewController = new UIViewController
				{
					View = content?.ToNative(_window.MauiContext)
				}
			};

			uiWindow.MakeKeyAndVisible();

			return true;
		}

		public override void OnActivated(UIApplication application)
		{
			if (_isSuspended)
			{
				_isSuspended = false;
				_app?.OnResumed();
			}
		}

		public override void OnResignActivation(UIApplication application)
		{
			_isSuspended = true;
			_app?.OnPaused();
		}

		public override void WillTerminate(UIApplication application)
		{
			_app?.OnStopped();
		}

		void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddTransient<IWindowService, WindowService>();
		}
	}
}