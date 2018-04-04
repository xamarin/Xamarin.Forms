using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using AImageView = Android.Widget.ImageView;

namespace Xamarin.Forms.Platform.Android
{

	internal static class ImageViewExtensions
	{
		public static void Reset(this FormsAnimationDrawable animation)
		{
			if (!animation.IsDisposed())
			{
				animation.Stop();
				int frameCount = animation.NumberOfFrames;
				for (int i = 0; i < frameCount; i++)
				{
					var currentFrame = animation.GetFrame(i);
					if (currentFrame is BitmapDrawable bitmapDrawable)
					{
						var bitmap = bitmapDrawable.Bitmap;
						if (bitmap != null)
						{
							if (!bitmap.IsRecycled)
							{
								bitmap.Recycle();
							}
							bitmap.Dispose();
							bitmap = null;
						}
						bitmapDrawable.Dispose();
						bitmapDrawable = null;
					}
					currentFrame = null;
				}
				animation = null;
			}
		}

		public static void Reset(this AImageView imageView)
		{
			if (!imageView.IsDisposed())
			{
				if (imageView.Drawable is FormsAnimationDrawable animation)
				{
					imageView.SetImageDrawable(null);
					animation.Reset();
				}

				imageView.SetImageResource(global::Android.Resource.Color.Transparent);
			}
		}

		// TODO hartez 2017/04/07 09:33:03 Review this again, not sure it's handling the transition from previousImage to 'null' newImage correctly
		public static async Task UpdateBitmap(this AImageView imageView, Image newImage, ImageSource source, Image previousImage = null, ImageSource previousImageSource = null)
		{
			if (imageView == null || imageView.IsDisposed())
				return;

			if (Device.IsInvokeRequired)
				throw new InvalidOperationException("Image Bitmap must not be updated from background thread");

			source = source ?? newImage?.Source;
			previousImageSource = previousImageSource ?? previousImage?.Source;

			if (Equals(previousImageSource, source))
				return;

			var imageController = newImage as IImageController;

			imageController?.SetIsLoading(true);

			(imageView as IImageRendererController)?.SkipInvalidate();

			imageView.Reset();

			Bitmap bitmap = null;
			FormsAnimationDrawable animation = null;
			Drawable drawable = null;
			IImageSourceHandler handler;

			if (source != null && (handler = Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source)) != null)
			{
				bool useAnimation = (newImage.IsSet(Image.IsAnimationAutoPlayProperty) || newImage.IsSet(Image.IsAnimationPlayingProperty)) && (handler is IImageSourceHandlerEx);
				if (handler is FileImageSourceHandler && !useAnimation)
				{
					drawable = imageView.Context.GetDrawable((FileImageSource)source);
				}

				if (drawable == null)
				{
					try
					{
						if (!useAnimation)
						{
							bitmap = await handler.LoadImageAsync(source, imageView.Context);
						}
						else
						{
							System.Diagnostics.Debug.Assert(handler is IImageSourceHandlerEx);
							animation = await ((IImageSourceHandlerEx)handler).LoadImageAnimationAsync(source, imageView.Context);
							if (animation == null)
							{
								// Fallback, try to load as regular bitmap.
								bitmap = await handler.LoadImageAsync(source, imageView.Context);
							}
						}
					}
					catch (TaskCanceledException)
					{
						imageController?.SetIsLoading(false);
					}
				}
			}

			// Check if the source on the new image has changed since the image was loaded
			if ((newImage != null && !Equals(newImage.Source, source)) || imageView.IsDisposed())
			{
				bitmap?.Dispose();
				animation?.Reset();
				animation?.Dispose();
				return;
			}

			if (drawable != null)
			{
				imageView.SetImageDrawable(drawable);
			}
			else if (bitmap != null)
			{
				imageView.SetImageBitmap(bitmap);
			}
			else if (animation != null)
			{
				imageView.SetImageDrawable(animation);
			}

			bitmap?.Dispose();
			imageController?.SetIsLoading(false);
			((IVisualElementController)newImage)?.NativeSizeChanged();

		}

		public static async Task UpdateBitmap(this AImageView imageView, Image newImage, Image previousImage = null)
		{
			await UpdateBitmap(imageView, newImage, newImage?.Source, previousImage, previousImage?.Source);

		}
	}
}
