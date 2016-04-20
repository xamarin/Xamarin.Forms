using System;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Reflection;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms.Internals;

#if WINDOWS_PHONE
using Xamarin.Forms.Platform.WinPhone;
#endif

[assembly:Dependency (typeof(MockDeserializer))]
[assembly:Dependency (typeof(MockResourcesProvider))]

namespace Xamarin.Forms.Core.UnitTests
{
	internal class MockPlatformServices : IPlatformServices
	{
		Action<Action> invokeOnMainThread;
		Action<Uri> openUriAction;
		Func<Uri, CancellationToken, Task<Stream>> getStreamAsync;
		public MockPlatformServices (Action<Action> invokeOnMainThread = null, Action<Uri> openUriAction = null, Func<Uri, CancellationToken, Task<Stream>> getStreamAsync = null)
		{
			this.invokeOnMainThread = invokeOnMainThread;
			this.openUriAction = openUriAction;
			this.getStreamAsync = getStreamAsync;
		}

		static MD5CryptoServiceProvider checksum = new MD5CryptoServiceProvider ();

		public string GetMD5Hash (string input)
		{
			var bytes = checksum.ComputeHash (Encoding.UTF8.GetBytes (input));
			var ret = new char [32];
			for (int i = 0; i < 16; i++){
				ret [i*2] = (char)hex (bytes [i] >> 4);
				ret [i*2+1] = (char)hex (bytes [i] & 0xf);
			}
			return new string (ret);
		}
		static int hex (int v)
		{
			if (v < 10)
				return '0' + v;
			return 'a' + v-10;
		}

		public double GetNamedSize (NamedSize size, Type targetElement, bool useOldSizes)
		{
			switch (size) {
				case NamedSize.Default:
					return 10;
				case NamedSize.Micro:
					return 4;
				case NamedSize.Small:
					return 8;
				case NamedSize.Medium:
					return 12;
				case NamedSize.Large:
					return 16;
				default:
					throw new ArgumentOutOfRangeException ("size");
			}
		}

		public void OpenUriAction (Uri uri)
		{
			if (openUriAction != null)
				openUriAction (uri);
			else
				throw new NotImplementedException ();
		}

		public bool IsInvokeRequired
		{
			get { return false; }
		}

		public void BeginInvokeOnMainThread (Action action) 
		{
			if (invokeOnMainThread == null)
				action ();
			else
				invokeOnMainThread (action);
		}

		public Ticker CreateTicker()
		{
			return new MockTicker();
		}

		public void StartTimer (TimeSpan interval, Func<bool> callback)
		{
			Timer timer = null;
			TimerCallback onTimeout = o => BeginInvokeOnMainThread (() => {
				if (callback ())
					return;

				timer.Dispose ();
			});
			timer = new Timer (onTimeout, null, interval, interval);
		}

		public Task<Stream> GetStreamAsync (Uri uri, CancellationToken cancellationToken)
		{
			if (getStreamAsync == null)
				throw new NotImplementedException ();
			return getStreamAsync (uri, cancellationToken);
		}

		public Assembly[] GetAssemblies ()
		{
			return AppDomain.CurrentDomain.GetAssemblies ();
		}

		public ITimer CreateTimer (Action<object> callback)
		{
			return new MockTimer (new Timer (o => callback(o)));
		}

		public ITimer CreateTimer (Action<object> callback, object state, int dueTime, int period)
		{
			return new MockTimer (new Timer (o => callback(o), state, dueTime, period));
		}

		public ITimer CreateTimer (Action<object> callback, object state, long dueTime, long period)
		{
			return new MockTimer (new Timer (o => callback(o), state, dueTime, period));
		}

		public ITimer CreateTimer (Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
		{
			return new MockTimer (new Timer (o => callback(o), state, dueTime, period));
		}

		public ITimer CreateTimer (Action<object> callback, object state, uint dueTime, uint period)
		{
			return new MockTimer (new Timer (o => callback(o), state, dueTime, period));
		}

		public class MockTimer : ITimer
		{
			readonly Timer timer;
			public MockTimer (Timer timer)
			{
				this.timer = timer;
			}

			public void Change (int dueTime, int period)
			{
				timer.Change (dueTime, period);
			}
			public void Change (long dueTime, long period)
			{
				timer.Change (dueTime, period);
			}
			public void Change (TimeSpan dueTime, TimeSpan period)
			{
				timer.Change (dueTime, period);
			}
			public void Change (uint dueTime, uint period)
			{
				timer.Change (dueTime, period);
			}
		}

		public IIsolatedStorageFile GetUserStoreForApplication ()
		{
#if WINDOWS_PHONE
			return new MockIsolatedStorageFile (IsolatedStorageFile.GetUserStoreForApplication ());
#else
			return new MockIsolatedStorageFile (IsolatedStorageFile.GetUserStoreForAssembly ());
#endif
		}

		public class MockIsolatedStorageFile : IIsolatedStorageFile
		{
			readonly IsolatedStorageFile isolatedStorageFile;
			public MockIsolatedStorageFile (IsolatedStorageFile isolatedStorageFile)
			{
				this.isolatedStorageFile = isolatedStorageFile;
			}

			public Task<bool> GetDirectoryExistsAsync (string path)
			{
				return Task.FromResult (isolatedStorageFile.DirectoryExists (path));
			}

			public Task CreateDirectoryAsync (string path)
			{
				isolatedStorageFile.CreateDirectory (path);
				return Task.FromResult (true);
			}

			public Task<Stream> OpenFileAsync (string path, FileMode mode, FileAccess access)
			{
				Stream stream = isolatedStorageFile.OpenFile (path, (System.IO.FileMode)mode, (System.IO.FileAccess)access);
				return Task.FromResult (stream);
			}

			public Task<Stream> OpenFileAsync (string path, FileMode mode, FileAccess access, FileShare share)
			{
				Stream stream = isolatedStorageFile.OpenFile (path, (System.IO.FileMode)mode, (System.IO.FileAccess)access, (System.IO.FileShare)share);
				return Task.FromResult (stream);
			}

			public Task<bool> GetFileExistsAsync (string path)
			{
				return Task.FromResult (isolatedStorageFile.FileExists (path));
			}

			public Task<DateTimeOffset> GetLastWriteTimeAsync (string path)
			{
				return Task.FromResult (isolatedStorageFile.GetLastWriteTime (path));
			}
		}
	}

	internal class MockDeserializer : IDeserializer
	{
		public Task<IDictionary<string, object>> DeserializePropertiesAsync ()
		{
			return Task.FromResult<IDictionary<string, object>> (new Dictionary<string,object> ());
		}

		public Task SerializePropertiesAsync (IDictionary<string, object> properties)
		{
			return Task.FromResult (false);
		}
	}

	internal class MockResourcesProvider : ISystemResourcesProvider
	{
		public IResourceDictionary GetSystemResources ()
		{
			var dictionary = new ResourceDictionary ();
			Style style;
			style = new Style (typeof(Label));
			dictionary [Device.Styles.BodyStyleKey] = style;

			style = new Style (typeof(Label));
			style.Setters.Add (Label.FontSizeProperty, 50);
			dictionary [Device.Styles.TitleStyleKey] = style;

			style = new Style (typeof(Label));
			style.Setters.Add (Label.FontSizeProperty, 40);
			dictionary [Device.Styles.SubtitleStyleKey] = style;

			style = new Style (typeof(Label));
			style.Setters.Add (Label.FontSizeProperty, 30);
			dictionary [Device.Styles.CaptionStyleKey] = style;

			style = new Style (typeof(Label));
			style.Setters.Add (Label.FontSizeProperty, 20);
			dictionary [Device.Styles.ListItemTextStyleKey] = style;

			style = new Style (typeof(Label));
			style.Setters.Add (Label.FontSizeProperty, 10);
			dictionary [Device.Styles.ListItemDetailTextStyleKey] = style;

			return dictionary;
		}
	}

	public class MockApplication : Application
	{
		public MockApplication ()
		{
		}
	}

	internal class MockTicker : Ticker
	{
		bool _enabled;

		protected override void EnableTimer()
		{
			_enabled = true;

			while (_enabled)
			{
				SendSignals(16);
			}
		}

		protected override void DisableTimer()
		{
			_enabled = false;
		}
	}
}