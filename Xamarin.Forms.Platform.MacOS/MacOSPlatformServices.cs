using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using AppKit;
using Foundation;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.MacOS
{
	class MacOSPlatformServices : IPlatformServices
	{
		static readonly MD5CryptoServiceProvider Checksum = new MD5CryptoServiceProvider();

		public bool IsInvokeRequired
		{
			get { return !NSThread.IsMain; }
		}

		public void BeginInvokeOnMainThread(Action action)
		{
			NSRunLoop.Main.BeginInvokeOnMainThread(action.Invoke);
		}

		public Ticker CreateTicker()
		{
			return new CADisplayLinkTicker();
		}

		public Assembly[] GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		public string GetMD5Hash(string input)
		{
			var bytes = Checksum.ComputeHash(Encoding.UTF8.GetBytes(input));
			var ret = new char[32];
			for (var i = 0; i < 16; i++)
			{
				ret[i * 2] = (char)Hex(bytes[i] >> 4);
				ret[i * 2 + 1] = (char)Hex(bytes[i] & 0xf);
			}
			return new string(ret);
		}

		public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
		{
			switch (size)
			{
				case NamedSize.Default:
					return typeof(Button).IsAssignableFrom(targetElementType) ? 15 : 17;
				case NamedSize.Micro:
					return 12;
				case NamedSize.Small:
					return 14;
				case NamedSize.Medium:
					return 17;
				case NamedSize.Large:
					return 22;
				default:
					throw new ArgumentOutOfRangeException("size");
			}
		}

		public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
		{
			using (var client = GetHttpClient())
			using (var response = await client.GetAsync(uri, cancellationToken))
				return await response.Content.ReadAsStreamAsync();
		}

		public IIsolatedStorageFile GetUserStoreForApplication()
		{
			return new MacOSIsolatedStorageFile(IsolatedStorageFile.GetUserStoreForApplication());
		}

		public void OpenUriAction(Uri uri)
		{
			NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl(uri.AbsoluteUri));
		}

		public void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			NSTimer timer = null;
			timer = NSTimer.CreateRepeatingScheduledTimer(interval, t =>
			{
				if (!callback())
					t.Invalidate();
			});
			NSRunLoop.Main.AddTimer(timer, NSRunLoopMode.Common);
		}

		static HttpClient GetHttpClient()
		{
			var proxy = CoreFoundation.CFNetwork.GetSystemProxySettings();
			var handler = new HttpClientHandler();
			if (!string.IsNullOrEmpty(proxy.HTTPProxy))
			{
				handler.Proxy = CoreFoundation.CFNetwork.GetDefaultProxy();
				handler.UseProxy = true;
			}
			return new HttpClient(handler);
		}

		static int Hex(int v)
		{
			if (v < 10)
				return '0' + v;
			return 'a' + v - 10;
		}
	}
}