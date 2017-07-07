using System;
using System.ComponentModel;
using AppKit;
using CoreAnimation;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	public class ImageRenderer : VisualElementRenderer<Image>
	{
		bool _isDisposed;

		bool _isOpaque;

		public override bool IsOpaque => _isOpaque;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				DisposeImage();
			}

			_isDisposed = true;

			base.Dispose(disposing);
		}

		void DisposeImage()
		{
			var cgimage = Layer.Contents as CGImage;
			if (cgimage != null)
			{
				cgimage.Dispose();
				Layer.Contents = null;
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			if (e.NewElement != null)
			{
				WantsLayer = true;

				SetAspect();
				SetImage(e.OldElement);
				SetOpacity();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == Image.SourceProperty.PropertyName)
				SetImage();
			else if (e.PropertyName == Image.IsOpaqueProperty.PropertyName)
				SetOpacity();
			else if (e.PropertyName == Image.AspectProperty.PropertyName)
				SetAspect();
		}

		void SetAspect()
		{
			switch (Element.Aspect)
			{
				case Aspect.AspectFill:
					Layer.ContentsGravity = CALayer.GravityResizeAspectFill;
					break;
				case Aspect.Fill:
					Layer.ContentsGravity = CALayer.GravityResize;
					break;
				case Aspect.AspectFit:
				default:
					Layer.ContentsGravity = CALayer.GravityResizeAspect;
					break;
			}
		}

		async void SetImage(Image oldElement = null)
		{
			var source = Element.Source;

			if (oldElement != null)
			{
				var oldSource = oldElement.Source;
				if (Equals(oldSource, source))
					return;

				var imageSource = oldSource as FileImageSource;
				if (imageSource != null && source is FileImageSource && imageSource.File == ((FileImageSource)source).File)
					return;

				DisposeImage();
			}

			IImageSourceHandler handler;

			Element.SetIsLoading(true);

			if (source != null && (handler = Internals.Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
			{
				NSImage nsImage;
				try
				{
					nsImage = await handler.LoadImageAsync(source, scale: (float)NSScreen.MainScreen.BackingScaleFactor);
				}
				catch (OperationCanceledException)
				{
					nsImage = null;
				}

				DisposeImage();
				if (nsImage != null)
				{
					Layer.Contents = nsImage.CGImage;
					nsImage.Dispose();
				}

				if (!_isDisposed)
					((IVisualElementController)Element).NativeSizeChanged();
			}
			else
				DisposeImage();

			if (!_isDisposed)
				Element.SetIsLoading(false);
		}

		void SetOpacity()
		{
			_isOpaque = (Element.IsOpaque);
		}
	}
}