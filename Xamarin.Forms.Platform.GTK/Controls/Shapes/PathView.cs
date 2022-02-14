using System;
using System.Collections.Generic;
using Cairo;
using Gdk;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.Shapes;

namespace Xamarin.Forms.Platform.GTK.Controls
{
	public class PathView : ShapeView
	{
		private PathGeometry _geometry;

		public void UpdateGeometry(PathGeometry geometry)
		{
			_geometry = geometry;
			QueueDraw();
		}

		protected override void Draw(Gdk.Rectangle area, Context cr)
		{
			cr.FillRule = _geometry.FillRule == Shapes.FillRule.EvenOdd ? Cairo.FillRule.EvenOdd : Cairo.FillRule.Winding;

			foreach (var figure in _geometry.Figures)
			{
				cr.MoveTo(figure.StartPoint.X, figure.StartPoint.Y);

				Point lastPoint = figure.StartPoint;

				foreach (var segment in figure.Segments)
				{
					if (segment is LineSegment lineSegment)
					{
						cr.LineTo(lineSegment.Point.X, lineSegment.Point.Y);

						lastPoint = lineSegment.Point;
					}
					else if (segment is ArcSegment arcSegment)
					{
						List<Point> points = new List<Point>();

						GeometryHelper.FlattenArc(points,
							lastPoint,
							arcSegment.Point,
							arcSegment.Size.Width,
							arcSegment.Size.Height,
							arcSegment.RotationAngle,
							arcSegment.IsLargeArc,
							arcSegment.SweepDirection == SweepDirection.CounterClockwise,
							1);

						for (int i = 0; i < points.Count; i++)
						{
							if (!double.IsNaN(points[i].X) && !double.IsNaN(points[i].Y))
								cr.LineTo(points[i].X, points[i].Y);
						}

						if (points.Count > 0)
							if (!double.IsNaN(points[points.Count - 1].X) && !double.IsNaN(points[points.Count - 1].Y))
								lastPoint = points[points.Count - 1];

					}
				}

				if (figure.IsClosed)
					cr.ClosePath();
			}

			base.Draw(area, cr);
		}

	}
}
