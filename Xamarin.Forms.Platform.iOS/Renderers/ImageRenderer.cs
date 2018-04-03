using System;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using ImageIO;
using CoreGraphics;
using CoreAnimation;
using Xamarin.Forms.Internals;
using RectangleF = CoreGraphics.CGRect;

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

	public class FormsCAKeyFrameAnimation : CAKeyFrameAnimation
	{
		public int Width { get; set; }

		public int Height { get; set; }
	}

	public class FormsUIImageView : UIImageView
	{
		const string AnimationLayerName = "FormsUIImageViewAnimation";
		FormsCAKeyFrameAnimation _animation;
		bool _autoPlay;

		public FormsUIImageView(CGRect frame) : base(frame)
		{
			;
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			if (Image == null && Animation != null)
			{
				return new CoreGraphics.CGSize(Animation.Width, Animation.Height);
			}

			return base.SizeThatFits(size);
		}

		public bool AutoPlay {
			get { return _autoPlay; }
			set
			{
				_autoPlay = value;
				if (_animation != null)
				{
					Layer.Speed = _autoPlay ? 1.0f : 0.0f;
				}
			}
		}

		public FormsCAKeyFrameAnimation Animation
		{
			get { return _animation; }
			set
			{
				if (_animation != null)
				{
					Layer.RemoveAnimation(AnimationLayerName);
					_animation.Dispose();
				}

				_animation = value;
				if (_animation != null)
				{
					Layer.AddAnimation(_animation, AnimationLayerName);
					Layer.Speed = AutoPlay ? 1.0f : 0.0f;
				}
			}
		}

		public override bool IsAnimating
		{
			get
			{
				if (_animation != null)
					return Layer.Speed != 0.0f;
				else
					return base.IsAnimating;
			}
		}

		public override void StartAnimating()
		{
			if (_animation != null && Layer.Speed == 0.0f)
			{
				Layer.RemoveAnimation(AnimationLayerName);
				Layer.AddAnimation(_animation, AnimationLayerName);
				Layer.Speed = 1.0f;
			}
			else
			{
				base.StartAnimating();
			}
		}

		public override void StopAnimating()
		{
			if (_animation != null && Layer.Speed != 0.0f)
			{
				Layer.RemoveAnimation(AnimationLayerName);
				Layer.AddAnimation(_animation, AnimationLayerName);
				Layer.Speed = 0.0f;
			}
			else
			{
				base.StopAnimating();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _animation != null)
			{
				Layer.RemoveAnimation(AnimationLayerName);
				_animation.Dispose();
				_animation = null;
			}

			base.Dispose(disposing);
		}
	}

	public class ImageRenderer : ViewRenderer<Image, FormsUIImageView>, IImageVisualElementRenderer
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
				if (Control != null && Control.Animation != null)
				{
					Control.Animation.AnimationStopped -= OnAnimationStopped;
					Control.Animation.Dispose();
					Control.Animation = null;
				}

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
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var imageView = new FormsUIImageView(RectangleF.Empty);
					imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					imageView.ClipsToBounds = true;
					SetNativeControl(imageView);
				}

				await TrySetImage(e.OldElement as Image);
			}

			base.OnElementChanged(e);
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TrySetImage().ConfigureAwait(false);
            else if (e.PropertyName == Image.IsOpaqueProperty.PropertyName)
				SetOpacity();
			else if (e.PropertyName == Image.AspectProperty.PropertyName)
				SetAspect();
			else if (e.PropertyName == Image.IsAnimationPlayingProperty.PropertyName)
				StartStopAnimation();
		}

		void SetAspect()
		{
			if (_isDisposed || Element == null || Control == null)
			{
				return;
			}

			Control.ContentMode = Element.Aspect.ToUIViewContentMode();
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

            //TODO GIF

            if (_isDisposed || Element == null || Control == null)
            {
                return;
            }

            var source = Element.Source;

            if (oldElement != null)
            {
                var oldSource = oldElement.Source;
                if (Equals(oldSource, source))
                    return;

                if (oldSource is FileImageSource && source is FileImageSource && ((FileImageSource)oldSource).File == ((FileImageSource)source).File)
                    return;

                Control.Image = null;
                Control.Animation = null;
            }

            IImageSourceHandlerEx handler;

            Element.SetIsLoading(true);

            if (source != null &&
                (handler = Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandlerEx>(source)) != null)
            {
                UIImage uiimage = null;
                FormsCAKeyFrameAnimation animation = null;
                try
                {
                    if (!Element.IsSet(Image.AnimationPlayBehaviorProperty) && !Element.IsSet(Image.IsAnimationPlayingProperty))
                        uiimage = await handler.LoadImageAsync(source, scale: (float)UIScreen.MainScreen.Scale);
                    else
                        animation = await handler.LoadImageAnimationAsync(source, scale: (float)UIScreen.MainScreen.Scale);
                }
                catch (OperationCanceledException)
                {
                    uiimage = null;
                    animation = null;
                }

                if (_isDisposed)
                {
                    uiimage?.Dispose();
                    uiimage = null;
                    animation?.Dispose();
                    animation = null;
                    return;
                }

				var imageView = Control;
				if (imageView != null)
				{
					if (uiimage != null)
					{
						imageView.Image = uiimage;
					}
					else if (animation != null)
					{
						imageView.AutoPlay = ((Image.AnimationPlayBehaviorValue)Element.GetValue(Image.AnimationPlayBehaviorProperty) == Image.AnimationPlayBehaviorValue.OnLoad);
						imageView.Animation = animation;

						animation.AnimationStopped += OnAnimationStopped;
					}
				}

                ((IVisualElementController)Element).NativeSizeChanged();
            }
            else
            {
                Control.Image = null;
                Control.Animation = null;
            }

            Element.SetIsLoading(false);
        }

		void OnAnimationStopped(object sender, CAAnimationStateEventArgs e)
		{
			if (Element != null && !_isDisposed && e.Finished)
				Element.OnAnimationFinishedPlaying();
		}

		void SetOpacity()
		{
			if (_isDisposed || Element == null || Control == null)
			{
				return;
			}

		bool IImageVisualElementRenderer.IsDisposed => _isDisposed;

		UIImageView IImageVisualElementRenderer.GetImage() => Control;
	}

				}

	public interface IImageSourceHandler : IRegisterable
	{
		Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1);
	}

	public interface IImageSourceHandlerEx : IImageSourceHandler
	{
		Task<FormsCAKeyFrameAnimation> LoadImageAnimationAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1);
	}

	public sealed class FileImageSourceHandler : IImageSourceHandlerEx
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

		public Task<FormsCAKeyFrameAnimation> LoadImageAnimationAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1)
		{
			FormsCAKeyFrameAnimation animation = ImageAnimationHelper.CreateAnimationFromFileImageSource(imagesource as FileImageSource);
			if (animation == null)
			{
				Log.Warning(nameof(FileImageSourceHandler), "Could not find image: {0}", imagesource);
			}

			return Task.FromResult(animation);
		}
	}

	public sealed class StreamImagesourceHandler : IImageSourceHandlerEx
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

		public async Task<FormsCAKeyFrameAnimation> LoadImageAnimationAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1)
		{
			FormsCAKeyFrameAnimation animation = await ImageAnimationHelper.CreateAnimationFromStreamImageSourceAsync(imagesource as StreamImageSource, cancelationToken);
			if (animation == null)
			{
				Log.Warning(nameof(FileImageSourceHandler), "Could not find image: {0}", imagesource);
			}

			return animation;
		}
	}

	public sealed class ImageLoaderSourceHandler : IImageSourceHandlerEx
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

		public async Task<FormsCAKeyFrameAnimation> LoadImageAnimationAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1)
		{
			FormsCAKeyFrameAnimation animation = await ImageAnimationHelper.CreateAnimationFromUriImageSourceAsync(imagesource as UriImageSource, cancelationToken);
			if (animation == null)
			{
				Log.Warning(nameof(FileImageSourceHandler), "Could not find image: {0}", imagesource);
			}

			return animation;
		}
	}

	public sealed class FontImageSourceHandler : IImageSourceHandler
	{
		//should this be the default color on the BP for iOS? 
		readonly Color _defaultColor = Color.White;

		public Task<UIImage> LoadImageAsync(
			ImageSource imagesource, 
			CancellationToken cancelationToken = default(CancellationToken), 
			float scale = 1f)
		{
			UIImage image = null;
			var fontsource = imagesource as FontImageSource;
			if (fontsource != null)
			{
				var font = UIFont.FromName(fontsource.FontFamily ?? string.Empty, (float)fontsource.Size) ??
					UIFont.SystemFontOfSize((float)fontsource.Size);
				var iconcolor = fontsource.Color.IsDefault ? _defaultColor : fontsource.Color;
				var attString = new NSAttributedString(fontsource.Glyph, font: font, foregroundColor: iconcolor.ToUIColor());
				var imagesize = ((NSString)fontsource.Glyph).GetSizeUsingAttributes(attString.GetUIKitAttributes(0, out _));
				
				UIGraphics.BeginImageContextWithOptions(imagesize, false, 0f);
				var ctx = new NSStringDrawingContext();
				var boundingRect = attString.GetBoundingRect(imagesize, (NSStringDrawingOptions)0, ctx);
				attString.DrawString(new RectangleF(
					imagesize.Width / 2 - boundingRect.Size.Width / 2,
					imagesize.Height / 2 - boundingRect.Size.Height / 2,
					imagesize.Width,
					imagesize.Height));
				image = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();

				if (iconcolor != _defaultColor)
					image = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
			}
			return Task.FromResult(image);

		}
	}
}