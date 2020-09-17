using System.ComponentModel;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class BrushData
	{
		public BrushData()
		{

		}

		public BrushData(Brush brush, FlowDirection flowDirection)
		{
			Brush = brush;
			FlowDirection = flowDirection;
		}

		public Brush Brush { get; internal set; }
		public FlowDirection FlowDirection { get; internal set; }
	}

	public static class BrushExtensions
	{
		const string BackgroundLayer = "BackgroundLayer";

		public static void UpdateBackground(this UIView control, BrushData brushData)
		{
			if (control == null)
				return;

			UIView view = ShouldUseParentView(control) ? control.Superview : control;

			// Remove previous background gradient layer if any
			RemoveBackgroundLayer(view);

			if (brushData == null || Brush.IsNullOrEmpty(brushData.Brush))
				return;

			var backgroundLayer = GetBackgroundLayer(control, brushData);

			if (backgroundLayer != null)
			{
				control.BackgroundColor = UIColor.Clear;
				view.InsertBackgroundLayer(backgroundLayer, 0);
			}
		}

		public static CALayer GetBackgroundLayer(this UIView control, BrushData brushData)
		{
			if (control == null)
				return null;

			if (brushData == null || Brush.IsNullOrEmpty(brushData.Brush))
				return null;

			if (brushData.Brush is SolidColorBrush solidColorBrush)
			{
				var linearGradientLayer = new CALayer
				{
					Name = BackgroundLayer,
					ContentsGravity = CALayer.GravityResizeAspectFill,
					Frame = control.Bounds,
					BackgroundColor = solidColorBrush.Color.ToCGColor()
				};

				return linearGradientLayer;
			}

			if (brushData.Brush is LinearGradientBrush linearGradientBrush)
			{
				var p1 = linearGradientBrush.StartPoint;
				var p2 = linearGradientBrush.EndPoint;

				double x1, y1, x2, y2;

				if (brushData.FlowDirection == FlowDirection.RightToLeft)
				{
					x1 = p2.X;
					y1 = p2.Y;

					x2 = p1.X;
					y2 = p1.Y;
				}
				else
				{
					x1 = p1.X;
					y1 = p1.Y;

					x2 = p2.X;
					y2 = p2.Y;
				}

				var linearGradientLayer = new CAGradientLayer
				{
					Name = BackgroundLayer,
					ContentsGravity = CALayer.GravityResizeAspectFill,
					Frame = control.Bounds,
					LayerType = CAGradientLayerType.Axial,
					StartPoint = new CGPoint(x1, y1),
					EndPoint = new CGPoint(x2, y2)
				};

				if (linearGradientBrush.GradientStops != null && linearGradientBrush.GradientStops.Count > 0)
				{
					var orderedStops = linearGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();
					linearGradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
					linearGradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();
				}

				return linearGradientLayer;
			}

			if (brushData.Brush is RadialGradientBrush radialGradientBrush)
			{
				var center = radialGradientBrush.Center;
				var radius = radialGradientBrush.Radius;

				var radialGradientLayer = new CAGradientLayer
				{
					Name = BackgroundLayer,
					ContentsGravity = CALayer.GravityResizeAspectFill,
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

		public static UIImage GetBackgroundImage(this UIView control, BrushData brushData)
		{
			if (control == null || brushData == null || brushData.Brush == null || brushData.Brush.IsEmpty)
				return null;

			var backgroundLayer = control.GetBackgroundLayer(brushData);

			if (backgroundLayer == null)
				return null;

			UIGraphics.BeginImageContextWithOptions(backgroundLayer.Bounds.Size, false, UIScreen.MainScreen.Scale);

			if (UIGraphics.GetCurrentContext() == null)
				return null;

			backgroundLayer.RenderInContext(UIGraphics.GetCurrentContext());
			UIImage gradientImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext(); 

			return gradientImage;
		}

		public static void InsertBackgroundLayer(this UIView view, CALayer backgroundLayer, int index = -1)
		{
			InsertBackgroundLayer(view.Layer, backgroundLayer, index);
		}

		public static void InsertBackgroundLayer(this CALayer layer, CALayer backgroundLayer, int index = -1)
		{
			RemoveBackgroundLayer(layer);

			if (backgroundLayer != null)
			{
				if (index > -1)
					layer.InsertSublayer(backgroundLayer, index);
				else
					layer.AddSublayer(backgroundLayer);
			}
		}

		public static void RemoveBackgroundLayer(this UIView view)
		{
			if (view != null)
				RemoveBackgroundLayer(view.Layer);
		}

		public static void RemoveBackgroundLayer(this CALayer layer)
		{
			if (layer == null)
				return;

			if (layer.Name == BackgroundLayer)
				layer?.RemoveFromSuperLayer();

			if (layer.Sublayers == null || layer.Sublayers.Count() == 0)
				return;

			foreach (var subLayer in layer.Sublayers)
			{
				if (subLayer.Name == BackgroundLayer)
					subLayer?.RemoveFromSuperLayer();
			}
		}

		public static void UpdateBackgroundLayer(this UIView view)
		{
			if (view == null || view.Frame.IsEmpty)
				return;

			var layer = view.Layer;

			UpdateBackgroundLayer(layer, view.Bounds);
		}

		static void UpdateBackgroundLayer(this CALayer layer, CGRect bounds)
		{
			if (layer != null && layer.Sublayers != null)
			{
				foreach (var sublayer in layer.Sublayers)
				{
					UpdateBackgroundLayer(sublayer, bounds);

					if (sublayer.Name == BackgroundLayer && sublayer.Frame != bounds)
						sublayer.Frame = bounds;
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
			double y = startPoint.Y == 1 ? (startPoint.Y - radius) : (startPoint.Y + radius);

			return new CGPoint(x, y);
		}
	}
}