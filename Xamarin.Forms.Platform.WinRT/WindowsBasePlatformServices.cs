﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Xamarin.Forms.Internals;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	internal abstract class WindowsBasePlatformServices : IPlatformServices
	{
		CoreDispatcher _dispatcher;

		public WindowsBasePlatformServices(CoreDispatcher dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			_dispatcher = dispatcher;
		}

		public void BeginInvokeOnMainThread(Action action)
		{
			_dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).WatchForError();
		}

		public Ticker CreateTicker()
		{
			return new WindowsTicker();
		}

		public virtual Assembly[] GetAssemblies()
		{
			var options = new QueryOptions { FileTypeFilter = { ".exe", ".dll" } };

			StorageFileQueryResult query = Package.Current.InstalledLocation.CreateFileQueryWithOptions(options);
			IReadOnlyList<StorageFile> files = query.GetFilesAsync().AsTask().Result;

			var assemblies = new List<Assembly>(files.Count);
			for (var i = 0; i < files.Count; i++)
			{
				StorageFile file = files[i];
				try
				{
					Assembly assembly = Assembly.Load(new AssemblyName { Name = Path.GetFileNameWithoutExtension(file.Name) });

					assemblies.Add(assembly);
				}
				catch (IOException)
				{
				}
				catch (BadImageFormatException)
				{
				}
			}

			Assembly thisAssembly = GetType().GetTypeInfo().Assembly;
			// this happens with .NET Native
			if (!assemblies.Contains(thisAssembly))
				assemblies.Add(thisAssembly);

			return assemblies.ToArray();
		}

		public string GetMD5Hash(string input)
		{
			HashAlgorithmProvider algorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
			IBuffer buffer = algorithm.HashData(Encoding.Unicode.GetBytes(input).AsBuffer());
			return CryptographicBuffer.EncodeToHexString(buffer);
		}

		public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
		{
			return size.GetFontSize();
		}

		public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
		{
			using (var client = new HttpClient())
			{
				HttpResponseMessage streamResponse = await client.GetAsync(uri.AbsoluteUri).ConfigureAwait(false);
				return streamResponse.IsSuccessStatusCode ? await streamResponse.Content.ReadAsStreamAsync().ConfigureAwait(false) : null;
			}
		}

		public IIsolatedStorageFile GetUserStoreForApplication()
		{
			return new WindowsIsolatedStorage(ApplicationData.Current.LocalFolder);
		}

		public bool IsInvokeRequired => !CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess;

		public void OpenUriAction(Uri uri)
		{
			Launcher.LaunchUriAsync(uri).WatchForError();
		}

		public void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			var timer = new DispatcherTimer { Interval = interval };
			timer.Start();
			timer.Tick += (sender, args) =>
			{
				bool result = callback();
				if (!result)
					timer.Stop();
			};
		}

		internal class WindowsTimer : ITimer
		{
			readonly Timer _timer;

			public WindowsTimer(Timer timer)
			{
				_timer = timer;
			}

			public void Change(int dueTime, int period)
			{
				_timer.Change(dueTime, period);
			}

			public void Change(long dueTime, long period)
			{
				Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
			}

			public void Change(TimeSpan dueTime, TimeSpan period)
			{
				_timer.Change(dueTime, period);
			}

			public void Change(uint dueTime, uint period)
			{
				Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
			}
		}
	}
}