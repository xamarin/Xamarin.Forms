using System.ComponentModel;
using System.Drawing;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class FrameRenderer : VisualElementRenderer<Frame>
	{
		ShadowView _shadowView;

		protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
				SetupLayer();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
			    e.PropertyName == Xamarin.Forms.Frame.BorderColorProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.HasShadowProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.CornerRadiusProperty.PropertyName)
				SetupLayer();
		}

		void SetupLayer()
		{
			float cornerRadius = Element.CornerRadius;

			if (cornerRadius == -1f)
				cornerRadius = 5f; // default corner radius

			Layer.CornerRadius = cornerRadius;

			if (Element.BackgroundColor == Color.Default)
				Layer.BackgroundColor = UIColor.White.CGColor;
			else
				Layer.BackgroundColor = Element.BackgroundColor.ToCGColor();

			if (Element.BorderColor == Color.Default)
				Layer.BorderColor = UIColor.Clear.CGColor;
			else
			{
				Layer.BorderColor = Element.BorderColor.ToCGColor();
				Layer.BorderWidth = 1;
			}

			if (Element.HasShadow)
			{
				if (_shadowView == null)
				{
					_shadowView = new ShadowView(Layer);
					SetNeedsLayout();
				}
				_shadowView.Layer.CornerRadius = Layer.CornerRadius;
				_shadowView.Layer.BorderColor = Layer.BorderColor;
			}
			else
			{
				if (_shadowView != null)
				{
					_shadowView.RemoveFromSuperview();
					_shadowView.Dispose();
					_shadowView = null;
				}
			}

			Layer.RasterizationScale = UIScreen.MainScreen.Scale;
			Layer.ShouldRasterize = true;
		}

		public override void LayoutSubviews()
		{
			if (_shadowView != null)
			{
				if (_shadowView.Superview == null)
					Superview.InsertSubviewBelow(_shadowView, this);

				_shadowView?.SetNeedsLayout();
			}
			base.LayoutSubviews();
		}

		class ShadowView : UIView
		{
			CALayer _shadowee;
			CGRect _previousBounds;
			CGRect _previousFrame;

			public ShadowView(CALayer shadowee)
			{
				_shadowee = shadowee;
				BackgroundColor = UIColor.Clear;
				Layer.ShadowRadius = 5;
				Layer.ShadowColor = UIColor.Black.CGColor;
				Layer.ShadowOpacity = 0.8f;
				Layer.ShadowOffset = new SizeF();
				Layer.ShadowOpacity = 0.8f;
				Layer.BorderWidth = 1;
			}

			public override void LayoutSubviews()
			{
				if (_shadowee.Bounds != _previousBounds || _shadowee.Frame != _previousFrame)
				{
					base.LayoutSubviews();
					SetBounds();
				}
			}

			void SetBounds()
			{
				Layer.Frame = _shadowee.Frame;
				Layer.Bounds = _shadowee.Bounds;
				_previousBounds = _shadowee.Bounds;
				_previousFrame = _shadowee.Frame;
			}
		}
	}
}