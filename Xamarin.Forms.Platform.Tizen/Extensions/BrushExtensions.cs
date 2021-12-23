using System;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Tizen;

namespace Xamarin.Forms.Platform.Tizen
{
	public static class BrushExtensions
	{
		public static SKPath ToPath(this SKRectI bounds)
		{
			var path = new SKPath();
			path.AddRect(bounds);
			path.Close();
			return path;
		}

		public static SKPath ToPath(this SKRoundRect bounds)
		{
			var path = new SKPath();
			path.AddRoundRect(bounds);
			path.Close();
			return path;
		}

		public static SKPath ToRoundedRectPath(this SKRectI bounds, CornerRadius cornerRadius)
		{
			var path = new SKPath();
			var skRoundRect = new SKRoundRect(bounds);
			SKPoint[] radii = new SKPoint[4]
			{
				new SKPoint((float)cornerRadius.TopLeft, (float)cornerRadius.TopLeft),
				new SKPoint((float)cornerRadius.TopRight, (float)cornerRadius.TopRight),
				new SKPoint((float)cornerRadius.BottomRight, (float)cornerRadius.BottomRight),
				new SKPoint((float)cornerRadius.BottomLeft, (float)cornerRadius.BottomLeft)
			};
			skRoundRect.SetRectRadii(skRoundRect.Rect, radii);
			path.AddRoundRect(skRoundRect);
			path.Close();
			return path;
		}

		public static SKPaint GetBackgroundPaint(this VisualElement element, SKRectI bounds)
		{
			var brush = element.Background;
			if (Brush.IsNullOrEmpty(brush))
				return null;

			var paint = new SKPaint()
			{
				IsAntialias = true,
				Style = SKPaintStyle.Fill
			};

			if (brush is GradientBrush fillGradientBrush)
			{
				paint.Shader = fillGradientBrush.CreateShader(bounds);
			}
			else if (brush is SolidColorBrush solidColorBrush)
			{
				paint.Color = solidColorBrush.ToSolidColor();
			}
			return paint;
		}

		public static SKShader CreateShader(this GradientBrush gradientBrush, SKRect bounds)
		{
			SKShader shader = null;

			if (gradientBrush is LinearGradientBrush linearGradientBrush)
			{
				shader = CreateLinearGradient(linearGradientBrush, bounds);
			}

			if (gradientBrush is RadialGradientBrush radialGradientBrush)
			{
				shader = CreateRadialGradient(radialGradientBrush, bounds);
			}

			return shader;
		}

		public static SKColor ToSolidColor(this SolidColorBrush solidColorBrush)
		{
			return solidColorBrush.Color != Color.Default ? solidColorBrush.Color.ToNative().ToSKColor() : SKColor.Empty;
		}

		static SKShader CreateLinearGradient(LinearGradientBrush linearGradientBrush, SKRect pathBounds)
		{
			var startPoint = new SKPoint(pathBounds.Left + (float)linearGradientBrush.StartPoint.X * pathBounds.Width, pathBounds.Top + (float)linearGradientBrush.StartPoint.Y * pathBounds.Height);
			var endPoint = new SKPoint(pathBounds.Left + (float)linearGradientBrush.EndPoint.X * pathBounds.Width, pathBounds.Top + (float)linearGradientBrush.EndPoint.Y * pathBounds.Height);
			var orderedGradientStops = linearGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();
			var gradientColors = orderedGradientStops.Select(x => x.Color.ToNative().ToSKColor()).ToArray();
			var gradientColorPos = orderedGradientStops.Select(x => x.Offset).ToArray();
			return SKShader.CreateLinearGradient(startPoint, endPoint, gradientColors, gradientColorPos, SKShaderTileMode.Clamp);
		}

		static SKShader CreateRadialGradient(RadialGradientBrush radialGradientBrush, SKRect pathBounds)
		{
			var center = new SKPoint((float)radialGradientBrush.Center.X * pathBounds.Width + pathBounds.Left, (float)radialGradientBrush.Center.Y * pathBounds.Height + pathBounds.Top);
			var radius = (float)radialGradientBrush.Radius * Math.Max(pathBounds.Height, pathBounds.Width);
			var orderedGradientStops = radialGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();
			var gradientColors = orderedGradientStops.Select(x => x.Color.ToNative().ToSKColor()).ToArray();
			var gradientColorPos = orderedGradientStops.Select(x => x.Offset).ToArray();
			return SKShader.CreateRadialGradient(center, radius, gradientColors, gradientColorPos, SKShaderTileMode.Clamp);
		}
	}
}
