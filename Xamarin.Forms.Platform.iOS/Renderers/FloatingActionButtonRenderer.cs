using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using SizeF = CoreGraphics.CGSize;

namespace Xamarin.Forms.Platform.iOS
{
	public class FloatingActionButtonRenderer : ViewRenderer<FloatingActionButton, UIButton>
	{
		public override SizeF SizeThatFits(SizeF size)
		{
			if (Element == null)
				return SizeF.Empty;

			if (Element.Size == FloatingActionButtonSize.Mini)
				return new SizeF(44, 44);

			return new SizeF(56, 56);
		}

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
			{
				Control.TouchUpInside -= OnButtonTouchUpInside;
				Control.TouchDown -= OnButtonTouchDown;
			}

			base.Dispose(disposing);
		}

		protected override async void OnElementChanged(ElementChangedEventArgs<FloatingActionButton> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(CreateNativeControl());

					Debug.Assert(Control != null, "Control != null");

					Control.TouchUpInside += OnButtonTouchUpInside;
					Control.TouchDown += OnButtonTouchDown;
				}

				UpdateSize();
				UpdateBorder();
				UpdateColor();
				await TrySetImage();
			}
		}

		protected override UIButton CreateNativeControl()
		{
			return new UIButton(UIButtonType.System);
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == FloatingActionButton.ColorProperty.PropertyName)
				UpdateColor();
			else if (e.PropertyName == FloatingActionButton.SizeProperty.PropertyName)
			{
				UpdateBorder();
				UpdateSize();
			}
			else if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TrySetImage();
		}

		void OnButtonTouchUpInside(object sender, EventArgs eventArgs)
		{
			((IButtonController)Element)?.SendReleased();
			((IButtonController)Element)?.SendClicked();
		}

		void OnButtonTouchDown(object sender, EventArgs eventArgs)
		{
			((IButtonController)Element)?.SendPressed();
		}

		void UpdateBorder()
		{
			var uiButton = Control;
			var button = Element;

			uiButton.ClipsToBounds = false;

			nfloat cornerRadius = button.Size == FloatingActionButtonSize.Mini ? 22 : 28;
			uiButton.Layer.CornerRadius = cornerRadius;
			uiButton.Layer.ShadowColor = UIColor.Black.CGColor;
			uiButton.Layer.ShadowOpacity = 0.4f;
			uiButton.Layer.ShadowOffset = new CGSize(0, 4);
			uiButton.Layer.ShadowRadius = 4;
			uiButton.Layer.MasksToBounds = false;
		}

		void UpdateColor()
		{
			var color = Element.Color.ToUIColor();

			Control.TintColor = UIColor.White;
			Control.SetBackgroundImage (ImageColor(color), UIControlState.Normal);
		}

		void UpdateSize()
		{
			var size = Element.Size == FloatingActionButtonSize.Mini ? 44 : 56;

			Control.Frame = new CGRect(Control.Frame.X, Control.Frame.Y, size, size);
		}

		protected virtual async Task TrySetImage()
		{
			try
			{
				await SetImage().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Log.Warning(nameof(FloatingActionButtonRenderer), "Error loading image: {0}", ex);
			}
			finally
			{
			}
		}

		protected async Task SetImage()
		{
			if (Element == null || Control == null)
			{
				return;
			}

			var source = Element.ImageSource;

			IImageSourceHandler handler;

			if (source != null &&
				(handler = Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source)) != null)
			{
				UIImage uiimage;
				try
				{
					uiimage = await handler.LoadImageAsync(source, scale: (float)UIScreen.MainScreen.Scale);
				}
				catch (OperationCanceledException)
				{
					uiimage = null;
				}

				if (Control != null)
					Control.SetImage(uiimage, UIControlState.Normal);
			}
			else
			{
				Control.SetImage(null, UIControlState.Normal);
			}
		}

		public override void LayoutSubviews()
		{
			// VisualElement's width is used in base on layout subviews, need to override that here, base is not called on purpose
			UpdateSize();
		}

		UIImage ImageColor(UIColor color)
		{
			var rect = new CGRect(x: 0.0, y: 0.0, width: 1.0, height: 1.0);
			UIGraphics.BeginImageContext(rect.Size);

			using (var context = UIGraphics.GetCurrentContext())
			{
				context?.SetFillColor(color.CGColor);
				context?.FillRect(rect);
				var image = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
				return image;
			}
		}
	}
}
