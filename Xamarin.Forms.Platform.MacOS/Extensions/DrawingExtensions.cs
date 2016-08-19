using System;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;

namespace Xamarin.Forms.Platform.MacOS
{
	public static class PointExtensions
	{
		public static Point ToPoint(this PointF point)
		{
			return new Point(point.X, point.Y);
		}

		public static PointF ToPointF(this Point point)
		{
			return new PointF(point.X, point.Y);
		}

	}

	public static class SizeExtensions
	{
		public static SizeF ToSizeF(this Size size)
		{
			return new SizeF((float)size.Width, (float)size.Height);
		}
	}

	public static class RectangleExtensions
	{
		public static Rectangle ToRectangle(this RectangleF rect)
		{
			return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF ToRectangleF(this Rectangle rect)
		{
			return new RectangleF((nfloat)rect.X, (nfloat)rect.Y, (nfloat)rect.Width, (nfloat)rect.Height);
		}
	}
}

