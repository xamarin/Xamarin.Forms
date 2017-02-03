using System;
using System.ComponentModel;
using AppKit;
using CoreAnimation;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	public class FormsNSImageView : NSView
	{
		bool _isOpaque;

		public FormsNSImageView()
		{
			Layer = new CALayer();
			WantsLayer = true;
		}

		public void SetIsOpaque(bool isOpaque)
		{
			_isOpaque = isOpaque;
		}

		public override bool IsOpaque => _isOpaque;
	}


	public class ImageRenderer : ViewRenderer<Image, FormsNSImageView>
	{
		bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				CGImage oldUIImage;
				if (Control != null && (oldUIImage = Control.Layer.Contents) != null)
				{
					oldUIImage.Dispose();
				}
			}

			_isDisposed = true;

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			if (Control == null)
			{
				var imageView = new FormsNSImageView();
				SetNativeControl(imageView);
			}

			if (e.NewElement != null)
			{
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
					Control.Layer.ContentsGravity = CALayer.GravityResizeAspectFill;
					break;
				case Aspect.Fill:
					Control.Layer.ContentsGravity = CALayer.GravityResize;
					break;
				case Aspect.AspectFit:
				default:
					Control.Layer.ContentsGravity = CALayer.GravityResizeAspect;
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

				Control.Layer.Contents = null;
			}

			IImageSourceHandler handler;

			((IImageController)Element).SetIsLoading(true);

			if (source != null && (handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
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

				var imageView = Control;
				if (imageView != null)
					imageView.Layer.Contents = nsImage != null ? nsImage.CGImage : null;
				if (nsImage != null)
					nsImage.Dispose();

				if (!_isDisposed)
					((IVisualElementController)Element).NativeSizeChanged();
			}
			else
				Control.Layer.Contents = null;

			if (!_isDisposed)
				((IImageController)Element).SetIsLoading(false);
		}

		void SetOpacity()
		{
			(Control as FormsNSImageView)?.SetIsOpaque(Element.IsOpaque);
		}
	}
}