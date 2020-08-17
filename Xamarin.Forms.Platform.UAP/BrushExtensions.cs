﻿using System.Linq;
using WBrush = Windows.UI.Xaml.Media.Brush;
using WGradientStopCollection = Windows.UI.Xaml.Media.GradientStopCollection;
using WGradientStop = Windows.UI.Xaml.Media.GradientStop;
using WLinearGradientBrush = Windows.UI.Xaml.Media.LinearGradientBrush;
using WPoint = Windows.Foundation.Point;

#if UWP_18362
using WRadialGradientBrush = Microsoft.UI.Xaml.Media.RadialGradientBrush;
#endif

namespace Xamarin.Forms.Platform.UWP
{
	public static class BrushExtensions
	{
		public static WBrush ToBrush(this Brush brush)
		{
			if (brush == null)
				return null;
			
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
				var p2 = linearGradientBrush.EndPoint;

				return new WLinearGradientBrush(gradientStopCollection, 0)
				{
					StartPoint = new WPoint(p1.X, p1.Y),
					EndPoint = new WPoint(p2.X, p2.Y)
				};
			}
#if UWP_18362
			if (brush is RadialGradientBrush radialGradientBrush)
			{
				var wRadialGradientBrush = new WRadialGradientBrush()
				{
					Center = new WPoint(radialGradientBrush.Center.X, radialGradientBrush.Center.Y),
					RadiusX = radialGradientBrush.Radius,
					RadiusY = radialGradientBrush.Radius
				};

				var orderedStops = radialGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();

				foreach (var gradientStop in orderedStops)
					wRadialGradientBrush.GradientStops.Add(
						new WGradientStop { Color = gradientStop.Color.ToWindowsColor(), Offset = gradientStop.Offset });

				return wRadialGradientBrush;
			}
#endif

			return null;
		}
	}
}