using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Android.Widget;
using AScaleType = Android.Widget.ImageView.ScaleType;
using ARect = Android.Graphics.Rect;
using System;
using Xamarin.Forms.Internals;
#if __ANDROID_29__
using AViewCompat = AndroidX.Core.View.ViewCompat;
#else
using AViewCompat = Android.Support.V4.View.ViewCompat;
#endif

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class ImageElementManager : IDisposable
	{
		readonly IVisualElementRenderer renderer;
		CancellationTokenSource _currentImageLoadCancellationSource;
		bool _disposed;

		public ImageElementManager(IVisualElementRenderer renderer)
		{
			this.renderer = renderer;

			renderer.ElementPropertyChanged += OnElementPropertyChanged;
			renderer.ElementChanged += OnElementChanged;

			if (renderer is ILayoutChanges layoutChanges)
				layoutChanges.LayoutChange += OnLayoutChange;
		}

		static void OnLayoutChange(object sender, global::Android.Views.View.LayoutChangeEventArgs e)
		{
			if (sender is IVisualElementRenderer renderer && renderer.View is ImageView imageView)
				AViewCompat.SetClipBounds(imageView, imageView.GetScaleType() == AScaleType.CenterCrop ? new ARect(0, 0, e.Right - e.Left, e.Bottom - e.Top) : null);
		}

		void Unregister()
		{
			renderer.ElementPropertyChanged -= OnElementPropertyChanged;
			renderer.ElementChanged -= OnElementChanged;
			if (renderer is ILayoutChanges layoutChanges)
				layoutChanges.LayoutChange -= OnLayoutChange;

			if (renderer is IImageRendererController imageRenderer)
				imageRenderer.SetFormsAnimationDrawable(null);

			if (renderer.View is ImageView imageView)
			{
				imageView.SetImageDrawable(null);
				imageView.Reset();
			}
		}

		async void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			var view = renderer.View as ImageView;
			var newImageElementManager = e.NewElement as IImageElement;
			var oldImageElementManager = e.OldElement as IImageElement;
			var rendererController = renderer as IImageRendererController;

			if (rendererController.IsDisposed)
				return;

			await TryUpdateBitmap(rendererController, view, newImageElementManager, oldImageElementManager);

			if (rendererController.IsDisposed)
				return;

			UpdateAspect(rendererController, view, newImageElementManager, oldImageElementManager);

			if (rendererController.IsDisposed)
				return;

			ElevationHelper.SetElevation(view, renderer.Element);
		}

		async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var ImageElementManager = (IImageElement)renderer.Element;
			var imageController = (IImageController)renderer.Element;

			if (renderer?.View?.LayoutParameters == null &&(renderer is ILayoutChanges lc && lc.HasLayoutOccurred))
			{
				return;
			}

			if (e.IsOneOf(Image.SourceProperty, Button.ImageSourceProperty))
			{
				await TryUpdateBitmap(renderer as IImageRendererController, (ImageView)renderer.View, (IImageElement)renderer.Element).ConfigureAwait(false);

			}
			else if (e.Is(Image.AspectProperty))
			{
				UpdateAspect(renderer as IImageRendererController, (ImageView)renderer.View, (IImageElement)renderer.Element);
			}
			else if (e.Is(Image.IsAnimationPlayingProperty))
				await StartStopAnimation(renderer, imageController, ImageElementManager).ConfigureAwait(false);
		}

		async Task StartStopAnimation(
			IVisualElementRenderer renderer,
			IImageController imageController,
			IImageElement imageElement)
		{
			IImageRendererController imageRendererController = renderer as IImageRendererController;
			var view = renderer.View as ImageView;
			if (imageRendererController.IsDisposed || imageElement == null || view == null || view.IsDisposed())
				return;

			if (imageElement.IsLoading)
				return;

			if (!(view.Drawable is FormsAnimationDrawable) && imageElement.IsAnimationPlaying)
				await TryUpdateBitmap(imageRendererController, view, imageElement);

			if (view.Drawable is FormsAnimationDrawable animation)
			{
				if (imageElement.IsAnimationPlaying && !animation.IsRunning)
					animation.Start();
				else if (!imageElement.IsAnimationPlaying && animation.IsRunning)
					animation.Stop();
			}
		}

		async Task TryUpdateBitmap(IImageRendererController rendererController, ImageView Control, IImageElement newImage, IImageElement previous = null)
		{
			if (newImage == null || rendererController.IsDisposed)
			{
				return;
			}

			if (!TryRequestUpdateImageSource(newImage, previous, out var imageLoadCancellation))
				return;

			if (Control.Drawable is FormsAnimationDrawable currentAnimation)
			{
				rendererController.SetFormsAnimationDrawable(currentAnimation);
				currentAnimation.Stop();
			}
			else
			{
				rendererController.SetFormsAnimationDrawable(null);
			}

			try
			{
				await Control.UpdateBitmap(newImage, previous, imageLoadCancellation).ConfigureAwait(false);
			}
			catch (OperationCanceledException) when (imageLoadCancellation.IsCancellationRequested)
			{
				return;
			}
			catch (Exception ex)
			{
				Log.Warning(nameof(ImageElementManager), "Error loading image: {0}", ex);
			}
			finally
			{
				if (!imageLoadCancellation.IsCancellationRequested && newImage is IImageController imageController)
					imageController.SetIsLoading(false);
			}

			if (imageLoadCancellation.IsCancellationRequested || rendererController.IsDisposed)
				return;

			if (Control.Drawable is FormsAnimationDrawable updatedAnimation)
			{
				rendererController.SetFormsAnimationDrawable(updatedAnimation);

				if (newImage.IsAnimationPlaying)
					updatedAnimation.Start();
			}
		}

		bool TryRequestUpdateImageSource(IImageElement newImage, IImageElement previous, out CancellationToken cancellationToken)
		{
			bool sameImageSource = newImage.Source != null && Equals(previous?.Source, newImage.Source);
			bool alreadyLoading = _currentImageLoadCancellationSource != null;

			if (sameImageSource && alreadyLoading)
				return false;

			_currentImageLoadCancellationSource?.Cancel();
			_currentImageLoadCancellationSource = new CancellationTokenSource();
			cancellationToken = _currentImageLoadCancellationSource.Token;
			return true;
		}

		internal static void OnAnimationStopped(IElementController image, FormsAnimationDrawableStateEventArgs e)
		{
			if (image != null && e.Finished)
				image.SetValueFromRenderer(Image.IsAnimationPlayingProperty, false);
		}

		static void UpdateAspect(IImageRendererController rendererController, ImageView Control, IImageElement newImage, IImageElement previous = null)
		{
			if (newImage == null || rendererController.IsDisposed)
			{
				return;
			}

			ImageView.ScaleType type = newImage.Aspect.ToScaleType();
			Control.SetScaleType(type);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					Unregister();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}