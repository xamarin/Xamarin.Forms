using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Specifics = Xamarin.Forms.PlatformConfiguration.WindowsSpecific.Image;
using SpecificsPage = Xamarin.Forms.PlatformConfiguration.WindowsSpecific.Page;

namespace Xamarin.Forms.Platform.UWP
{
	public class ImageRenderer : ViewRenderer<Image, Windows.UI.Xaml.Controls.Image>, IImageVisualElementRenderer
	{
		bool _measured;
		bool _disposed;
		ImageSource _sourceWithAssetPath;

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

				if (!await UpdateImageDirectory())
					await TryUpdateSource().ConfigureAwait(false);
			}
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TryUpdateSource();
			else if (e.PropertyName == Specifics.ImageDirectoryProperty.PropertyName)
				await UpdateImageDirectory();
		}

		/// <returns>Source updated</returns>
		async Task<bool> UpdateImageDirectory()
		{
			if (Element.IsSet(Specifics.ImageDirectoryProperty))
				return await AddPathToSource(Element.OnThisPlatform().GetImageDirectory());

			// Check parent page
			var parentPage = GetParentPage();
			if (parentPage?.IsSet(SpecificsPage.ImageDirectoryProperty) == true)
			{
				Element.OnThisPlatform().SetImageDirectory(parentPage.OnThisPlatform().GetImageDirectory());
				return true;
			}

			// Reset
			var oldPath = _sourceWithAssetPath;
			_sourceWithAssetPath = null;
			if (oldPath != null)
			{
				await TryUpdateSource();
				return true;
			}

			return false;
		}

		async Task<bool> AddPathToSource(string newPath)
		{
			if (!(Element.Source is FileImageSource fileSource))
				return false;

			string fullPath = $"{newPath}{System.IO.Path.DirectorySeparatorChar}{fileSource.File}";
			if ((_sourceWithAssetPath as FileImageSource)?.File == fullPath)
				return false;

			_sourceWithAssetPath = string.IsNullOrEmpty(newPath)
				? null
				: ImageSource.FromFile(fullPath);

			await TryUpdateSource();
			return true;
		}

		Page GetParentPage()
		{
			var parentPage = Element.Parent;
			while (parentPage != null && !(parentPage is Page))
				parentPage = parentPage.Parent;
			return parentPage as Page;
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
			await ImageElementManager.UpdateSource(this, _sourceWithAssetPath).ConfigureAwait(false);
		}

		void IImageVisualElementRenderer.SetImage(Windows.UI.Xaml.Media.ImageSource image)
		{
			Control.Source = image;
		}

		Windows.UI.Xaml.Controls.Image IImageVisualElementRenderer.GetImage() => Control;
	}
}
