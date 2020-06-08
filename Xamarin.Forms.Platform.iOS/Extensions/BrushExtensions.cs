using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public static class BrushExtensions
	{
		const string BackgroundLayer = "BackgroundLayer";

		public static void UpdateBackground(this UIView control, Brush brush)
		{
			if (control == null)
				return;

			UIView view = ShouldUseParentView(control) ? control.Superview : control;

			// Remove previous background gradient layer if any
			RemoveGradientLayer(view);

			if (brush == null || brush.IsEmpty)
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
					control.BackgroundColor = UIColor.Clear;

					view.InsertGradientLayer(gradientLayer, 0);
				}
			}
		}

		public static CAGradientLayer GetGradientLayer(this UIView control, Brush brush)
		{
			if (control == null)
				return null;

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
					EndPoint = GetRadialGradientBrushEndPoint(center, radius),
					CornerRadius = (float)radius
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
			if (control == null || brush == null || brush.IsEmpty)
				return null;

			var gradientLayer = control.GetGradientLayer(brush);

			if (gradientLayer == null)
				return null;

			UIGraphics.BeginImageContextWithOptions(gradientLayer.Bounds.Size, false, UIScreen.MainScreen.Scale);

			if (UIGraphics.GetCurrentContext() == null)
				return null;

			gradientLayer.RenderInContext(UIGraphics.GetCurrentContext());
			UIImage gradientImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return gradientImage;
		}

		public static void InsertGradientLayer(this UIView view, CAGradientLayer gradientLayer, int index)
		{
			InsertGradientLayer(view.Layer, gradientLayer, index);
		}

		public static void InsertGradientLayer(this CALayer layer, CAGradientLayer gradientLayer, int index)
		{
			RemoveGradientLayer(layer);

			if (gradientLayer != null)
				layer.InsertSublayer(gradientLayer, index);
		}

		public static void RemoveGradientLayer(this UIView view)
		{
			if (view != null)
				RemoveGradientLayer(view.Layer);
		}

		public static void RemoveGradientLayer(this CALayer layer)
		{
			if (layer != null && layer.Sublayers != null && layer.Sublayers.Count() > 0)
			{
				var previousBackgroundLayer = layer.Sublayers.FirstOrDefault(x => x.Name == BackgroundLayer);
				previousBackgroundLayer?.RemoveFromSuperLayer();
			}
		}

		public static void UpdateGradientLayerSize(this UIView view)
		{
			if (view.Frame.IsEmpty)
				return;

			var layer = view.Layer;

			if (layer.Sublayers != null)
			{
				foreach (var sublayer in layer.Sublayers)
				{
					if (sublayer.Frame.IsEmpty && sublayer.Name == BackgroundLayer)
						sublayer.Frame = view.Bounds;
				}
			}
		}

		static bool ShouldUseParentView(UIView view)
		{
			if (view is UILabel)
				return true;

			return false;
		}

		static CGPoint GetRadialGradientBrushEndPoint(Point startPoint, double radius)
		{
			double x = startPoint.X == 1 ? (startPoint.X - radius) : (startPoint.X + radius);

			if (x < 0)
				x = 0;

			if (x > 1)
				x = 1;

			double y = startPoint.Y == 1 ? (startPoint.Y - radius) : (startPoint.Y + radius);

			if (y < 0)
				y = 0;

			if (y > 1)
				y = 1;

			return new CGPoint(x, y);
		}
	}
}