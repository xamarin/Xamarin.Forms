using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Maui.Controls.Sample.Pages;
using Maui.Controls.Sample.Services;
using Maui.Controls.Sample.ViewModel;
using Xamarin.Platform;
using Xamarin.Platform.Hosting;

namespace Maui.Controls.Sample
{
	public class MyApp : App
	{
		public override IAppHostBuilder CreateBuilder()
		{
			var builder = base.CreateBuilder()
				   //.ConfigureLogging(logging =>
				   //{
				   //	logging.ClearProviders();
				   //	logging.AddConsole();
				   //})
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
			return builder;
		}

		//IAppState state
		public override IWindow GetWindowFor(Dictionary<string, string> state)
		{
			return Services.GetService<IWindow>();
		}

		void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddLogging();
			services.AddSingleton<ITextService, TextService>();
			services.AddTransient<MainPageViewModel>();
			services.AddTransient<MainPage>();
			services.AddTransient<IWindow, MainWindow>();
		}
	}

	//to use DI ServiceCollection and not the MAUI one
	public class DIExtensionsServiceProviderFactory : IServiceProviderFactory<ServiceCollection>
	{
		public ServiceCollection CreateBuilder(IServiceCollection services)
			=> new ServiceCollection { services };

		public IServiceProvider CreateServiceProvider(ServiceCollection containerBuilder)
			=> containerBuilder.BuildServiceProvider();
	}
}