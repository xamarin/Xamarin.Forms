using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class ImageLoaderSourceHandler : IImageSourceHandlerEx
	{
		public async Task<Bitmap> LoadImageAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			var imageLoader = imagesource as UriImageSource;
			Bitmap bitmap = null;
			if (imageLoader?.Uri != null)
			{
				using (Stream imageStream = await imageLoader.GetStreamAsync(cancelationToken).ConfigureAwait(false))
					bitmap = await BitmapFactory.DecodeStreamAsync(imageStream).ConfigureAwait(false);
			}

			if (bitmap == null)
			{
				Log.Warning(nameof(ImageLoaderSourceHandler), "Could not retrieve image or image data was invalid: {0}", imageLoader);
			}

			return bitmap;
		}

		public async Task<AnimationDrawable> LoadImageAnimationAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			var imageLoader = imagesource as UriImageSource;
			AnimationDrawable animation = null;
			if (imageLoader?.Uri != null)
			{
				using (Stream imageStream = await imageLoader.GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					var decoder = new AndroidGIFImageParser(context, 1, 1);
					await decoder.ParseAsync(imageStream);

					animation = decoder.Animation;
				}
			}

			if (animation == null)
			{
				Log.Warning(nameof(ImageLoaderSourceHandler), "Could not retrieve image or image data was invalid: {0}", imageLoader);
			}

			return animation;
		}
	}
}