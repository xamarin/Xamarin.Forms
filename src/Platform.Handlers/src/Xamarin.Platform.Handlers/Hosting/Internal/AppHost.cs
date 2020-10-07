using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Xamarin.Platform.Hosting.Internal
{
	internal class AppHost : IHost, IAsyncDisposable
	{
		readonly ILogger<AppHost> _logger;
		readonly IHostLifetime _hostLifetime;
		readonly ApplicationLifetime _applicationLifetime;
		readonly HostOptions _options;
		IEnumerable<IHostedService>? _hostedServices;

		public AppHost(IServiceProvider services, IHostApplicationLifetime applicationLifetime, ILogger<AppHost> logger,
			IHostLifetime hostLifetime, IOptions<HostOptions> options)
		{
			Services = services ?? throw new ArgumentNullException(nameof(services));
			_applicationLifetime = (applicationLifetime as ApplicationLifetime) ?? throw new ArgumentNullException(nameof(applicationLifetime));
			if (_applicationLifetime is null)
			{
				throw new ArgumentException("Replacing IHostApplicationLifetime is not supported.", nameof(applicationLifetime));
			}
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_hostLifetime = hostLifetime ?? throw new ArgumentNullException(nameof(hostLifetime));
			_options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public IServiceProvider Services { get; }

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			_logger.Starting();

			using var combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _applicationLifetime.ApplicationStopping);
			CancellationToken combinedCancellationToken = combinedCancellationTokenSource.Token;

			await _hostLifetime.WaitForStartAsync(combinedCancellationToken).ConfigureAwait(false);

			combinedCancellationToken.ThrowIfCancellationRequested();
			_hostedServices = Services?.GetService<IEnumerable<IHostedService>>();
			if (_hostedServices != null)
			{
				foreach (IHostedService hostedService in _hostedServices)
				{
					// Fire IHostedService.Start
					await hostedService.StartAsync(combinedCancellationToken).ConfigureAwait(false);

					if (hostedService is BackgroundService backgroundService)
					{
						//_ = HandleBackgroundException(backgroundService);
					}
				}
			}

			// Fire IHostApplicationLifetime.Started
			_applicationLifetime.NotifyStarted();

			_logger.Started();
		}

		public async Task StopAsync(CancellationToken cancellationToken = default)
		{
			_logger.Stopping();

			using (var cts = new CancellationTokenSource(_options.ShutdownTimeout))
			using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken))
			{
				CancellationToken token = linkedCts.Token;
				// Trigger IHostApplicationLifetime.ApplicationStopping
				_applicationLifetime.StopApplication();

				IList<Exception> exceptions = new List<Exception>();
				if (_hostedServices != null) // Started?
				{
					foreach (IHostedService hostedService in _hostedServices.Reverse())
					{
						try
						{
							await hostedService.StopAsync(token).ConfigureAwait(false);
						}
						catch (Exception ex)
						{
							exceptions.Add(ex);
						}
					}
				}

				// Fire IHostApplicationLifetime.Stopped
				_applicationLifetime.NotifyStopped();

				try
				{
					await _hostLifetime.StopAsync(token).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}

				if (exceptions.Count > 0)
				{
					var ex = new AggregateException("One or more hosted services failed to stop.", exceptions);
					_logger.StoppedWithException(ex);
					throw ex;
				}
			}

			_logger.Stopped();
		}

		public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();

		public async ValueTask DisposeAsync()
		{
			switch (Services)
			{
				case IAsyncDisposable asyncDisposable:
					await asyncDisposable.DisposeAsync().ConfigureAwait(false);
					break;
				case IDisposable disposable:
					disposable.Dispose();
					break;
			}
		}

		// I think is exists only in a new version
		//async Task HandleBackgroundException(BackgroundService backgroundService)
		//{
		//	try
		//	{
		//		//await backgroundService.ExecuteTask.ConfigureAwait(false);
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.BackgroundServiceFaulted(ex);
		//	}
		//}
	}
}
