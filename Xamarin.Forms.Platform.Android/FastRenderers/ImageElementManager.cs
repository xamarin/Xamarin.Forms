﻿using System.ComponentModel;
using System.Threading.Tasks;
using Android.Widget;
using AScaleType = Android.Widget.ImageView.ScaleType;
using ARect = Android.Graphics.Rect;
using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public static class ImageElementManager
	{
		public static void Init(IVisualElementRenderer renderer)
		{
			renderer.ElementPropertyChanged += OnElementPropertyChanged;
			renderer.ElementChanged += OnElementChanged;
			renderer.LayoutChange += OnLayoutChange;
		}

		static void OnLayoutChange(object sender, global::Android.Views.View.LayoutChangeEventArgs e)
		{
			if(sender is IVisualElementRenderer renderer && renderer.View is ImageView imageView)
				imageView.ClipBounds = imageView.GetScaleType() == AScaleType.CenterCrop ? new ARect(0, 0, e.Right - e.Left, e.Bottom - e.Top) : null;
		}

		public static void Dispose(IVisualElementRenderer renderer)
		{
			renderer.ElementPropertyChanged -= OnElementPropertyChanged;
			renderer.ElementChanged -= OnElementChanged;
			renderer.LayoutChange -= OnLayoutChange;
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
				try
				{
					await TryUpdateBitmap(renderer as IImageRendererController, (ImageView)renderer.View, (IImageController)renderer.Element).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					Log.Warning(renderer.GetType().Name, "Error loading image: {0}", ex);
				}
				finally
				{
					ImageElementManager?.SetIsLoading(false);
				}
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