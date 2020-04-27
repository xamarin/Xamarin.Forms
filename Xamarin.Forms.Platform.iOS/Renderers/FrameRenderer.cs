using System.ComponentModel;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class FrameRenderer : ViewRenderer<Frame, UIView>, ITabStop
	{
		readonly UIView _wrapperView = new UIView();
		UIView _actualView = new UIView();
		CGSize _previousSize;

		UIView ITabStop.TabStop => this;

		[Internals.Preserve(Conditional = true)]
		public FrameRenderer()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				// Add the subviews to the actual view.
				foreach (var item in NativeView.Subviews)
				{
					_actualView.AddSubview(item);
				}

				if (NativeView.GestureRecognizers != null)
				{
					foreach (var gesture in NativeView.GestureRecognizers)
					{
						_actualView.AddGestureRecognizer(gesture);
					}
				}

				_wrapperView.AddSubview(_actualView);
				SetNativeControl(_wrapperView);

				SetupLayer();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
			    e.PropertyName == Xamarin.Forms.Frame.BorderColorProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.HasShadowProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.CornerRadiusProperty.PropertyName ||
				e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
				SetupLayer();
		}

		public virtual void SetupLayer()
		{
			float cornerRadius = Element.CornerRadius;

			if (cornerRadius == -1f)
				cornerRadius = 5f; // default corner radius

			_wrapperView.Layer.CornerRadius = cornerRadius;
			_wrapperView.Layer.MasksToBounds = Layer.CornerRadius > 0;

			if (Element.BackgroundColor == Color.Default)
				_wrapperView.Layer.BackgroundColor = UIColor.White.CGColor;
			else
				_wrapperView.Layer.BackgroundColor = Element.BackgroundColor.ToCGColor();

			if (Element.BorderColor == Color.Default)
				_wrapperView.Layer.BorderColor = UIColor.Clear.CGColor;
			else
			{
				_wrapperView.Layer.BorderColor = Element.BorderColor.ToCGColor();
				_wrapperView.Layer.BorderWidth = 1;
			}

			if (Element.HasShadow)
			{
				_wrapperView.Layer.ShadowRadius = 5;
				_wrapperView.Layer.ShadowColor = UIColor.Black.CGColor;
				_wrapperView.Layer.ShadowOpacity = 0.8f;
				_wrapperView.Layer.ShadowOffset = new SizeF();
			}
			else
			{
				_wrapperView.Layer.ShadowOpacity = 0;
			}

			_wrapperView.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
			_wrapperView.Layer.ShouldRasterize = true;
		}

		public override void LayoutSubviews()
		{
			if (_previousSize != Bounds.Size)
				SetNeedsDisplay();

			base.LayoutSubviews();
		}

		public override void Draw(CGRect rect)
		{
			_actualView.Frame = Bounds;
			_wrapperView.Frame = Bounds;

			base.Draw(rect);

			_previousSize = Bounds.Size;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				if (_actualView != null)
				{
					
					for (var i = 0; i < _actualView.GestureRecognizers?.Length; i++)
						_actualView.GestureRecognizers.Remove(_actualView.GestureRecognizers[i]);

					for (var j = 0; j < _actualView.Subviews.Length; j++)
						_actualView.Subviews.Remove(_actualView.Subviews[j]);

					_actualView.RemoveFromSuperview();
					_actualView.Dispose();
					_actualView = null;
				}
			}
		}
	}
}