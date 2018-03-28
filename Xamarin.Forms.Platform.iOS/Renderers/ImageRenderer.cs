using System;
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

	public class ImageRenderer : ViewRenderer<Image, FormsUIImageView>
	{
		bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				UIImage oldUIImage;
				if (Control != null && (oldUIImage = Control.Image) != null)
				{
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
				var imageView = new FormsUIImageView(RectangleF.Empty);
				imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				imageView.ClipsToBounds = true;
				SetNativeControl(imageView);
			}

			if (e.NewElement != null)
			{
				SetAspect();
				await TrySetImage(e.OldElement);
				SetOpacity();
			}

			base.OnElementChanged(e);
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TrySetImage();
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

		void SetOpacity()
		{
			if (_isDisposed || Element == null || Control == null)
			{
				return;
			}

			Control.Opaque = Element.IsOpaque;
		}

		void StartStopAnimation()
		{
			if (_isDisposed || Element == null || Control == null || Control.Animation == null)
			{
				return;
			}

			if (Element.IsLoading)
				return;

			if (Element.IsAnimationPlaying && !Control.IsAnimating)
				Control.StartAnimating();
			else if (!Element.IsAnimationPlaying && Control.IsAnimating)
				Control.StopAnimating();
		}
	}

	public class ImageAnimationHelper
	{
		class ImageDataHelper : IDisposable
		{
			NSObject[] _keyFrames = null;
			NSNumber[] _keyTimes = null;
			double[] _delayTimes = null;
			int _imageCount = 0;
			double _totalAnimationTime = 0.0f;
			bool _disposed = false;

			public ImageDataHelper(nint imageCount)
			{
				if (imageCount <= 0)
					throw new ArgumentException();

				_keyFrames = new NSObject[imageCount];
				_keyTimes = new NSNumber[imageCount + 1];
				_delayTimes = new double[imageCount];
				_imageCount = (int)imageCount;
				Width = 0;
				Height = 0;
			}

			public int Width { get; set; }
			public int Height { get; set; }

			public void AddFrameData(int index, CGImageSource imageSource)
			{
				if (index < 0 || index >= _imageCount || index >= imageSource.ImageCount)
					throw new ArgumentException();

				double delayTime = 0.1f;

				var imageProperties = imageSource.GetProperties(index, null);
				using (var gifImageProperties = imageProperties?.Dictionary[ImageIO.CGImageProperties.GIFDictionary])
				using (var unclampedDelayTimeValue = gifImageProperties?.ValueForKey(ImageIO.CGImageProperties.GIFUnclampedDelayTime))
				using (var delayTimeValue = gifImageProperties?.ValueForKey(ImageIO.CGImageProperties.GIFDelayTime))
				{
					if (unclampedDelayTimeValue != null)
						double.TryParse(unclampedDelayTimeValue.ToString(), out delayTime);
					else if (delayTimeValue != null)
						double.TryParse(delayTimeValue.ToString(), out delayTime);

					// Frame delay compability adjustment.
					if (delayTime <= 0.02f)
						delayTime = 0.1f;

					using (var image = imageSource.CreateImage(index, null))
					{
						if (image != null)
						{
							Width = Math.Max(Width, (int)image.Width);
							Height = Math.Max(Height, (int)image.Height);

							_keyFrames[index]?.Dispose();
							_keyFrames[index] = null;

							_keyFrames[index] = NSObject.FromObject(image);
							_delayTimes[index] = delayTime;
							_totalAnimationTime += delayTime;
						}
					}
				}
			}

			public FormsCAKeyFrameAnimation CreateKeyFrameAnimation()
			{
				if (_totalAnimationTime <= 0.0f)
					return null;

				double currentTime = 0.0f;
				for (int i = 0; i < _imageCount; i++)
				{
					_keyTimes[i]?.Dispose();
					_keyTimes[i] = null;

					_keyTimes[i] = new NSNumber(currentTime);
					currentTime += _delayTimes[i] / _totalAnimationTime;
				}

				// When using discrete animation there should be one more keytime
				// than values, with 1.0f as value.
				_keyTimes[_imageCount] = new NSNumber(1.0f);

				return new FormsCAKeyFrameAnimation {
					Values = _keyFrames,
					KeyTimes = _keyTimes,
					Duration = _totalAnimationTime,
					CalculationMode = CAAnimation.AnimationDiscrete
				};
			}

			protected virtual void Dispose(bool disposing)
			{
				if (_disposed)
					return;

				for (int i = 0; i < _imageCount; i++)
				{
					_keyFrames[i]?.Dispose();
					_keyFrames[i] = null;

					_keyTimes[i]?.Dispose();
					_keyTimes[i] = null;
				}

				_keyTimes[_imageCount]?.Dispose();
				_keyTimes[_imageCount] = null;

				_disposed = true;
			}

			public void Dispose()
			{
				Dispose(true);
			}
		}

		static public FormsCAKeyFrameAnimation CreateAnimationFromCGImageSource(CGImageSource imageSource)
		{
			FormsCAKeyFrameAnimation animation = null;
			float repeatCount = float.MaxValue;
			var imageCount = imageSource.ImageCount;

			if (imageCount <= 0)
				return null;

			using (var imageData = new ImageDataHelper(imageCount))
			{
				if (imageSource.TypeIdentifier == "com.compuserve.gif")
				{
					var imageProperties = imageSource.GetProperties(null);
					using (var gifImageProperties = imageProperties?.Dictionary[ImageIO.CGImageProperties.GIFDictionary])
					using (var repeatCountValue = gifImageProperties?.ValueForKey(ImageIO.CGImageProperties.GIFLoopCount))
					{
						if (repeatCountValue != null)
							float.TryParse(repeatCountValue.ToString(), out repeatCount);
						if (repeatCount == 0)
							repeatCount = float.MaxValue;
					}
				}

				for (int i = 0; i < imageCount; i++)
				{
					imageData.AddFrameData(i, imageSource);
				}

				animation = imageData.CreateKeyFrameAnimation();
				if (animation != null)
				{
					animation.RemovedOnCompletion = false;
					animation.KeyPath = "contents";
					animation.RepeatCount = repeatCount;
					animation.Width = imageData.Width;
					animation.Height = imageData.Height;

					if (imageCount == 1)
					{
						animation.Duration = double.MaxValue;
						animation.KeyTimes = null;
					}
				}
			}

			return animation;
		}

		static public async Task<FormsCAKeyFrameAnimation> CreateAnimationFromStreamImageSourceAsync(StreamImageSource imageSource, CancellationToken cancelationToken = default(CancellationToken))
		{
			FormsCAKeyFrameAnimation animation = null;

			if (imageSource?.Stream != null)
			{
				using (var streamImage = await ((IStreamImageSource)imageSource).GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					if (streamImage != null)
					{
						using (var parsedImageSource = CGImageSource.FromData(NSData.FromStream(streamImage)))
						{
							animation = ImageAnimationHelper.CreateAnimationFromCGImageSource(parsedImageSource);
						}
					}
				}
			}

			return animation;
		}

		static public async Task<FormsCAKeyFrameAnimation> CreateAnimationFromUriImageSourceAsync(UriImageSource imageSource, CancellationToken cancelationToken = default(CancellationToken))
		{
			FormsCAKeyFrameAnimation animation = null;

			if (imageSource?.Uri != null)
			{
				using (var streamImage = await imageSource.GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					if (streamImage != null)
					{
						using (var parsedImageSource = CGImageSource.FromData(NSData.FromStream(streamImage)))
						{
							animation = ImageAnimationHelper.CreateAnimationFromCGImageSource(parsedImageSource);
						}
					}
				}
			}

			return animation;
		}

		static public FormsCAKeyFrameAnimation CreateAnimationFromFileImageSource(FileImageSource imageSource)
		{
			FormsCAKeyFrameAnimation animation = null;
			string file = imageSource?.File;
			if (!string.IsNullOrEmpty(file))
			{
				using (var parsedImageSource = CGImageSource.FromUrl(NSUrl.CreateFileUrl(file, null)))
				{
					animation = ImageAnimationHelper.CreateAnimationFromCGImageSource(parsedImageSource);
				}
			}

			return animation;
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
}