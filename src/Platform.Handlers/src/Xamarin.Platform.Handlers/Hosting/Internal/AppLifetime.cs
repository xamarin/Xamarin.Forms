using System;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace Xamarin.Platform.Hosting.Internal
{
	internal class AppLifetime : IHostApplicationLifetime
	{
		public AppLifetime()
		{
		}

		public CancellationToken ApplicationStarted => throw new NotImplementedException();

	
		public CancellationToken ApplicationPausing => throw new NotImplementedException();

		public CancellationToken ApplicationPaused => throw new NotImplementedException();

		public CancellationToken ApplicationStopping => throw new NotImplementedException();

		public CancellationToken ApplicationStopped => throw new NotImplementedException();

		public void ResumeApplication()
		{

		}

		public void PauseApplication()
		{

		}

		public void StopApplication()
		{
		
		}
	}
}
