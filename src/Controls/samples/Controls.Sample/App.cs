using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Maui.Controls.Sample.Pages;
using Maui.Controls.Sample.Services;
using Maui.Controls.Sample.ViewModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using Microsoft.Maui;
//#if __ANDROID__
//using Maui.Controls.Compatibility;
//#endif

using Application = Microsoft.Maui.Application;
using System.Diagnostics;

namespace Maui.Controls.Sample
{
	public class App : Application
	{
		public override IAppHostBuilder CreateBuilder()
		{
			var builder = base.CreateBuilder()
				.ConfigureAppConfiguration((hostingContext, config) =>
				   {
					   config.AddInMemoryCollection(new Dictionary<string, string>
					   {
							{"MyKey", "Dictionary MyKey Value"},
							{":Title", "Dictionary_Title"},
							{"Position:Name", "Dictionary_Name" },
							{"Logging:LogLevel:Default", "Warning"}
						});
				   })
				   .ConfigureServices(ConfigureServices);
#if __ANDROID__
				   //.RegisterCompatibilityRenderer<Microsoft.Maui.Controls.Button, Microsoft.Maui.Controls.Button, Microsoft.Maui.Controls.Platform.Android.FastRenderers.ButtonRenderer>()
#endif
			return builder;
		}

		public override void OnCreated()
		{
			Debug.WriteLine("Application Created.");

			MainWindow window = new MainWindow();
			window.Show();
		}

		public override void OnPaused()
		{
			Debug.WriteLine("Application Paused.");
		}

		public override void OnResumed()
		{
			Debug.WriteLine("Application Resumed.");
		}

		public override void OnStopped()
		{
			Debug.WriteLine("Application Stopped.");
		}

		void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
		{
			//services.AddLogging();
			services.AddSingleton<ITextService, TextService>();
			services.AddTransient<MainPageViewModel>();
			services.AddTransient<MainPage>();
			services.AddTransient<IWindow, MainWindow>();
		}
	}

	// To use DI ServiceCollection and not the MAUI one
	public class DIExtensionsServiceProviderFactory : IServiceProviderFactory<ServiceCollection>
	{
		public ServiceCollection CreateBuilder(IServiceCollection services)
			=> new ServiceCollection { services };

		public IServiceProvider CreateServiceProvider(ServiceCollection containerBuilder)
			=> containerBuilder.BuildServiceProvider();
	}
}