using System.Linq;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	public static class BrushExtensions
	{
		const string BackgroundLayer = "BackgroundLayer";
		const string SolidColorBrushLayer = "SolidColorBrushLayer";

		public static void UpdateBackground(this NSView control, Brush brush)
		{
			if (control == null)
				return;

			NSView view = ShouldUseParentView(control) ? control.Superview : control;

			// Clear previous background color
			if (control.Layer != null && control.Layer.Name.Equals(SolidColorBrushLayer))
				control.Layer.BackgroundColor = NSColor.Clear.CGColor;

			// Remove previous background gradient layer if any
			RemoveGradientLayer(view);

			if (brush == null || brush.IsEmpty)
				return;

			control.WantsLayer = true;

			if (brush is SolidColorBrush solidColorBrush)
			{
				var backgroundColor = solidColorBrush.Color;

				if (backgroundColor != Color.Default)
				{
					control.Layer.Name = SolidColorBrushLayer;
					control.Layer.BackgroundColor = backgroundColor.ToCGColor();
				}
			}
			else
			{
				var gradientLayer = GetGradientLayer(control, brush);

				if (gradientLayer != null)
					view.InsertGradientLayer(gradientLayer, 0);
			}
		}

		public static CAGradientLayer GetGradientLayer(this NSView control, Brush brush)
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
					EndPoint = new CGPoint(1, 1),
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

		public static NSImage GetGradientImage(this NSView control, Brush brush)
		{
			if (control == null || brush == null || brush.IsEmpty)
				return null;

			var gradientLayer = control.GetGradientLayer(brush);

			if (gradientLayer == null)
				return null;

			NSImage gradientImage = new NSImage(new CGSize(gradientLayer.Bounds.Width, gradientLayer.Bounds.Height));
			gradientImage.LockFocus();
			var context = NSGraphicsContext.CurrentContext.GraphicsPort;
			gradientLayer.RenderInContext(context);
			gradientImage.UnlockFocus();

			return gradientImage;
		}

		public static void InsertGradientLayer(this NSView view, CAGradientLayer gradientLayer, int index)
		{
			InsertGradientLayer(view.Layer, gradientLayer, index);
		}

		public static void InsertGradientLayer(this CALayer layer, CAGradientLayer gradientLayer, int index)
		{
			RemoveGradientLayer(layer);

			if (gradientLayer != null)
				layer.InsertSublayer(gradientLayer, index);
		}

		public static void RemoveGradientLayer(this NSView view)
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

		public static void UpdateGradientLayerSize(this NSView view)
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

		static bool ShouldUseParentView(NSView view)
		{
			if (view is NSButton || view is NSTextField || view is NSDatePicker || view is NSSlider || view is NSStepper)
				return true;

			return false;
		}
	}
}