using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Xamarin.Platform.Hosting.Internal
{
	internal class AppHostLifetime : IHostLifetime
	{
		public AppHostLifetime()
		{
		}

		public Task WaitForStartAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			// There's nothing to do here
			return Task.CompletedTask;
		}
	}
}
