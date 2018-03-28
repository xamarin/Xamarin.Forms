using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class StreamImagesourceHandler : IImageSourceHandlerEx
	{
		public async Task<Bitmap> LoadImageAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			var streamsource = imagesource as StreamImageSource;
			Bitmap bitmap = null;
			if (streamsource?.Stream != null)
			{
				using (Stream stream = await ((IStreamImageSource)streamsource).GetStreamAsync(cancelationToken).ConfigureAwait(false))
					bitmap = await BitmapFactory.DecodeStreamAsync(stream).ConfigureAwait(false);
			}

			if (bitmap == null)
			{
				Log.Warning(nameof(ImageLoaderSourceHandler), "Image data was invalid: {0}", streamsource);
			}

			return bitmap;
		}

		public async Task<AnimationDrawable> LoadImageAnimationAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			var streamSource = imagesource as StreamImageSource;
			AnimationDrawable animation = null;
			if (streamSource?.Stream != null)
			{
				using (Stream stream = await ((IStreamImageSource)streamSource).GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					int sourceDensity = 1;
					int targetDensity = 1;

					if (stream.CanSeek)
					{
						BitmapFactory.Options options = new BitmapFactory.Options();
						options.InJustDecodeBounds = true;
						await BitmapFactory.DecodeStreamAsync(stream, null, options);
						sourceDensity = options.InDensity;
						targetDensity = options.InTargetDensity;
						stream.Seek(0, SeekOrigin.Begin);
					}

					var decoder = new AndroidGIFImageParser(context, sourceDensity, targetDensity);
					await decoder.ParseAsync(stream);

					animation = decoder.Animation;
				}
			}

			if (animation == null)
			{
				Log.Warning(nameof(ImageLoaderSourceHandler), "Image data was invalid: {0}", streamSource);
			}

			return animation;
		}
	}
}