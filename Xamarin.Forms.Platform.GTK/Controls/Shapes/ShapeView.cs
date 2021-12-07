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
		double? _strokeDashOffset, _strokeMiterLimit;
		double[] _strokeDashArray;
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
			QueueDraw();
		}

		public void UpdateStrokeDashArray(double[] strokeDashArray)
		{
			if (_strokeThickness.HasValue && strokeDashArray != null && strokeDashArray.Length > 1)
			{
				double[] strokeDash = new double[strokeDashArray.Length];

				for (int i = 0; i < strokeDashArray.Length; i++)
					strokeDash[i] = strokeDashArray[i] * _strokeThickness.Value;

				_strokeDashArray = strokeDash;
			}
			QueueDraw();
		}

		public void UpdateStrokeDashOffset(double strokeDashOffset)
		{
			if (_strokeThickness.HasValue)
				_strokeDashOffset = strokeDashOffset * _strokeThickness.Value;
			QueueDraw();
		}

		public void UpdateStrokeLineCap(LineCap strokeCap)
		{
			_strokeCap = strokeCap;
			QueueDraw();
		}

		public void UpdateStrokeLineJoin(LineJoin strokeJoin)
		{
			_strokeJoin = strokeJoin;
			QueueDraw();
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

			if (_stroke is SolidColorBrush strokeBrush)
			{
				if (_strokeCap.HasValue)
					cr.LineCap = _strokeCap.Value;
				if (_strokeJoin.HasValue)
					cr.LineJoin = _strokeJoin.Value;
				if (_strokeThickness.HasValue)
					cr.LineWidth = _strokeThickness.Value;
				if (_strokeDashOffset.HasValue && _strokeDashArray != null)
					cr.SetDash(_strokeDashArray, _strokeDashOffset.Value);
				if (_strokeMiterLimit.HasValue)
					cr.MiterLimit = _strokeMiterLimit.Value;

				cr.SetSourceRGBA(strokeBrush.Color.R, strokeBrush.Color.G, strokeBrush.Color.B, strokeBrush.Color.A);
				cr.StrokePreserve();
			}
			else if (_stroke != null)
				throw new NotImplementedException("Brushes other than SolidColorBrush are not implemented yet");

		}
	}
}