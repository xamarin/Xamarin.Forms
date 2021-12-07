using System;
using Cairo;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.Shapes;

namespace Xamarin.Forms.Platform.GTK.Controls
{
	public class PolygonView : ShapeView
	{
		PointCollection _points;
		bool _fillMode;

		protected override void Draw(Gdk.Rectangle area, Context cr)
		{

			cr.FillRule = _fillMode ? Cairo.FillRule.Winding : Cairo.FillRule.EvenOdd;

			if (_points != null && _points.Count > 1)
			{
				cr.MoveTo(_points[0].X, _points[0].Y);

				for (int index = 1; index < _points.Count; index++)
					cr.LineTo((float)_points[index].X, (float)_points[index].Y);

				cr.ClosePath();
			}

			base.Draw(area, cr);
		}

		public void UpdatePoints(PointCollection points)
		{
			_points = points;
			QueueDraw();
		}

		public void UpdateFillMode(bool fillMode)
		{
			_fillMode = fillMode;
			QueueDraw();
		}
	}
}
