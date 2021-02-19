using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Platform.Handlers.UnitTests.TestClasses;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform.Handlers.Tests
{
	class AppStub : App
	{
		public void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddSingleton<IHandlersContext>(provider => new HandlersContextStub(provider));
			services.AddTransient<IButton, ButtonStub>();
		}

		public override IAppHostBuilder CreateBuilder()
		{
			return base.CreateBuilder().ConfigureServices(ConfigureServices);
		}

		public override IWindow GetWindowFor(Dictionary<string, string> state)
		{
			return new WindowStub();
		}
	}
}
