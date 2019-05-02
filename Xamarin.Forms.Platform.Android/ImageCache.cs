using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.Android
{
	/// <summary>
	/// I setup the access to all the cache elements to be async because
	/// if I didn't then it was locking up the GC and freezing the entire app
	/// </summary>
	class ImageCache
	{
		readonly FormsLruCache _lruCache;
		readonly ConcurrentDictionary<string, SemaphoreSlim> _waiting;

		public ImageCache() : base()
		{
			_waiting = new ConcurrentDictionary<string, SemaphoreSlim>();
			_lruCache = new FormsLruCache();
		}

		public async Task PutAsync(string key, Java.Lang.Object cacheObject)
		{
			await Task.Run(() =>
			{
				try
				{
					_lruCache.Put(key, cacheObject);
				}
				catch
				{
					//just in case
				}


			}).ConfigureAwait(false);
		}

		public Task<Java.Lang.Object> GetAsync(string cacheKey, Func<Task<Java.Lang.Object>> createMethod)
		{
			return Task.Run(async () =>
			{
				SemaphoreSlim semaphoreSlim = null;

				try
				{
					semaphoreSlim = _waiting.GetOrAdd(cacheKey, (key) => new SemaphoreSlim(1, 1));
					await semaphoreSlim.WaitAsync();

					var innerCacheObject = _lruCache.Get(cacheKey);
					
					if(innerCacheObject == null && createMethod != null)
					{
						innerCacheObject = await createMethod();
						if(innerCacheObject is global::Android.Graphics.Bitmap)
							await PutAsync(cacheKey, innerCacheObject);
						else if (innerCacheObject is global::Android.Graphics.Drawables.BitmapDrawable bitmap)
							await PutAsync(cacheKey, bitmap.Bitmap);
					}

					return innerCacheObject;
				}
				catch
				{
					//just in case
				}
				finally
				{
					semaphoreSlim?.Release();
				}

				return null;
			});
		}

		public async void Remove(string key)
		{
			await Task.Run(() =>
			{
				try
				{
					_lruCache.Remove(key);
				}
				catch
				{
					//just in case
				}

			}).ConfigureAwait(false);
		}
		public class FormsLruCache : global::Android.Util.LruCache
		{

			static int GetCacheSize()
			{
				// https://developer.android.com/topic/performance/graphics/cache-bitmap
				int cacheSize = 4 * 1024 * 1024;
				if (Java.Lang.Runtime.GetRuntime()?.MaxMemory() != null)
				{
					var maxMemory = (int)(Java.Lang.Runtime.GetRuntime().MaxMemory() / 1024);
					cacheSize = maxMemory / 8;
				}
				return cacheSize;
			}

			public FormsLruCache() : base(GetCacheSize())
			{
			}

			protected override int SizeOf(Java.Lang.Object key, Java.Lang.Object value)
			{
				if (value != null && value is global::Android.Graphics.Bitmap bitmap)
					return bitmap.ByteCount / 1024;

				return base.SizeOf(key, value);
			}

			protected override void EntryRemoved(bool evicted, Java.Lang.Object key, Java.Lang.Object oldValue, Java.Lang.Object newValue)
			{
				base.EntryRemoved(evicted, key, oldValue, newValue);
			}
		}

	}
}
