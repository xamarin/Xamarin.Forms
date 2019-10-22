using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using AImageView = Android.Widget.ImageView;

namespace Xamarin.Forms.Platform.Android
{
	internal static class ImageViewExtensions
	{
		public static Task UpdateBitmap(this AImageView imageView, IImageElement newView, IImageElement previousView, ImageSource placeholder) =>
			imageView.UpdateBitmap(newView, previousView, null, null, placeholder);

		public static Task UpdateBitmap(this AImageView imageView, ImageSource newImageSource, ImageSource previousImageSourc) =>
			imageView.UpdateBitmap(null, null, newImageSource, previousImageSourc, null);

		public static Task UpdateBitmap(this AImageView imageView, ImageSource newImageSource, ImageSource previousImageSourc, ImageSource placeholder) =>
			imageView.UpdateBitmap(null, null, newImageSource, previousImageSourc, placeholder);

		static async Task UpdateBitmap(
			this AImageView imageView,
			IImageElement newView,
			IImageElement previousView,
			ImageSource newImageSource,
			ImageSource previousImageSource,
			ImageSource placeholder)
		{

			IImageController imageController = newView as IImageController;
			newImageSource = newImageSource ?? newView?.Source;
			previousImageSource = previousImageSource ?? previousView?.Source;
			if(placeholder == null)
			{
				placeholder = (newView is Image image) ? image.Placeholder : null;
			}

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
						await imageViewHandler.LoadImageAsync(newImageSource, placeholder, imageView);
					}
					else
					{
						using (var drawable = await imageView.Context.GetFormsDrawableAsync(newImageSource))
						{
							// only set the image if we are still on the same one
							if (!imageView.IsDisposed() && SourceIsNotChanged(newView, newImageSource) && drawable != null)
								imageView.SetImageDrawable(drawable);
							else if(placeholder != null)
								await SetImagePlaceholder(imageView, placeholder);
						}
					}
				}
				else if (placeholder != null)
				{
					await SetImagePlaceholder(imageView, placeholder);
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


			bool SourceIsNotChanged(IImageElement imageElement, ImageSource imageSource)
			{
				return (imageElement != null) ? imageElement.Source == imageSource : true;
			}
		}

		static async Task SetImagePlaceholder(AImageView imageView, ImageSource placeholder)
		{
			using (var drawable = await imageView.Context.GetFormsDrawableAsync(placeholder))
			{
				// only set the image if we are still on the same one
				if (!imageView.IsDisposed())
					imageView.SetImageDrawable(drawable);
			}
		}

		internal static async void SetImage(this AImageView image, ImageSource source, Context context)
		{
			image.SetImageDrawable(await context.GetFormsDrawableAsync(source));
		}
	}
}
