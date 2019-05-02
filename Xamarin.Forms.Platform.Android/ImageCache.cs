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

		async Task PutAsync(string key, TimeSpan cacheValidity, global::Android.Graphics.Bitmap cacheObject)
		{
			await Task.Run(() =>
			{
				try
				{
					_lruCache.Put(key, new CacheEntry() { TimeToLive = DateTimeOffset.Now.Add(cacheValidity), Data = cacheObject });
				}
				catch
				{
					//just in case
				}


			}).ConfigureAwait(false);
		}

		public Task<Java.Lang.Object> GetAsync(string cacheKey, TimeSpan cacheValidity, Func<Task<Java.Lang.Object>> createMethod)
		{
			return Task.Run(async () =>
			{
				SemaphoreSlim semaphoreSlim = null;

				try
				{
					semaphoreSlim = _waiting.GetOrAdd(cacheKey, (key) => new SemaphoreSlim(1, 1));
					await semaphoreSlim.WaitAsync();

					var cacheEntry = _lruCache.Get(cacheKey) as CacheEntry;

					if (cacheEntry?.TimeToLive < DateTimeOffset.Now)
						cacheEntry = null;

					Java.Lang.Object innerCacheObject = null;
					if (cacheEntry == null && createMethod != null)
					{
						innerCacheObject = await createMethod();
						if(innerCacheObject is global::Android.Graphics.Bitmap bm)
							await PutAsync(cacheKey, cacheValidity, bm);
						else if (innerCacheObject is global::Android.Graphics.Drawables.BitmapDrawable bitmap)
							await PutAsync(cacheKey, cacheValidity, bitmap.Bitmap);
					}
					else
					{
						innerCacheObject = cacheEntry.Data;
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

		public class CacheEntry : Java.Lang.Object
		{
			bool _isDisposed;
			public DateTimeOffset TimeToLive { get; set; }
			public global::Android.Graphics.Bitmap Data { get; set; }

			protected override void Dispose(bool disposing)
			{
				if(!_isDisposed)
				{
					_isDisposed = true;
					Data = null;
				}

				base.Dispose(disposing);
			}
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
		}

	}
}
