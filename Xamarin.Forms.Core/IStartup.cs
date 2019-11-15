#if NETSTANDARD2_0
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endif

namespace Xamarin.Forms
{
	public interface IStartup
	{
#if NETSTANDARD2_0
		void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services);
		void ConfigureHostConfiguration(IConfigurationBuilder configurationBuilder);
		void ConfigureAppConfiguration(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder);
#endif
	}
}