using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.GTK
{
	internal class GtkPlatformServices : IPlatformServices
	{
		public bool IsInvokeRequired => Thread.CurrentThread.IsBackground;

		public string RuntimePlatform => Device.GTK;

		public void BeginInvokeOnMainThread(Action action)
		{
			GLib.Idle.Add(delegate { action(); return false; });
		}

		public Ticker CreateTicker()
		{
			return new GtkTicker();
		}

		public Assembly[] GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		public string GetHash(string input) => Crc64.GetHash(input);

		string IPlatformServices.GetMD5Hash(string input) => GetHash(input);

		public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
		{
			switch (size)
			{
				case NamedSize.Default:
					return 11;
				case NamedSize.Micro:
                case NamedSize.Caption:
                    return 12;
				case NamedSize.Medium:
					return 17;
				case NamedSize.Large:
					return 22;
                case NamedSize.Small:
                case NamedSize.Body:
                    return 14;
                case NamedSize.Header:
                    return 46;
                case NamedSize.Subtitle:
                    return 20;
                case NamedSize.Title:
                    return 24;
                default:
					throw new ArgumentOutOfRangeException(nameof(size));
			}
		}

		public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
		{
			using (var client = new HttpClient())
			{
				// Do not remove this await otherwise the client will dispose before
				// the stream even starts
				var result = await StreamWrapper.GetStreamAsync(uri, cancellationToken, client).ConfigureAwait(false);

				return result;
			}
		}

		public IIsolatedStorageFile GetUserStoreForApplication()
		{
			return new GtkIsolatedStorageFile();
		}

		public void OpenUriAction(Uri uri)
		{
			System.Diagnostics.Process.Start(uri.AbsoluteUri);
		}

		public void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			GLib.Timeout.Add((uint)interval.TotalMilliseconds, () =>
			{
				var result = callback();
				return result;
			});
		}

		public void StartTimer(TimeSpan interval, Func<Task<bool>> callback)
		{
			//Initialize `callbackResult` to `true` to ensure `ExecuteCallback` runs the first time GLib.TimeoutHandler is called
			bool callbackResult = true;

			//GLib.Timeout.Add cannot execute async/await code because TimeoutHandler is a delegate that returns `bool` (not Task<bool>)
			//To use asynchronous tasks, we leverage `ExecuteCallback` to update `callbackResult` using async/await
			GLib.Timeout.Add((uint)interval.TotalMilliseconds, () =>
			{
				//Verify the results of the previous call to ExecuteCallBack before calling ExecuteCallback again 
				if (callbackResult)
					ExecuteCallback();

				return callbackResult;
			});

			//Uses async void to execute the callback while still awaiting its result
			async void ExecuteCallback() => callbackResult = await callback();
		}

		private static int Hex(int v)
		{
			if (v < 10)
				return '0' + v;
			return 'a' + v - 10;
		}

		public void QuitApplication()
		{
			Gtk.Application.Quit();
		}

		public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			return Platform.GetNativeSize(view, widthConstraint, heightConstraint);
		}
	}
}
