using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using static Android.Graphics.Drawables.GradientDrawable;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public static class BrushExtensions
	{
		public static void UpdateBackground(this AView view, Brush brush)
		{
			GradientStopCollection gradients = null;

			if (brush is SolidColorBrush solidColorBrush)
			{
				var backgroundColor = solidColorBrush.Color;

				if (backgroundColor.IsDefault)
					view.SetBackground(null);
				else
					view.SetBackgroundColor(backgroundColor.ToAndroid());

				return;
			}

			if (brush is LinearGradientBrush linearGradientBrush)
				gradients = linearGradientBrush.GradientStops;

			if (brush is RadialGradientBrush radialGradientBrush)
				gradients = radialGradientBrush.GradientStops;

			if (gradients == null || gradients.Count == 0)
				return;

			view.SetPaintGradient(brush);
		}

		public static void UpdateBackground(this Paint paint, Brush brush, int height, int width)
		{
			if (brush is SolidColorBrush solidColorBrush)
			{
				var backgroundColor = solidColorBrush.Color;
				paint.Color = backgroundColor.ToAndroid();
			}

			if (brush is LinearGradientBrush linearGradientBrush)
			{
				var p1 = linearGradientBrush.StartPoint;
				var x1 = (float)p1.X;
				var y1 = (float)p1.Y;

				var p2 = linearGradientBrush.EndPoint;
				var x2 = (float)p2.X;
				var y2 = (float)p2.Y;

				var gradientBrushData = linearGradientBrush.GetGradientBrushData();
				var colors = gradientBrushData.Item1;
				var offsets = gradientBrushData.Item2;

				if (colors.Length < 2)
					return;

				var linearGradientShader = new LinearGradient(
					width * x1,
					height * y1,
					width * x2,
					height * y2,
					colors,
					offsets,
					Shader.TileMode.Clamp);

				paint.SetShader(linearGradientShader);
			}

			if (brush is RadialGradientBrush radialGradientBrush)
			{
				var center = radialGradientBrush.Center;
				float centerX = (float)center.X;
				float centerY = (float)center.Y;
				float radius = (float)radialGradientBrush.Radius;

				var gradientBrushData = radialGradientBrush.GetGradientBrushData();
				var colors = gradientBrushData.Item1;
				var offsets = gradientBrushData.Item2;

				if (colors.Length < 2)
					return;

				var radialGradientShader = new RadialGradient(
					width * centerX,
					height * centerY,
					Math.Max(height, width) * radius,
					colors,
					offsets,
					Shader.TileMode.Clamp);

				paint.SetShader(radialGradientShader);
			}
		}

		public static void UpdateBackground(this GradientDrawable gradientDrawable, Brush brush, int height, int width)
		{
			if (brush == null && brush.IsEmpty)
				return;

			if (brush is SolidColorBrush solidColorBrush)
			{
				Color bgColor = solidColorBrush.Color;
				gradientDrawable.SetColor(bgColor.IsDefault ? Color.White.ToAndroid() : bgColor.ToAndroid());
			}

			if (brush is LinearGradientBrush linearGradientBrush)
			{
				var p1 = linearGradientBrush.StartPoint;
				var x1 = (float)p1.X;
				var y1 = (float)p1.Y;

				var p2 = linearGradientBrush.EndPoint;
				var x2 = (float)p2.X;
				var y2 = (float)p2.Y;

				const double Rad2Deg = 180.0 / Math.PI;
				var angle = Math.Atan2(y2 - y1, x2 - x1) * Rad2Deg;

				var gradientBrushData = linearGradientBrush.GetGradientBrushData();
				var colors = gradientBrushData.Item1;

				if (colors.Length < 2)
					return;

				gradientDrawable.SetGradientType(GradientType.LinearGradient);
				gradientDrawable.SetColors(colors);
				gradientDrawable.SetGradientOrientation(angle);
			}

			if (brush is RadialGradientBrush radialGradientBrush)
			{
				var center = radialGradientBrush.Center;
				float centerX = (float)center.X;
				float centerY = (float)center.Y;
				float radius = (float)radialGradientBrush.Radius;

				var gradientBrushData = radialGradientBrush.GetGradientBrushData();
				var colors = gradientBrushData.Item1;

				if (colors.Length < 2)
					return;

				gradientDrawable.SetGradientType(GradientType.RadialGradient);
				gradientDrawable.SetGradientCenter(centerX, centerY);
				gradientDrawable.SetGradientRadius(Math.Max(height, width) * radius);
				gradientDrawable.SetColors(colors);
			}
		}

		internal static void SetPaintGradient(this AView view, Brush brush)
		{
			var gradientStrokeDrawable = new GradientStrokeDrawable
			{
				Shape = new RectShape()
			};
			gradientStrokeDrawable.SetGradient(brush);

			view.Background?.Dispose();
			view.Background = gradientStrokeDrawable;
		}

		internal static void SetGradientOrientation(this GradientDrawable drawable, double angle)
		{
			var orientation =
				angle >= 0 && angle < 45 ? Orientation.LeftRight :
				angle < 90 ? Orientation.BlTr :
				angle < 135 ? Orientation.BottomTop :
				angle < 180 ? Orientation.BrTl :
				angle < 225 ? Orientation.RightLeft :
				angle < 270 ? Orientation.TrBl :
				angle < 315 ? Orientation.TopBottom : Orientation.TlBr;

			drawable.SetOrientation(orientation);
		}

		internal static Tuple<int[], float[]> GetGradientBrushData(this GradientBrush gradientBrush)
		{
			var orderStops = gradientBrush.GradientStops;

			int[] colors = new int[orderStops.Count];
			float[] offsets = new float[orderStops.Count];

			int count = 0;
			foreach (var orderStop in orderStops)
			{
				colors[count] = orderStop.Color.ToAndroid().ToArgb();
				offsets[count] = orderStop.Offset;
				count++;
			}

			return Tuple.Create(colors, offsets);
		}
	}
}