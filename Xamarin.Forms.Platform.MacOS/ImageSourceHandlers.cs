using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using CoreText;
using Foundation;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.MacOS
{
	public interface IImageSourceHandler : IRegisterable
	{
		Task<NSImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken),
			float scale = 1);
	}

	public sealed class FileImageSourceHandler : IImageSourceHandler
	{
		public Task<NSImage> LoadImageAsync(ImageSource imagesource,
			CancellationToken cancelationToken = default(CancellationToken), float scale = 1f)
		{
			NSImage image = null;
			var filesource = imagesource as FileImageSource;
			var file = filesource?.File;
			if (!string.IsNullOrEmpty(file))
				image = File.Exists(file) ? new NSImage(file) : NSImage.ImageNamed(file);

			if (image == null)
			{
				Log.Warning(nameof(FileImageSourceHandler), "Could not find image: {0}", imagesource);
			}

			return Task.FromResult(image);
		}
	}

	public sealed class StreamImagesourceHandler : IImageSourceHandler
	{
		public async Task<NSImage> LoadImageAsync(ImageSource imagesource,
			CancellationToken cancelationToken = default(CancellationToken), float scale = 1f)
		{
			NSImage image = null;
			var streamsource = imagesource as StreamImageSource;
			if (streamsource?.Stream == null) return null;
			using (
				var streamImage = await ((IStreamImageSource)streamsource).GetStreamAsync(cancelationToken).ConfigureAwait(false))
			{
				if (streamImage != null)
					image = NSImage.FromStream(streamImage);
			}
			return image;
		}
	}

	public sealed class ImageLoaderSourceHandler : IImageSourceHandler
	{
		public async Task<NSImage> LoadImageAsync(ImageSource imagesource,
			CancellationToken cancelationToken = default(CancellationToken), float scale = 1f)
		{
			NSImage image = null;
			var imageLoader = imagesource as UriImageSource;
			if (imageLoader != null && imageLoader.Uri != null)
			{
				using (var streamImage = await imageLoader.GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					if (streamImage != null)
						image = NSImage.FromStream(streamImage);
				}
			}
			return image;
		}
	}

	public sealed class FontImageSourceHandler : IImageSourceHandler
	{
		//should this be the default color on the BP for iOS? 
		readonly Color _defaultColor = Color.White;

		public FontImageSourceHandler()
		{
		}

		private class LayerProperties
		{
			public Color IconColor { get; set; }
			public NSAttributedString AttString { get; set; }
			public CGSize ImageSize { get; set; }
		}

		public Task<NSImage> LoadImageAsync(
			ImageSource imageSource,
			CancellationToken cancelationToken = default(CancellationToken),
			float scale = 1f)
		{
			NSImage image;
			var layerPropertiesList = new List<LayerProperties>();
			var maxImageSize = CGSize.Empty;

			if (imageSource is LayeredFontImageSource layeredFontImageSource)
			{
				var baseSize = (float)layeredFontImageSource.Size;
				var baseFont = NSFont.FromFontName(layeredFontImageSource.FontFamily ?? string.Empty, baseSize) ?? NSFont.SystemFontOfSize(baseSize);
				var baseIconColor = layeredFontImageSource.Color.IsDefault ? _defaultColor : layeredFontImageSource.Color;
				var baseAttString = layeredFontImageSource.Glyph == null ? null : new NSAttributedString(layeredFontImageSource.Glyph, font: baseFont, foregroundColor: baseIconColor.ToNSColor());
				maxImageSize = layeredFontImageSource.Glyph == null ? CGSize.Empty : ((NSString)layeredFontImageSource.Glyph).StringSize(baseAttString.GetAppKitAttributes(0, out _));

				foreach (var layer in layeredFontImageSource.Layers)
				{
					var size = layer.IsSet(FontImageSource.SizeProperty) ? (float)layer.Size : baseSize;
					var font = layer.FontFamily == null ? baseFont : NSFont.FromFontName(layer.FontFamily ?? string.Empty, size) ??
						NSFont.SystemFontOfSize(size);
					var iconcolor = layer.Color.IsDefault ? baseIconColor : layer.Color;
					var attString = layer.Glyph == null ? baseAttString : new NSAttributedString(layer.Glyph, font: font, foregroundColor: iconcolor.ToNSColor());
					var imagesize = ((NSString)layer.Glyph).StringSize(attString.GetAppKitAttributes(0, out _));

					layerPropertiesList.Add(new LayerProperties { IconColor = iconcolor, AttString = attString, ImageSize = imagesize });

					if (imagesize.Width > maxImageSize.Width)
					{
						maxImageSize.Width = imagesize.Width;
					}
					if (imagesize.Height > maxImageSize.Height)
					{
						maxImageSize.Height = imagesize.Height;
					}
				}
			}
			else if (imageSource is FontImageSource fontSource)
			{
				var size = (float)fontSource.Size;
				var font = NSFont.FromFontName(fontSource.FontFamily ?? string.Empty, size) ?? NSFont.SystemFontOfSize(size);
				var iconColor = fontSource.Color.IsDefault ? _defaultColor : fontSource.Color;
				var attString = new NSAttributedString(fontSource.Glyph, font: font, foregroundColor: iconColor.ToNSColor());
				var imageSize = ((NSString)fontSource.Glyph).StringSize(attString.GetAppKitAttributes(0, out _));

				layerPropertiesList.Add(new LayerProperties { IconColor = iconColor, AttString = attString, ImageSize = imageSize });

				maxImageSize = imageSize;
			}

			var screenScale = NSScreen.MainScreen.BackingScaleFactor;
			var width = (nint)(maxImageSize.Width * screenScale);
			var height = (nint)(maxImageSize.Height * screenScale);

			using (var context = new CGBitmapContext(IntPtr.Zero, width, height, 8, width * 4, NSColorSpace.GenericRGBColorSpace.ColorSpace, CGImageAlphaInfo.PremultipliedFirst))
			{
				context.ScaleCTM(screenScale, screenScale);
				foreach (var layerProperties in layerPropertiesList)
				{
					var attString = layerProperties.AttString;
					var imageSize = layerProperties.ImageSize;

					using (var ctline = new CTLine(attString))
					{
						context.TextPosition = new CGPoint((maxImageSize.Width - imageSize.Width) / 2, (maxImageSize.Height - imageSize.Height) / 2);
						ctline.Draw(context);
					}
				}
				using (var cgImage = context.ToImage())
				{
					image = new NSImage(cgImage, maxImageSize);
				}
			}

			return Task.FromResult(image);
		}
	}
}