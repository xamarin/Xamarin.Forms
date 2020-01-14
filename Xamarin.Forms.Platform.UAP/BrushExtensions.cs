using System;
using System.Linq;
using WBrush = Windows.UI.Xaml.Media.Brush;
using WGradientStopCollection = Windows.UI.Xaml.Media.GradientStopCollection;
using WGradientStop = Windows.UI.Xaml.Media.GradientStop;
using WLinearGradientBrush = Windows.UI.Xaml.Media.LinearGradientBrush;
using WPoint = Windows.Foundation.Point;

namespace Xamarin.Forms.Platform.UWP
{
	public static class BrushExtensions
	{
		public static WBrush ToBrush(this Brush brush)
		{
			if (brush is SolidColorBrush solidColorBrush)
			{
				return solidColorBrush.Color.ToBrush();
			}

			if (brush is LinearGradientBrush linearGradientBrush)
			{
				var orderedStops = linearGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();
				var gradientStopCollection = new WGradientStopCollection();

				foreach (var item in orderedStops)
					gradientStopCollection.Add(new WGradientStop { Offset = item.Offset, Color = item.Color.ToWindowsColor() });

				var p1 = linearGradientBrush.StartPoint;
				var x1 = p1.X;
				var y1 = p1.Y;

				var p2 = linearGradientBrush.EndPoint;
				var x2 = p2.X;
				var y2 = p2.Y;

				var radians = Math.Atan2(y2 - y1, x2 - x1);
				var angle = radians * (180 / Math.PI);

				return new WLinearGradientBrush(gradientStopCollection, angle);
			}

			if (brush is Xamarin.Forms.RadialGradientBrush radialGradientBrush)
			{
				var orderedStops = radialGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();
				var gradientStopCollection = new WGradientStopCollection();

				foreach (var item in orderedStops)
					gradientStopCollection.Add(new WGradientStop { Offset = item.Offset, Color = item.Color.ToWindowsColor() });

				return new RadialGradientBrush(gradientStopCollection)
				{
					Center = new WPoint(radialGradientBrush.Center.X, radialGradientBrush.Center.Y),
					RadiusX = radialGradientBrush.Radius,
					RadiusY = radialGradientBrush.Radius
				};
			}

			return null;
		}
	}
}
