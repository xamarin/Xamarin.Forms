using System;
using System.Collections.Generic;
using Maui.Controls.Sample.Pages;
using Maui.Controls.Sample.Services;
using Maui.Controls.Sample.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;

namespace Maui.Controls.Sample
{
	public class MyApp : MauiApp
	{
		public override IAppHostBuilder CreateBuilder() =>
			base.CreateBuilder()
				//.ConfigureLogging(logging =>
				//{
				//	logging.ClearProviders();
				//	logging.AddConsole();
				//})
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					config.AddInMemoryCollection(new Dictionary<string, string>
					{
						{ "MyKey", "Dictionary MyKey Value" },
						{ ":Title", "Dictionary_Title" },
						{ "Position:Name", "Dictionary_Name" },
						{ "Logging:LogLevel:Default", "Warning" }
					});
				})
				.ConfigureServices((hostingContext, services) =>
				{
					services.AddSingleton<ITextService, TextService>();
					services.AddTransient<MainPageViewModel>();
					services.AddTransient<MainPage>();
					services.AddTransient<IWindow, MainWindow>();
				})
				.ConfigureFonts((hostingContext, fonts) =>
				{
					fonts.AddFont("dokdo_regular.ttf", "Dokdo");
				});

		//IAppState state
		public override IWindow GetWindowFor(IActivationState state)
		{
			return Services.GetService<IWindow>();
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