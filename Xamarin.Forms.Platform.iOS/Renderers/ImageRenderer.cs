using System;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Xamarin.Forms.Internals;
using RectangleF = CoreGraphics.CGRect;
using System.Linq;

namespace Xamarin.Forms.Platform.iOS
{
	public static class ImageExtensions
	{
		public static UIViewContentMode ToUIViewContentMode(this Aspect aspect)
		{
			switch (aspect)
			{
				case Aspect.AspectFill:
					return UIViewContentMode.ScaleAspectFill;
				case Aspect.Fill:
					return UIViewContentMode.ScaleToFill;
				case Aspect.AspectFit:
				default:
					return UIViewContentMode.ScaleAspectFit;
			}
		}
	}

	public class ImageRenderer : ViewRenderer<Image, UIImageView>, IImageVisualElementRenderer
	{
		bool _isDisposed;

		public ImageRenderer() : base()
		{
			ImageElementManager.Init(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				UIImage oldUIImage;
				if (Control != null && (oldUIImage = Control.Image) != null)
				{
					ImageElementManager.Dispose(this);
					oldUIImage.Dispose();
				}
			}

			_isDisposed = true;

			base.Dispose(disposing);
		}

		protected override async void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			if (Control == null)
			{
				var imageView = new UIImageView(RectangleF.Empty);
				imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				imageView.ClipsToBounds = true;
				SetNativeControl(imageView);
			}

			if (e.NewElement != null)
			{
				await TrySetImage(e.OldElement as Image);
			}

			base.OnElementChanged(e);
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TrySetImage().ConfigureAwait(false);
		}

		protected virtual async Task TrySetImage(Image previous = null)
		{
			// By default we'll just catch and log any exceptions thrown by SetImage so they don't bring down
			// the application; a custom renderer can override this method and handle exceptions from
			// SetImage differently if it wants to

			try
			{
				await SetImage(previous).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
			}
			finally
			{
				((IImageController)Element)?.SetIsLoading(false);
			}
		}

		protected async Task SetImage(Image oldElement = null)
		{
			await ImageElementManager.SetImage(this, Element, oldElement).ConfigureAwait(false);
		}

		void IImageVisualElementRenderer.SetImage(UIImage image) => Control.Image = image;

		bool IImageVisualElementRenderer.IsDisposed => _isDisposed;

		UIImageView IImageVisualElementRenderer.GetImage() => Control;
	}


	public interface IImageSourceHandler : IRegisterable
	{
		Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1);
	}

	public sealed class FileImageSourceHandler : IImageSourceHandler
	{
		public Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1f)
		{
			UIImage image = null;
			var filesource = imagesource as FileImageSource;
			var file = filesource?.File;
			if (!string.IsNullOrEmpty(file))
				image = File.Exists(file) ? new UIImage(file) : UIImage.FromBundle(file);

			if (image == null)
			{
				Log.Warning(nameof(FileImageSourceHandler), "Could not find image: {0}", imagesource);
			}

			return Task.FromResult(image);
		}
	}

	public sealed class StreamImagesourceHandler : IImageSourceHandler
	{
		public async Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1f)
		{
			UIImage image = null;
			var streamsource = imagesource as StreamImageSource;
			if (streamsource?.Stream != null)
			{
				using (var streamImage = await ((IStreamImageSource)streamsource).GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					if (streamImage != null)
						image = UIImage.LoadFromData(NSData.FromStream(streamImage), scale);
				}
			}

			if (image == null)
			{
				Log.Warning(nameof(StreamImagesourceHandler), "Could not load image: {0}", streamsource);
			}

			return image;
		}
	}

	public sealed class ImageLoaderSourceHandler : IImageSourceHandler
	{
		public async Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1f)
		{
			UIImage image = null;
			var imageLoader = imagesource as UriImageSource;
			if (imageLoader?.Uri != null)
			{
				using (var streamImage = await imageLoader.GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					if (streamImage != null)
						image = UIImage.LoadFromData(NSData.FromStream(streamImage), scale);
				}
			}

			if (image == null)
			{
				Log.Warning(nameof(ImageLoaderSourceHandler), "Could not load image: {0}", imageLoader);
			}

			return image;
		}
	}

	public sealed class FontImageSourceHandler : IImageSourceHandler
	{
		public Task<UIImage> LoadImageAsync(
			ImageSource imagesource, 
			CancellationToken cancelationToken = default(CancellationToken), 
			float scale = 1f)
		{
			UIImage image = null;
			var fontsource = imagesource as FontImageSource;
			if (fontsource != null)
			{
				var iconcolor = fontsource.Color != Color.Default ? fontsource.Color : Color.White;
				var imagesize = new SizeF((float)fontsource.Size, (float)fontsource.Size);
				var font = UIFont.FromName(fontsource.FontFamily ?? string.Empty, (float)fontsource.Size) ??
					UIFont.SystemFontOfSize((float)fontsource.Size);

				UIGraphics.BeginImageContextWithOptions(imagesize, false, 0f);
				var attString = new NSAttributedString(fontsource.Glyph, font: font, foregroundColor: iconcolor.ToUIColor());
				var ctx = new NSStringDrawingContext();
				var boundingRect = attString.GetBoundingRect(imagesize, (NSStringDrawingOptions)0, ctx);
				attString.DrawString(new RectangleF(
					imagesize.Width / 2 - boundingRect.Size.Width / 2,
					imagesize.Height / 2 - boundingRect.Size.Height / 2,
					imagesize.Width,
					imagesize.Height));
				image = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();

				if (iconcolor != Color.Default)
					image = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
			}
			return Task.FromResult(image);

		}
	}
}