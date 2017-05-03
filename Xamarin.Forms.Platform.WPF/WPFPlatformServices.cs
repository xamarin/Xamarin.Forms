using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.WPF
{
	internal class WPFPlatformServices : IPlatformServices
	{
		static readonly MD5CryptoServiceProvider Checksum = new MD5CryptoServiceProvider();

		public void BeginInvokeOnMainThread(Action action)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(action));
		}

		public Ticker CreateTicker()
		{
			return new WPFTicker();
		}

		public Assembly[] GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		public string GetMD5Hash(string input)
		{
			byte[] bytes = Checksum.ComputeHash(Encoding.UTF8.GetBytes(input));
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
					if (typeof(Label).IsAssignableFrom(targetElementType))
						return (double)System.Windows.Application.Current.Resources["FontSizeNormal"];
					return (double)System.Windows.Application.Current.Resources["FontSizeMedium"];
				case NamedSize.Micro:
					return (double)System.Windows.Application.Current.Resources["FontSizeSmall"] - 3;
				case NamedSize.Small:
					return (double)System.Windows.Application.Current.Resources["FontSizeSmall"];
				case NamedSize.Medium:
					if (useOldSizes)
						goto case NamedSize.Default;
					return (double)System.Windows.Application.Current.Resources["FontSizeMedium"];
				case NamedSize.Large:
					return (double)System.Windows.Application.Current.Resources["FontSizeLarge"];
				default:
					throw new ArgumentOutOfRangeException("size");
			}
		}

		public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
		{
			var tcs = new TaskCompletionSource<Stream>();

			try
			{
				HttpWebRequest request = WebRequest.CreateHttp(uri);
				request.AllowReadStreamBuffering = true;
				request.BeginGetResponse(ar =>
				{
					if (cancellationToken.IsCancellationRequested)
					{
						tcs.SetCanceled();
						return;
					}

					try
					{
						Stream stream = request.EndGetResponse(ar).GetResponseStream();
						tcs.TrySetResult(stream);
					}
					catch (Exception ex)
					{
						tcs.TrySetException(ex);
					}
				}, null);
			}
			catch (Exception ex)
			{
				tcs.TrySetException(ex);
			}

			return tcs.Task;
		}

		public IIsolatedStorageFile GetUserStoreForApplication()
		{
			return new _IsolatedStorageFile(IsolatedStorageFile.GetUserStoreForApplication());
		}
		public bool IsInvokeRequired => Dispatcher.CurrentDispatcher.CheckAccess();

		public void OpenUriAction(Uri uri)
		{
			System.Diagnostics.Process.Start(uri.ToString());
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

		static int Hex(int v)
		{
			if (v < 10)
				return '0' + v;
			return 'a' + v - 10;
		}

		public class _IsolatedStorageFile : IIsolatedStorageFile
		{
			readonly IsolatedStorageFile _isolatedStorageFile;

			public _IsolatedStorageFile(IsolatedStorageFile isolatedStorageFile)
			{
				_isolatedStorageFile = isolatedStorageFile;
			}

			public Task CreateDirectoryAsync(string path)
			{
				_isolatedStorageFile.CreateDirectory(path);
				return Task.FromResult(true);
			}

			public Task<bool> GetDirectoryExistsAsync(string path)
			{
				return Task.FromResult(_isolatedStorageFile.DirectoryExists(path));
			}

			public Task<bool> GetFileExistsAsync(string path)
			{
				return Task.FromResult(_isolatedStorageFile.FileExists(path));
			}

			public Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
			{
				return Task.FromResult(_isolatedStorageFile.GetLastWriteTime(path));
			}

			public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
			{
				Stream stream = _isolatedStorageFile.OpenFile(path, (System.IO.FileMode)mode, (System.IO.FileAccess)access);
				return Task.FromResult(stream);
			}

			public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
			{
				Stream stream = _isolatedStorageFile.OpenFile(path, (System.IO.FileMode)mode, (System.IO.FileAccess)access, (System.IO.FileShare)share);
				return Task.FromResult(stream);
			}
		}
	}
}
