using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.Android
{
	/// <summary>
	/// I setup the access to all the cache elements to be async because
	/// if I didn't then it was locking up the GC and freezing the entire app
	/// </summary>
	class ImageCache
	{

		const string _cachePlaceHolder = "PLEASEHOLD";

		global::Android.Util.LruCache _lruCache;

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

		public ImageCache() : base()
		{
			_lruCache = new global::Android.Util.LruCache(GetCacheSize());
		}

		public async void Put(string key, Java.Lang.Object cacheObject)
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

		public async void PutLoadingKey(string key)
		{
			await Task.Run(() =>
			{
				try
				{
					_lruCache.Put(key, _cachePlaceHolder);
				}
				catch
				{
					//just in case
				}

			}).ConfigureAwait(false);
		}

		public Task<Java.Lang.Object> GetAsync(string cacheKey)
		{
			return Task.Run(async () =>
			{
				try
				{
					var innerCacheObject = _lruCache.Get(cacheKey);

					// this only really gets hit during load when there are a lot of requests for the same image
					while (innerCacheObject?.ToString() == _cachePlaceHolder)
					{
						await Task.Delay(100).ConfigureAwait(false);
						innerCacheObject = _lruCache.Get(cacheKey);
					}

					return innerCacheObject;
				}
				catch
				{
					//just in case
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
	}
}
