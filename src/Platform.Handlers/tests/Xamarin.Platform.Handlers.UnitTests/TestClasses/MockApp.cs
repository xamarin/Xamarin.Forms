using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform.Handlers.Tests
{
	public partial class HostBuilderTests
	{
		class MockApp : App
		{
			public void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
			{
				services.AddTransient<IStartup, MockButton>();
			}
		}
	}
}
