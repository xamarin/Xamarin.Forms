using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Maui.Controls.Sample.Pages;
using Maui.Controls.Sample.Services;
using Maui.Controls.Sample.ViewModel;
using Xamarin.Platform;
using Xamarin.Platform.Hosting;
using System;

using System.Linq;
namespace Maui.Controls.Sample
{
	public class MyApp : App
	{
		public MyApp()
		{

		}
		public override IAppHostBuilder Builder()
		{

			//public MyApp(IServiceProvider provider) : base(provider)
			//{
			//}

			var app = CreateDefaultBuilder()
							//.RegisterHandlers(new Dictionary<Type, Type>
							//		{
							//			{ typeof(VerticalStackLayout),typeof(LayoutHandler) },
							//			{ typeof(HorizontalStackLayout),typeof(LayoutHandler) },
							//		})
							.ConfigureServices(ConfigureMyAppServices);
							//.UseServiceProviderFactory(new DIExtensionsServiceProviderFactory());

			return (IAppHostBuilder)app;
		}

		public void ConfigureMyAppServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddSingleton<ITextService, TextService>();
			services.AddTransient<MainPageViewModel>();
			services.AddTransient<MainPage>();
			services.AddTransient<IWindow, MainWindow>();
		}

		public IWindow GetWindowFor(Dictionary<string,string> valuePairs)
		{
			return Services.GetService<IWindow>();
		}
	}

	public class DIExtensionsServiceProviderFactory : IServiceProviderFactory<ServiceCollection>
	{
		public ServiceCollection CreateBuilder(IServiceCollection services)
		//=> new ServiceCollection { services };
		{
			var sc = new ServiceCollection();

			foreach (var y in services)
			{
				sc.Add(y);
			}
			return sc;
		}

		public IServiceProvider CreateServiceProvider(ServiceCollection containerBuilder)
			=> containerBuilder.BuildServiceProvider();
	}
}