using System.ComponentModel;
using System.Threading.Tasks;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public static class ImageElementManager
	{
		public static void Init(IVisualElementRenderer renderer)
		{
			renderer.ElementPropertyChanged += OnElementPropertyChanged;
			renderer.ElementChanged += OnElementChanged;
		}
		public static void Dispose(IVisualElementRenderer renderer)
		{
			renderer.ElementPropertyChanged -= OnElementPropertyChanged;
			renderer.ElementChanged -= OnElementChanged;
		}

		async static void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			var renderer = (sender as IVisualElementRenderer);
			var view = renderer.View as ImageView;
			var newImageElementManager = e.NewElement as IImageController;
			var oldImageElementManager = e.OldElement as IImageController;
			var rendererController = renderer as IImageRendererController;

			await TryUpdateBitmap(rendererController, view, newImageElementManager, oldImageElementManager);
			UpdateAspect(rendererController, view, newImageElementManager, oldImageElementManager);

			if (!rendererController.IsDisposed)
			{
				ElevationHelper.SetElevation(view, renderer.Element);
			}
		}

		async static void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var renderer = (sender as IVisualElementRenderer);
			var ImageElementManager = (IImageController)renderer.Element;
			if (e.PropertyName == ImageElementManager.SourceProperty?.PropertyName)
			{
				await TryUpdateBitmap(renderer as IImageRendererController, (ImageView)renderer.View, (IImageController)renderer.Element).ConfigureAwait(false);
			}
			else if (e.PropertyName == ImageElementManager.AspectProperty?.PropertyName)
			{
				UpdateAspect(renderer as IImageRendererController, (ImageView)renderer.View, (IImageController)renderer.Element);
			}
		}


		async static Task TryUpdateBitmap(IImageRendererController rendererController, ImageView Control, IImageController newImage, IImageController previous = null)
		{
			if (newImage == null || rendererController.IsDisposed)
			{
				return;
			}

			await Control.UpdateBitmap(newImage, previous).ConfigureAwait(false);
		}

		static void UpdateAspect(IImageRendererController rendererController, ImageView Control, IImageController newImage, IImageController previous = null)
		{
			if (newImage == null || rendererController.IsDisposed)
			{
				return;
			}

			ImageView.ScaleType type = newImage.Aspect.ToScaleType();
			Control.SetScaleType(type);
		}
	}
}