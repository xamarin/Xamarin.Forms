using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using System;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class ImageLoaderSourceHandler : IImageSourceHandler
	{
		public async Task<Bitmap> LoadImageAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			var imageLoader = imagesource as UriImageSource;
			if (imageLoader != null && imageLoader.Uri != null)
			{
				using (Stream imageStream = await imageLoader.GetStreamAsync(cancelationToken).ConfigureAwait(false))
					return await BitmapFactory.DecodeStreamAsync(imageStream).ConfigureAwait(false);
			}
			return null;
		}

		public async Task<Movie> LoadAnimatedImageAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			var imageLoader = imagesource as UriImageSource;
			if (imageLoader != null && imageLoader.Uri != null)
			{
				using (Stream imageStream = await imageLoader.GetStreamAsync(cancelationToken).ConfigureAwait(false))
					return await Movie.DecodeStreamAsync(imageStream).ConfigureAwait(false);
			}
			return null;
		}
	}
}