using System;
using Android.Graphics.Drawables.Shapes;
using Xamarin.Forms;
using AView = Android.Views.View;

namespace Xamarin.Platform
{
	public static class BrushExtensions
	{
		public static void UpdateBackground(this AView view, Brush2 brush)
		{
			if (view == null)
				return;

			if (view.Background is GradientStrokeDrawable)
			{
				// Remove previous background gradient if any
				view.SetBackground(null);
			}

			if (Brush2.IsNullOrEmpty(brush))
				return;

			if (brush is LinearGradientBrush2 linearGradientBrush)
			{
				GradientStopCollection2 gradients = linearGradientBrush.GradientStops;

				if (!IsValidGradient(gradients))
					return;
			}

			if (brush is RadialGradientBrush2 radialGradientBrush)
			{
				GradientStopCollection2 gradients = radialGradientBrush.GradientStops;

				if (!IsValidGradient(gradients))
					return;
			}

			view.SetPaintGradient(brush);
		}

		internal static bool IsValidGradient(GradientStopCollection2 gradients)
		{
			if (gradients == null || gradients.Count == 0)
				return false;

			return true;
		}

		internal static void SetPaintGradient(this AView view, Brush2 brush)
		{
			var gradientStrokeDrawable = new GradientStrokeDrawable
			{
				Shape = new RectShape()
			};

			gradientStrokeDrawable.SetStroke(0, Color.Default.ToNative());

			if (brush is SolidColorBrush2 solidColorBrush)
			{
				var color = solidColorBrush.Color.IsDefault ? Color.Default.ToNative() : solidColorBrush.Color.ToNative();
				gradientStrokeDrawable.SetColor(color);
			}
			else
				gradientStrokeDrawable.SetGradient(brush);

			view.Background?.Dispose();
			view.Background = gradientStrokeDrawable;
		}

		internal static Tuple<int[], float[]> GetGradientBrushData(this GradientBrush2 gradientBrush)
		{
			var orderStops = gradientBrush.GradientStops;

			int[] colors = new int[orderStops.Count];
			float[] offsets = new float[orderStops.Count];

			int count = 0;
			foreach (var orderStop in orderStops)
			{
				colors[count] = orderStop.Color.ToNative().ToArgb();
				offsets[count] = orderStop.Offset;
				count++;
			}

			return Tuple.Create(colors, offsets);
		}
	}
}