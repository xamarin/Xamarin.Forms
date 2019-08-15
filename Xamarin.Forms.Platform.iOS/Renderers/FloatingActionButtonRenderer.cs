using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms.Internals;
using SizeF = CoreGraphics.CGSize;

namespace Xamarin.Forms.Platform.iOS
{
	public class FloatingActionButtonRenderer : ViewRenderer<FloatingActionButton, UIButton>, IImageVisualElementRenderer
	{
		const int SmallSize = 44;
		const int NormalSize = 56;

		IImageVisualElementRenderer ImageVisualElementRenderer => this;

		public override SizeF SizeThatFits(SizeF size)
		{
			if (Element == null)
				return SizeF.Empty;

			if (Element.Size == FloatingActionButtonSize.Mini)
				return new SizeF(SmallSize, SmallSize);

			return new SizeF(NormalSize, NormalSize);
		}

		public bool IsDisposed { get; private set; }

		protected override void Dispose(bool disposing)
		{
			if (IsDisposed)
				return;

			if (disposing)
			{
				if (Control != null)
				{
					Control.TouchUpInside -= OnButtonTouchUpInside;
					Control.TouchDown -= OnButtonTouchDown;
				}
			}

			IsDisposed = true;

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

					Control.TouchUpInside += OnButtonTouchUpInside;
					Control.TouchDown += OnButtonTouchDown;
				}

				UpdateSize();
				UpdateBorder();
				UpdateColor();
				await UpdateImageAsync();
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
				await UpdateImageAsync();
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

		async Task UpdateImageAsync()
		{
			if (IsDisposed || Control == null || Element == null)
				return;

			var imageRenderer = ImageVisualElementRenderer;
			if (imageRenderer == null)
				return;

			try
			{
				await ImageElementManager.SetImage(imageRenderer, Element);
			}
			catch (Exception ex)
			{
				Internals.Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
			}
		}


		public UIImageView GetImage() => Control?.ImageView;

		public void SetImage(UIImage image)
		{
			if (IsDisposed || Control == null || Element == null)
				return;

			var control = Control;
			if (control == null)
				return;

			if (image != null)
			{
				control.SetImage(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
				control.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			}
			else
			{
				control.SetImage(null, UIControlState.Normal);
			}

			UpdateEdgeInsets();
		}

		void UpdateEdgeInsets()
		{
			if (IsDisposed || Control == null || Element == null)
				return;

			var control = Control;
			if (control == null)
				return;

			nfloat inset = Element.Size == FloatingActionButtonSize.Mini ? SmallSize * .2f : NormalSize * .3f;

			var imageInsets = new UIEdgeInsets(inset, inset, inset, inset);
			control.ImageEdgeInsets = imageInsets;
		}

		public override void LayoutSubviews()
		{
			// VisualElement's width is used in base on layout subviews, need to override that here, base is not called on purpose
			UpdateSize();
		}
	}
}
