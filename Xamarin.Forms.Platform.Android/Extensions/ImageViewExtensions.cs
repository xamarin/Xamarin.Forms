using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Xamarin.Forms.Internals;
using Android.Content;
using Android.Graphics;
using AImageView = Android.Widget.ImageView;
using Android.Views.Accessibility;

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
		{
			IImageController imageController = newView as IImageController;
			newImageSource ??= newView?.Source;
			previousImageSource ??= previousView?.Source;

			if (imageView.IsDisposed())
				return;

			if (newImageSource != null && Equals(previousImageSource, newImageSource))
				return;

			imageController?.SetIsLoading(true);

			(imageView as IImageRendererController)?.SkipInvalidate();
			imageView.Reset();
			imageView.SetImageResource(global::Android.Resource.Color.Transparent);

			try
			{
				bool imageLoadedSuccessfully = false;
				try
				{
					if (newView.LoadingSource != null)
					{
						imageLoadedSuccessfully = await imageView.TryToUpdateBitmap(newView, newView.LoadingSource, imageController, newImageSource);
					}

					if (newImageSource != null)
					{
						imageLoadedSuccessfully = await imageView.TryToUpdateBitmap(newView, newImageSource, imageController, newImageSource);
					}
					else if (newView.LoadingSource == null)
					{
						imageLoadedSuccessfully = true;
						imageView.SetImageBitmap(null);
					}
				}
				finally
				{
					if (SourceIsNotChanged(newView, newImageSource) && 
						newImageSource != null && 
						!imageLoadedSuccessfully &&
						newView.ErrorSource != null)
					{
						await imageView.TryToUpdateBitmap(newView, newView.ErrorSource, imageController, newImageSource);
					}
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
		}

		static async Task<bool> TryToUpdateBitmap(
			this AImageView imageView,
			IImageElement newView,
			ImageSource imageSourceToLoad,
			IImageController imageController,
			ImageSource imageSourceToCheckIfUserChangedImageSource)
		{
			
			// all this animation code will go away if/once we pull in GlideX
			IFormsAnimationDrawable animation = null;

			if (imageController != null && imageController.GetLoadAsAnimation())
			{
				var animationHandler = Registrar.Registered.GetHandlerForObject<IAnimationSourceHandler>(imageSourceToLoad);
				if (animationHandler != null)
					animation = await animationHandler.LoadImageAnimationAsync(imageSourceToLoad, imageView.Context);
			}

			if (animation == null)
			{
				var imageViewHandler = Registrar.Registered.GetHandlerForObject<IImageViewHandler>(imageSourceToLoad);
				if (imageViewHandler != null)
				{
					await imageViewHandler.LoadImageAsync(imageSourceToLoad, imageView);
				}
				else
				{
					using (var drawable = await imageView.Context.GetFormsDrawableAsync(imageSourceToLoad))
					{
						// only set the image if we are still on the same one
						if (imageView.ShouldStillSetImage(newView, imageSourceToCheckIfUserChangedImageSource))
						{
							if (drawable != null)
								imageView.SetImageDrawable(drawable);

							return false;
						}
					}
				}
			}
			else
			{
				if (imageView.ShouldStillSetImage(newView, imageSourceToCheckIfUserChangedImageSource))
					imageView.SetImageDrawable(animation.ImageDrawable);
				else
				{
					animation?.Reset();
					animation?.Dispose();
				}
			}

			return imageView.Drawable != null;
		}

		static bool ShouldStillSetImage(this AImageView imageView, IImageElement newView, ImageSource newImageSource) =>
			!imageView.IsDisposed() && SourceIsNotChanged(newView, newImageSource);

		static bool SourceIsNotChanged(IImageElement imageElement, ImageSource imageSource) =>
			imageElement == null || imageElement.Source == imageSource;

		static async Task SetImagePlaceholder(this AImageView imageView, ImageSource placeholder)
		{
			using (var drawable = await imageView.Context.GetFormsDrawableAsync(placeholder))
			{
				// only set the image if we are still on the same one
				if (!imageView.IsDisposed())
					imageView.SetImageDrawable(drawable);
			}
		}

		internal static void Reset(this IFormsAnimationDrawable formsAnimation)
		{
			if (formsAnimation is FormsAnimationDrawable animation)
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

		internal static async void SetImage(this AImageView image, ImageSource source, Context context)
		{
			image.SetImageDrawable(await context.GetFormsDrawableAsync(source));
		}
	}
}
