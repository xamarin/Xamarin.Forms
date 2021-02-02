using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Platform.Core;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform
{
	public abstract class App : 
#if __ANDROID__
		global::Android.App.Application,
#elif __IOS__
		global::UIKit.UIApplicationDelegate,
#endif
		IApp
	{
		IServiceProvider? _serviceProvider;
		IHandlerServiceProvider? _handlerServiceProvider;
		protected App()
		{
			Current = this;
		}

		public static App? Current { get; private set; }

		public IServiceProvider? Services => _serviceProvider;

		public IHandlerServiceProvider? Handlers => _handlerServiceProvider;

		public virtual IEnumerable<IWindow>? Windows => Services?.GetServices<IWindow>();

		public void SetServiceProvider(IServiceProvider provider)
		{
			_serviceProvider = provider;
			SetHandlerServiceProvider(provider.GetService<IHandlerServiceProvider>());
		}

		internal void SetHandlerServiceProvider(IHandlerServiceProvider? provider)
		{
			_handlerServiceProvider = provider;
		}

		public static AppBuilder CreateDefaultBuilder()
		{
			var builder = new AppBuilder();

			//builder.UseContentRoot(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			//builder.ConfigureHostConfiguration(config =>
			//{
			//	config.AddEnvironmentVariables(prefix: "DOTNET_");
			//});

			//builder.ConfigureAppConfiguration((hostingContext, config) =>
			//{
			//	IHostEnvironment env = hostingContext.HostingEnvironment;

			//	//bool reloadOnChange = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);

			//	//config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
			//	//	  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);

			//	//if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
			//	//{
			//	//	var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
			//	//	if (appAssembly != null)
			//	//	{
			//	//		config.AddUserSecrets(appAssembly, optional: true);
			//	//	}
			//	//}

			//	//config.AddEnvironmentVariables();
			//})
			//builder.ConfigureLogging((hostingContext, logging) =>
			//{
				//logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
				//logging.AddConsole();
				//logging.AddDebug();
				//logging.AddEventSourceLogger();


				//logging.Configure(options =>
				//{
				//	options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
				//										| ActivityTrackingOptions.TraceId
				//										| ActivityTrackingOptions.ParentId;
				//});

			//});
			//.UseDefaultServiceProvider((context, options) =>
			//{
			//	bool isDevelopment = context.HostingEnvironment.IsDevelopment();
			//	options.ValidateScopes = isDevelopment;
			//	options.ValidateOnBuild = isDevelopment;
			//});

			builder.UseXamarinHandlers();

			return builder;
		}
	}
}
