using System.ComponentModel;
using System.Drawing;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
#if __UNIFIED__
using UIKit;
using CoreGraphics;
#else
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
#endif
#if __UNIFIED__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;

#else
using nfloat=System.Single;
using nint=System.Int32;
using nuint=System.UInt32;
#endif

namespace Xamarin.Forms.Platform.iOS
{
	public class BoxRenderer : VisualElementRenderer<BoxView>
	{
		UIColor _colorToRenderer;
		UIVisualEffectView _blur;
		BlurEffectStyle _previousBlur;
		SizeF _previousSize;

		public override void Draw(RectangleF rect)
		{
			using (var context = UIGraphics.GetCurrentContext())
			{
				_colorToRenderer.SetFill();
				context.FillRect(rect);
			}
			base.Draw(rect);

			_previousSize = Bounds.Size;

			if (_blur != null)
			{
				_blur.Frame = rect;
				if (_blur.Superview == null)
					Superview.Add(_blur);
			}
		}

		public override void LayoutSubviews()
		{
			if (_previousSize != Bounds.Size)
				SetNeedsDisplay();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged(e);

			if (Element != null)
			{
				SetBackgroundColor(Element.BackgroundColor);
				SetBlur();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == BoxView.ColorProperty.PropertyName)
				SetBackgroundColor(Element.BackgroundColor);
			else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && Element.IsVisible)
				SetNeedsDisplay();
			else if (e.PropertyName == PlatformConfiguration.iOSSpecific.BoxView.BlurEffectProperty.PropertyName)
				SetBlur();
		}

		protected override void SetBackgroundColor(Color color)
		{
			if (Element == null)
				return;

			var elementColor = Element.Color;
			if (!elementColor.IsDefault)
				_colorToRenderer = elementColor.ToUIColor();
			else
				_colorToRenderer = color.ToUIColor();

			SetNeedsDisplay();
		}

		void SetBlur()
		{
			if (Element == null)
				return;

			var blur = Element.OnThisPlatform().GetBlurEffect();

			if (_previousBlur == blur)
				return;

			if (_blur != null)
			{
				_blur.RemoveFromSuperview();
				_blur = null;
			}

			if (blur == BlurEffectStyle.None)
			{
				SetNeedsDisplay();
				return;
			}

			UIBlurEffect blurEffect;
			switch (blur)
			{
				default:
				case BlurEffectStyle.ExtraLight:
					blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight);
					break;
				case BlurEffectStyle.Light:
					blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
					break;
				case BlurEffectStyle.Dark:
					blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark);
					break;
			}

			_blur = new UIVisualEffectView(blurEffect);
			SetNeedsDisplay();
		}
	}
}