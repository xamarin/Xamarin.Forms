using System;
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.PlatformConfiguration.GTKSpecific;
using Xamarin.Forms.Shapes;
using Cairo;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
	public class ShapeRenderer<TShape, TNativeShape> : ViewRenderer<TShape, TNativeShape>
		where TShape : Shape
		where TNativeShape : ShapeView, new()
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TShape> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new TNativeShape());
				}

				UpdateFill();
				UpdateStroke();
				UpdateStrokeThickness();
				UpdateStrokeDashArray();
				UpdateStrokeDashOffset();
				UpdateStrokeLineCap();
				UpdateStrokeLineJoin();
				UpdateStrokeMiterLimit();

				UpdateSize();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Shape.FillProperty.PropertyName)
				UpdateFill();
			else if (e.PropertyName == Shape.StrokeProperty.PropertyName)
				UpdateStroke();
			else if (e.PropertyName == Shape.StrokeThicknessProperty.PropertyName)
				UpdateStrokeThickness();
			else if (e.PropertyName == Shape.StrokeDashArrayProperty.PropertyName)
				UpdateStrokeDashArray();
			else if (e.PropertyName == Shape.StrokeDashOffsetProperty.PropertyName)
				UpdateStrokeDashOffset();
			else if (e.PropertyName == Shape.StrokeLineCapProperty.PropertyName)
				UpdateStrokeLineCap();
			else if (e.PropertyName == Shape.StrokeLineJoinProperty.PropertyName)
				UpdateStrokeLineJoin();
			else if (e.PropertyName == Shape.StrokeMiterLimitProperty.PropertyName)
				UpdateStrokeMiterLimit();
		}

		protected override void OnSizeAllocated(Gdk.Rectangle allocation)
		{
			UpdateSize();

			base.OnSizeAllocated(allocation);
		}


		void UpdateFill()
		{
			if (Element == null || Control == null)
				return;

			Control.UpdateFill(Element.Fill);
		}

		void UpdateStroke()
		{
			if (Element == null || Control == null)
				return;

			Control.UpdateStroke(Element.Stroke);
		}

		void UpdateSize()
		{
			int height = HeightRequest;
			int width = WidthRequest;

			Control.UpdateSize(height, width);
		}

		void UpdateStrokeThickness()
		{
			Control.UpdateStrokeThickness(Element.StrokeThickness);
		}

		void UpdateStrokeDashArray()
		{
			Control.UpdateStrokeDashArray(Element.StrokeDashArray.ToArray());
		}

		void UpdateStrokeDashOffset()
		{
			Control.UpdateStrokeDashOffset(Element.StrokeDashOffset);
		}

		void UpdateStrokeLineCap()
		{
			PenLineCap lineCap = Element.StrokeLineCap;
			LineCap cairoStrokeCap = LineCap.Butt;
			switch (lineCap)
			{
				case PenLineCap.Flat:
					cairoStrokeCap = LineCap.Butt;
					break;
				case PenLineCap.Square:
					cairoStrokeCap = LineCap.Square;
					break;
				case PenLineCap.Round:
					cairoStrokeCap = LineCap.Round;
					break;
			}
			Control.UpdateStrokeLineCap(cairoStrokeCap);
		}

		void UpdateStrokeLineJoin()
		{
			PenLineJoin lineJoin = Element.StrokeLineJoin;
			LineJoin cairoStrokeJoin = LineJoin.Miter;
			switch (lineJoin)
			{
				case PenLineJoin.Miter:
					cairoStrokeJoin = LineJoin.Miter;
					break;
				case PenLineJoin.Bevel:
					cairoStrokeJoin = LineJoin.Bevel;
					break;
				case PenLineJoin.Round:
					cairoStrokeJoin = LineJoin.Round;
					break;
			}
			Control.UpdateStrokeLineJoin(cairoStrokeJoin);
		}

		void UpdateStrokeMiterLimit()
		{
			Control.UpdateStrokeMiterLimit((float)Element.StrokeMiterLimit);
		}

	}
}
