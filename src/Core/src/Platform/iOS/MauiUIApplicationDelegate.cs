using System;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UIKit;

namespace Microsoft.Maui
{
	public class MauiUIApplicationDelegate<TApplication> : UIApplicationDelegate, IUIApplicationDelegate where TApplication : Application
	{
		bool _isSuspended;
		Application? _app;
		IWindow? _window;

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			if (!(Activator.CreateInstance(typeof(TApplication)) is TApplication app))
				throw new InvalidOperationException($"We weren't able to create the App {typeof(TApplication)}");

			var host = app.CreateBuilder().ConfigureServices(ConfigureNativeServices).Build(app);

			_app = Application.Current;

			if (_app == null || _app.Services == null)
				throw new InvalidOperationException("App was not intialized");

			_app.OnCreated();

			var mauiContext = new MauiContext(_app.Services);
			_window = app.CreateWindow(new ActivationState(mauiContext));

			_window.MauiContext = mauiContext;

			_window.OnCreated();

			//Hack for now we set this on the App Static but this should be on IFrameworkElement
			_app.SetHandlerContext(_window.MauiContext);

			var content = (_window.Content as IView) ?? _window.Content?.View;

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
				_window?.OnResumed();
			}
		}

		public override void OnResignActivation(UIApplication application)
		{
			_isSuspended = true;
			_app?.OnPaused();
			_window?.OnPaused();
		}

		public override void WillTerminate(UIApplication application)
		{
			_app?.OnStopped();
			_window?.OnStopped();
		}

		void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddTransient<IWindowService, WindowService>();
		}
	}
}