﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.UWP
{
	public class ImageRenderer : ViewRenderer<Image, Windows.UI.Xaml.Controls.Image>, IImageVisualElementRenderer
	{
		bool _measured;
		bool _disposed;

		public ImageRenderer() : base()
		{
			ImageElementManager.Init(this);
		}

		bool IImageVisualElementRenderer.IsDisposed => _disposed;

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (Control.Source == null)
				return new SizeRequest();

			_measured = true;

			if (Control.Source is BitmapSource bitmap)
			{
				return new SizeRequest(
					new Size
					{
						Width = bitmap.PixelWidth,
						Height = bitmap.PixelHeight
					});
			}
			else if (Control.Source is CanvasImageSource canvas)
			{
				return new SizeRequest(
					new Size
					{
						Width = canvas.SizeInPixels.Width,
						Height = canvas.SizeInPixels.Height
					});
			}
			else
			{
				throw new InvalidCastException($"\"{Control.Source.GetType().FullName}\" is not supported.");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				ImageElementManager.Dispose(this);
				if (Control != null)
				{
					Control.ImageOpened -= OnImageOpened;
					Control.ImageFailed -= OnImageFailed;
				}
			}

			base.Dispose(disposing);
		}

		protected override async void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var image = new Windows.UI.Xaml.Controls.Image();
					image.ImageOpened += OnImageOpened;
					image.ImageFailed += OnImageFailed;
					SetNativeControl(image);
				}

				await TryUpdateSource().ConfigureAwait(false);
			}
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TryUpdateSource().ConfigureAwait(false);
		}


		void OnImageOpened(object sender, RoutedEventArgs routedEventArgs)
		{
			if (_measured)
			{
				ImageElementManager.RefreshImage(Element);
			}

			Element?.SetIsLoading(false);
		}

		protected virtual void OnImageFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
		{
			Log.Warning("Image Loading", $"Image failed to load: {exceptionRoutedEventArgs.ErrorMessage}");
			Element?.SetIsLoading(false);
		}


		protected virtual async Task TryUpdateSource()
		{
			// By default we'll just catch and log any exceptions thrown by UpdateSource so we don't bring down
			// the application; a custom renderer can override this method and handle exceptions from
			// UpdateSource differently if it wants to

			try
			{
				await UpdateSource().ConfigureAwait(false);
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

		protected async Task UpdateSource()
		{
			await ImageElementManager.UpdateSource(this).ConfigureAwait(false);		}

		void IImageVisualElementRenderer.SetImage(Windows.UI.Xaml.Media.ImageSource image)
		{
			Control.Source = image;
		}

		Windows.UI.Xaml.Controls.Image IImageVisualElementRenderer.GetImage() => Control;
	}
}
