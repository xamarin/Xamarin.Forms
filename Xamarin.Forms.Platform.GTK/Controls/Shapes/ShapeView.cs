using System;
using Cairo;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
	public class ShapeView : GtkFormsContainer
	{
		protected Brush _fill, _stroke;
		protected int _height, _width;
		double? _strokeThickness;
		float? _strokeDashOffset, _strokeMiterLimit;
		double[] _dashArray;
		LineCap? _strokeCap;
		LineJoin? _strokeJoin;

		public void UpdateFill(Brush brush)
		{
			_fill = brush;
			QueueDraw();
		}

		public void UpdateStroke(Brush brush)
		{
			_stroke = brush;
		}

		public void UpdateSize(int height, int width)
		{
			_height = height;
			_width = width;
			QueueDraw();
		}

		public void UpdateStrokeThickness(double strokeThickness)
		{
			_strokeThickness = strokeThickness;
		}

		public void UpdateStrokeDashArray(double[] dashArray)
		{
			_dashArray = dashArray;
		}

		public void UpdateStrokeDashOffset(float strokeDashOffset)
		{
			_strokeDashOffset = strokeDashOffset;
		}

		public void UpdateStrokeLineCap(LineCap strokeCap)
		{
			_strokeCap = strokeCap;
		}

		public void UpdateStrokeLineJoin(LineJoin strokeJoin)
		{
			_strokeJoin = strokeJoin;
		}

		public void UpdateStrokeMiterLimit(float strokeMiterLimit)
		{
			_strokeMiterLimit = strokeMiterLimit;
			QueueDraw();
		}

		protected override void Draw(Gdk.Rectangle area, Context cr)
		{
			if (_fill is SolidColorBrush fillBrush)
			{
				cr.SetSourceRGBA(fillBrush.Color.R, fillBrush.Color.G, fillBrush.Color.B, fillBrush.Color.A);
				cr.FillPreserve();
			}
			else if (_fill != null)
				throw new NotImplementedException("Brushes other than SolidColorBrush are not implemented yet");
		}
	}
}