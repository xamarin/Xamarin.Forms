﻿using System.Collections.Specialized;
using System.ComponentModel;
using Android.Content;
using Xamarin.Forms.Shapes;
using static Android.Graphics.Path;
using APath = Android.Graphics.Path;

namespace Xamarin.Forms.Platform.Android
{
	public class PolygonRenderer : ShapeRenderer<Polygon, PolygonView>
	{
		PointCollection _points;

		public PolygonRenderer(Context context) : base(context)
		{

		}

		protected override void OnElementChanged(ElementChangedEventArgs<Polygon> args)
		{
			if (Control == null && args.NewElement != null)
			{
				SetNativeControl(new PolygonView(Context));
			}

			base.OnElementChanged(args);

			if (args.NewElement != null)
			{
				UpdatePoints();
				UpdateFillRule();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged(sender, args);

			if (args.PropertyName == Polyline.PointsProperty.PropertyName)
				UpdatePoints();
			else if (args.PropertyName == Polyline.FillRuleProperty.PropertyName)
				UpdateFillRule();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				if (_points != null)
				{
					_points.CollectionChanged -= OnCollectionChanged;
					_points = null;
				}
			}
		}

		void UpdatePoints()
		{
			if (_points != null)
				_points.CollectionChanged -= OnCollectionChanged;

			_points = Element.Points;

			_points.CollectionChanged += OnCollectionChanged;

			Control.UpdatePoints(_points);
		}

		void UpdateFillRule()
		{
			Control.UpdateFillMode(Element.FillRule == FillRule.Nonzero);
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdatePoints();
		}
	}

	public class PolygonView : ShapeView
	{
		PointCollection _points;
		bool _fillMode;

		public PolygonView(Context context) : base(context)
		{

		}

		void UpdateShape()
		{
			if (_points != null && _points.Count > 1)
			{
				APath path = new APath();
				path.SetFillType(_fillMode ? FillType.Winding : FillType.EvenOdd);

				path.MoveTo(_density * (float)_points[0].X, _density * (float)_points[0].Y);

				for (int index = 1; index < _points.Count; index++)
					path.LineTo(_density * (float)_points[index].X, _density * (float)_points[index].Y);

				path.Close();

				UpdateShape(path);
			}
		}

		public void UpdatePoints(PointCollection points)
		{
			_points = points;
			UpdateShape();
		}

		public void UpdateFillMode(bool fillMode)
		{
			_fillMode = fillMode;
			UpdateShape();
		}
	}
}
