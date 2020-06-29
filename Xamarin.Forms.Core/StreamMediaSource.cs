using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public class StreamMediaSource : MediaSource, IStreamImageSource
	{
		readonly object _synchandle = new object();
		CancellationTokenSource _cancellationTokenSource;

		TaskCompletionSource<bool> _completionSource;

		public static readonly BindableProperty StreamProperty = BindableProperty.Create(nameof(Stream), typeof(Func<CancellationToken, Task<Stream>>), typeof(StreamMediaSource),
			default(Func<CancellationToken, Task<Stream>>));

		protected CancellationTokenSource CancellationTokenSource
		{
			get { return _cancellationTokenSource; }
			private set
			{
				if (_cancellationTokenSource == value)
					return;
				if (_cancellationTokenSource != null)
				{
					_cancellationTokenSource.Cancel();
					_cancellationTokenSource.Dispose();
				}
				_cancellationTokenSource = value;
			}
		}

		bool IsLoading
		{
			get { return _cancellationTokenSource != null; }
		}

		public virtual Func<CancellationToken, Task<Stream>> Stream
		{
			get => (Func<CancellationToken, Task<Stream>>)GetValue(StreamProperty);
			set => SetValue(StreamProperty, value);
		}

		protected override void OnPropertyChanged(string propertyName)
		{
			if (propertyName == StreamProperty.PropertyName)
				OnSourceChanged();
			base.OnPropertyChanged(propertyName);
		} 

		async Task<Stream> IStreamImageSource.GetStreamAsync(CancellationToken userToken)
		{
			if (Stream == null)
				return null;

			OnLoadingStarted();
			userToken.Register(CancellationTokenSource.Cancel);
			try
			{
				Stream stream = await Stream(CancellationTokenSource.Token);
				OnLoadingCompleted(false);
				return stream;
			}
			catch (OperationCanceledException)
			{
				OnLoadingCompleted(true);
				throw;
			}
		}

		protected void OnLoadingCompleted(bool cancelled)
		{
			if (!IsLoading || _completionSource == null)
				return;

			TaskCompletionSource<bool> tcs = Interlocked.Exchange(ref _completionSource, null);
			if (tcs != null)
				tcs.SetResult(cancelled);

			lock (_synchandle)
			{
				CancellationTokenSource = null;
			}
		}

		protected void OnLoadingStarted()
		{
			lock (_synchandle)
			{
				CancellationTokenSource = new CancellationTokenSource();
			}
		}

		public virtual Task<bool> Cancel()
		{
			if (!IsLoading)
				return Task.FromResult(false);

			var tcs = new TaskCompletionSource<bool>();
			TaskCompletionSource<bool> original = Interlocked.CompareExchange(ref _completionSource, tcs, null);
			if (original == null)
			{
				CancellationTokenSource = null;
			}
			else
				tcs = original;

			return tcs.Task;
		}
	}
}