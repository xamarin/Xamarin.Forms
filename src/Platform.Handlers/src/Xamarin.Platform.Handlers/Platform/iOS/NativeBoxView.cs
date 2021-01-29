using System;
using UIKit;
using Xamarin.Forms;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Platform
{
	public class NativeBoxView : UIView
	{
		const float PI = (float)Math.PI;
		const float PIAndAHalf = PI * 1.5f;
		const float HalfPI = PI * .5f;
		const float TwoPI = PI * 2;

		Size _size;
		Color _color;
		CornerRadius _cornerRadius;

		nfloat _topLeft;
		nfloat _topRight;
		nfloat _bottomLeft;
		nfloat _bottomRight;

		UIColor? _nativeColor;

		public NativeBoxView()
		{
			Opaque = false;
		}

		public Size Size
		{
			get { return _size; }
			set
			{
				_size = value;
				SetSize(_size);
			}
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				SetColor(_color);
			}
		}

		public CornerRadius CornerRadius
		{
			get { return _cornerRadius; }
			set
			{
				_cornerRadius = value;
				SetCornerRadius(_cornerRadius);
			}
		}

		public override void Draw(RectangleF rect)
		{
			UIBezierPath bezierPath = new UIBezierPath();

			bezierPath.AddArc(new CoreGraphics.CGPoint(Bounds.X + Bounds.Width - _topRight, Bounds.Y + _topRight), _topRight, PIAndAHalf, TwoPI, true);
			bezierPath.AddArc(new CoreGraphics.CGPoint(Bounds.X + Bounds.Width - _bottomRight, Bounds.Y + Bounds.Height - _bottomRight), _bottomRight, 0, HalfPI, true);
			bezierPath.AddArc(new CoreGraphics.CGPoint(Bounds.X + _bottomLeft, Bounds.Y + Bounds.Height - _bottomLeft), _bottomLeft, HalfPI, PI, true);
			bezierPath.AddArc(new CoreGraphics.CGPoint(Bounds.X + _topLeft, Bounds.Y + _topLeft), _topLeft, PI, PIAndAHalf, true);

			_nativeColor?.SetFill();
			bezierPath.Fill();

			base.Draw(rect);
		}

		void SetSize(Size size)
		{
			RectangleF frame = Frame;
			Frame = new RectangleF(frame.X, frame.Y, size.Width, size.Height);

			SetNeedsDisplay();
		}

		void SetColor(Color color)
		{
			_nativeColor = color.ToNative();

			SetNeedsDisplay();
		}

		void SetCornerRadius(CornerRadius cornerRadius)
		{
			var elementCornerRadius = cornerRadius;

			_topLeft = (nfloat)elementCornerRadius.TopLeft;
			_topRight = (nfloat)elementCornerRadius.TopRight;
			_bottomLeft = (nfloat)elementCornerRadius.BottomLeft;
			_bottomRight = (nfloat)elementCornerRadius.BottomRight;

			SetNeedsDisplay();
		}
	}
}