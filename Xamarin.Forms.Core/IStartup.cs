using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if NETSTANDARD2_0
using Microsoft.Extensions.Hosting;

#endif

namespace Xamarin.Forms
{
	public interface IStartup
	{
#if NETSTANDARD2_0
		void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services);
		void ConfigureHostConfiguration(IConfigurationBuilder configureDelegate);
#endif
	}
}