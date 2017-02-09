using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using System;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class FileImageSourceHandler : IImageSourceHandler
	{
		// This is set to true when run under designer context
		internal static bool DecodeSynchronously
		{
			get;
			set;
		}

		public async Task<Bitmap> LoadImageAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			string file = ((FileImageSource)imagesource).File;
			if (File.Exists(file))
				return !DecodeSynchronously ? (await BitmapFactory.DecodeFileAsync(file).ConfigureAwait(false)) : BitmapFactory.DecodeFile(file);
			else
				return !DecodeSynchronously ? (await context.Resources.GetBitmapAsync(file).ConfigureAwait(false)) : context.Resources.GetBitmap(file);
		}

		public async Task<Movie> LoadAnimatedImageAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			string file = ((FileImageSource)imagesource).File;

			if (File.Exists(file))
				return !DecodeSynchronously ? (await Movie.DecodeFileAsync(file).ConfigureAwait(false)) : Movie.DecodeFile(file);
			else
			{
				var id = ResourceManager.IdFromTitle(file, ResourceManager.DrawableClass);
				Movie movie = null;
				try
				{
					var stream = context.Resources.OpenRawResource(id);
					movie = await Movie.DecodeStreamAsync(stream).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					Log.Warning("Xamarin.Forms.Platform.Android.FileImageSourceHandler", "Resource not found: {0}", ex);
				}

				return movie;
			}
		}

	}
}