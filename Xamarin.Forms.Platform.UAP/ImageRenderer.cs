using System;
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

		static bool _nativeAnimationSupport = false;

		static ImageRenderer()
		{
			if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Media.Imaging.BitmapImage", "AutoPlay"))
				if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Media.Imaging.BitmapImage", "IsPlaying"))
					if (Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.UI.Xaml.Media.Imaging.BitmapImage", "Play"))
						if (Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.UI.Xaml.Media.Imaging.BitmapImage", "Stop"))
							_nativeAnimationSupport = true;
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (Control.Source == null)
				return new SizeRequest();

			_measured = true;

			return new SizeRequest(Control.Source.GetImageSourceSize());
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
			else if (e.PropertyName == Image.AspectProperty.PropertyName)
				UpdateAspect();
			else if (e.PropertyName == Image.IsAnimationPlayingProperty.PropertyName)
				StartStopAnimation();
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
			await ImageElementManager.UpdateSource(this).ConfigureAwait(false);
            // TODO GIF

			Element.SetIsLoading(true);

			ImageSource source = Element.Source;
			IImageSourceHandler handler;
			if (source != null && (handler = Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source)) != null)
			{
				Windows.UI.Xaml.Media.ImageSource imagesource;

				try
				{
					imagesource = await handler.LoadImageAsync(source);
				}
				catch (OperationCanceledException)
				{
					imagesource = null;
				}

				// In the time it takes to await the imagesource, some zippy little app
				// might have disposed of this Image already.
				if (Control != null)
				{
					if (imagesource is BitmapImage bitmapImage)
					{
						if (_nativeAnimationSupport)
						{
							bitmapImage.AutoPlay = false;
							if (Element.IsSet(Image.IsAnimationAutoPlayProperty) || Element.IsSet(Image.IsAnimationPlayingProperty))
								bitmapImage.AutoPlay = (bool)Element.GetValue(Image.IsAnimationAutoPlayProperty);

							if (bitmapImage.IsPlaying && !bitmapImage.AutoPlay)
								bitmapImage.Stop();
						}
					}

					Control.Source = imagesource;
				}

				RefreshImage();
			}
			else
			{
				Control.Source = null;
				Element.SetIsLoading(false);
			}
		}

		void StartStopAnimation()
		{
			if (_disposed || Element == null || Control == null)
			{
				return;
			}

			if (Element.IsLoading)
				return;

			if (Control.Source is BitmapImage bitmapImage)
			{
				if (_nativeAnimationSupport)
				{
					if (Element.IsAnimationPlaying && !bitmapImage.IsPlaying)
						bitmapImage.Play();
					else if (!Element.IsAnimationPlaying && bitmapImage.IsPlaying)
						bitmapImage.Stop();
				}
			}
		}
		Windows.UI.Xaml.Controls.Image IImageVisualElementRenderer.GetImage() => Control;

        void IImageVisualElementRenderer.SetImage(Windows.UI.Xaml.Media.ImageSource image)
        {
            Control.Source = image;
        }
    }
}
