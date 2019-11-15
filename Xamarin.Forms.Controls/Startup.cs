using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Xamarin.Forms.Controls
{
	public class Startup : IStartup
	{
		public void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
		{
			if (hostBuilderContext.HostingEnvironment.IsDevelopment())
			{
				var world = hostBuilderContext.Configuration["Hello"];

			}
			services.AddSingleton<App>();
		}

		public void ConfigureHostConfiguration(IConfigurationBuilder configurationBuilder)
		{
		
		}

		public void ConfigureAppConfiguration(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder)
		{
			
		}
	}
}