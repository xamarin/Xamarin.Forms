using CoreGraphics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms.Internals;
using SizeF = CoreGraphics.CGSize;

namespace Xamarin.Forms.Platform.iOS
{
	public class FloatingActionButtonRenderer : ViewRenderer<FloatingActionButton, UIButton>
	{
		const int SmallSize = 44;
		const int NormalSize = 56;

		public override SizeF SizeThatFits(SizeF size)
		{
			if (Element == null)
				return SizeF.Empty;

			if (Element.Size == FloatingActionButtonSize.Mini)
				return new SizeF(SmallSize, SmallSize);

			return new SizeF(NormalSize, NormalSize);
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

			if (e.Is(VisualElement.BackgroundColorProperty))
			{
				UpdateColor();
			}
			else if (e.Is(FloatingActionButton.SizeProperty))
			{
				UpdateBorder();
				UpdateSize();
			}
			else if (e.Is(FloatingActionButton.ImageSourceProperty))
			{
				await TrySetImage();
			}
			else if (e.Is(VisualElement.IsEnabledProperty))
			{
				UpdateEnabled();
			}
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

			nfloat cornerRadius = button.Size == FloatingActionButtonSize.Mini ? SmallSize / 2 : NormalSize / 2;
			uiButton.Layer.CornerRadius = cornerRadius;
			uiButton.Layer.ShadowColor = UIColor.Black.CGColor;
			uiButton.Layer.ShadowOpacity = 0.4f;
			uiButton.Layer.ShadowOffset = new CGSize(0, 4);
			uiButton.Layer.ShadowRadius = 4;
			uiButton.Layer.MasksToBounds = false;
		}

		void UpdateColor()
		{
			var color = Element.BackgroundColor.ToUIColor();

			Control.TintColor = UIColor.White;
			Control.BackgroundColor = color;
		}

		void UpdateSize()
		{
			var size = Element.Size == FloatingActionButtonSize.Mini ? SmallSize : NormalSize;
			Control.Frame = new CGRect(Control.Frame.X, Control.Frame.Y, size, size);
		}

		void UpdateEnabled()
		{
			Control.Enabled = Element.IsEnabled;
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
	}
}
