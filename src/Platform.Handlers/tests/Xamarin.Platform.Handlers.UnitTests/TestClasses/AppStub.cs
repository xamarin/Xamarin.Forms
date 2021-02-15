using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform.Handlers.Tests
{
	class AppStub : App
	{
		public void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddTransient<IButton, ButtonStub>();
		}
	}
}
