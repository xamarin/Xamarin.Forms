using System.Linq;

#if __MOBILE__
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
#endif
{
	public static class BrushExtensions
	{
		const string BackgroundLayer = "BackgroundLayer";

		public static void UpdateBackground(this UIView control, Brush brush)
		{
			if (control == null)
				return;

			// Remove previous background gradient layer if any
			RemoveGradientLayer(control);

			if (brush == null)
				return;

			if (brush is SolidColorBrush solidColorBrush)
			{
				var backgroundColor = solidColorBrush.Color;

				if (backgroundColor != Color.Default)
					control.BackgroundColor = backgroundColor.ToUIColor();
			}
			else
			{
				var gradientLayer = GetGradientLayer(control, brush);

				if (gradientLayer != null)
				{
					control.BackgroundColor = null;

					int index = GetGradientLayerIndex(control);
					control.InsertGradientLayer(gradientLayer, index);
				}
			}
		}

		public static CAGradientLayer GetGradientLayer(this UIView control, Brush brush)
		{
			if (brush is LinearGradientBrush linearGradientBrush)
			{
				var p1 = linearGradientBrush.StartPoint;
				var p2 = linearGradientBrush.EndPoint;

				var linearGradientLayer = new CAGradientLayer
				{
					Name = BackgroundLayer,
					Frame = control.Bounds,
					LayerType = CAGradientLayerType.Axial,
					StartPoint = new CGPoint(p1.X, p1.Y),
					EndPoint = new CGPoint(p2.X, p2.Y)
				};

				if (linearGradientBrush.GradientStops != null && linearGradientBrush.GradientStops.Count > 0)
				{
					var orderedStops = linearGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();
					linearGradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
					linearGradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();
				}

				return linearGradientLayer;
			}

			if (brush is RadialGradientBrush radialGradientBrush)
			{
				var center = radialGradientBrush.Center;
				var radius = radialGradientBrush.Radius;

				var radialGradientLayer = new CAGradientLayer
				{
					Name = BackgroundLayer,
					Frame = control.Bounds,
					LayerType = CAGradientLayerType.Radial,
					StartPoint = new CGPoint(center.X, center.Y),
					EndPoint = new CGPoint(1, 1),
					CornerRadius = (float)(radius)
				};

				if (radialGradientBrush.GradientStops != null && radialGradientBrush.GradientStops.Count > 0)
				{
					var orderedStops = radialGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();
					radialGradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
					radialGradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();
				}

				return radialGradientLayer;
			}

			return null;
		}

		public static UIImage GetGradientImage(this UIView control, Brush brush)
		{
			if (brush == null)
				return null;

			var gradientLayer = control.GetGradientLayer(brush);

			if (gradientLayer == null)
				return null;

			UIGraphics.BeginImageContextWithOptions(gradientLayer.Frame.Size, false, 0);
			gradientLayer.RenderInContext(UIGraphics.GetCurrentContext());
			var gradientImage = UIGraphics.GetImageFromCurrentImageContext();

			return gradientImage;
		}

		public static void InsertGradientLayer(this UIView view, CAGradientLayer gradientLayer, int index)
		{
			InsertGradientLayer(view.Layer, gradientLayer, index);
		}

		public static void InsertGradientLayer(this CALayer layer, CAGradientLayer gradientLayer, int index)
		{
			RemoveGradientLayer(layer);
			layer.InsertSublayer(gradientLayer, index);
		}

		public static void RemoveGradientLayer(this UIView view)
		{
			if (view != null)
				RemoveGradientLayer(view.Layer);
		}

		public static void RemoveGradientLayer(this CALayer layer)
		{
			if (layer.Sublayers != null && layer.Sublayers.Count() > 0)
			{
				var previousBackgroundLayer = layer.Sublayers.FirstOrDefault(x => x.Name == BackgroundLayer);
				previousBackgroundLayer?.RemoveFromSuperLayer();
			}
		}

		internal static int GetGradientLayerIndex(UIView view)
		{
			int index = 0;
			var sublayers = view.Layer.Sublayers;

			if (HasCustomLayers(sublayers))
			{
				int count = 0;

				foreach (var sublayer in sublayers)
				{
					count++;

					if (sublayers.GetType() != typeof(CALayer))
						break;
				}

				index = count;
			}

			return index;
		}

		internal static bool HasCustomLayers(CALayer[] sublayers)
		{
			if (sublayers == null)
				return false;

			int count = 0;

			foreach (var sublayer in sublayers)
			{
				if (sublayer.Sublayers == null && sublayers.GetType() != typeof(CALayer))
					count++;
			}

			return count > 0;
		}
	}
}