using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using AImageView = Android.Widget.ImageView;

namespace Xamarin.Forms.Platform.Android
{

	internal static class ImageViewExtensions
	{
		public static Task UpdateBitmap(this AImageView imageView, IImageElement newView, IImageElement previousView) =>
			imageView.UpdateBitmap(newView, previousView, null, null);

		public static Task UpdateBitmap(this AImageView imageView, ImageSource newImageSource, ImageSource previousImageSourc) =>
			imageView.UpdateBitmap(null, null, newImageSource, previousImageSourc);

		static async Task UpdateBitmap(
			this AImageView imageView,
			IImageElement newView,
			IImageElement previousView,
			ImageSource newImageSource,
			ImageSource previousImageSource)
		public static void Reset(this AnimationDrawable animation)
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
				if (imageView.Drawable is AnimationDrawable animation)
				{
					imageView.SetImageDrawable(null);
					animation.Reset();
					animation.Dispose();
					animation = null;
				}

				imageView.SetImageResource(global::Android.Resource.Color.Transparent);
			}
		}

		// TODO hartez 2017/04/07 09:33:03 Review this again, not sure it's handling the transition from previousImage to 'null' newImage correctly
		public static async Task UpdateBitmap(this AImageView imageView, Image newImage, ImageSource source, Image previousImage = null, ImageSource previousImageSource = null)
		{

			IImageController imageController = newView as IImageController;
			newImageSource = newImageSource ?? newView?.Source;
			previousImageSource = previousImageSource ?? previousView?.Source;

			if (imageView.IsDisposed())
				return;

			if (newImageSource != null && Equals(previousImageSource, newImageSource))
				return;

			imageController?.SetIsLoading(true);

			(imageView as IImageRendererController)?.SkipInvalidate();
			imageView.SetImageResource(global::Android.Resource.Color.Transparent);

			try
			{
				if (newImageSource != null)
				{
					var imageViewHandler = Internals.Registrar.Registered.GetHandlerForObject<IImageViewHandler>(newImageSource);
					if (imageViewHandler != null)
					{
						await imageViewHandler.LoadImageAsync(newImageSource, imageView);
					}
					else
					{
						using (var drawable = await imageView.Context.GetFormsDrawableAsync(newImageSource))
						{
							// only set the image if we are still on the same one
							if (!imageView.IsDisposed() && SourceIsNotChanged(newView, newImageSource))
								imageView.SetImageDrawable(drawable);
						}
					}
				}
				else
				{
					imageView.SetImageBitmap(null);
				}
			}
			finally
			{
				// only mark as finished if we are still working on the same image
				if (SourceIsNotChanged(newView, newImageSource))
				{
					imageController?.SetIsLoading(false);
					imageController?.NativeSizeChanged();
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
                AnimationDrawable animation = null;
                Drawable drawable = null;
                IImageSourceHandlerEx handler;
                bool useAnimation = newImage.IsSet(Image.AnimationPlayBehaviorProperty) || newImage.IsSet(Image.IsAnimationPlayingProperty);

                if (source != null && (handler = Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandlerEx>(source)) != null)
                {
                    if (handler is FileImageSourceHandler && !useAnimation)
                    {
                        drawable = imageView.Context.GetDrawable((FileImageSource)source);
                    }

                    if (drawable == null)
                    {
                        try
                        {
                            if (!useAnimation)
                                bitmap = await handler.LoadImageAsync(source, imageView.Context);
                            else
                                animation = await handler.LoadImageAnimationAsync(source, imageView.Context);
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
                    if ((Image.AnimationPlayBehaviorValue)newImage.GetValue(Image.AnimationPlayBehaviorProperty) == Image.AnimationPlayBehaviorValue.OnLoad)
                        animation.Start();
                }

                bitmap?.Dispose();
                imageController?.SetIsLoading(false);
                ((IVisualElementController)newImage)?.NativeSizeChanged();

            }

            bool SourceIsNotChanged(IImageElement imageElement, ImageSource imageSource)
			{
				return (imageElement != null) ? imageElement.Source == imageSource : true;
			}
		}

		internal static async void SetImage(this AImageView image, ImageSource source, Context context)
		{
			image.SetImageDrawable(await context.GetFormsDrawableAsync(source));
		}
	}
}
