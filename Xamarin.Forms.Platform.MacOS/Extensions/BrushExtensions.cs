using System.ComponentModel;
using System.Linq;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
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
		const string SolidColorBrushLayer = "SolidColorBrushLayer";

		public static void UpdateBackground(this NSView control, BrushData brushData)
		{
			if (control == null)
				return;

			NSView view = ShouldUseParentView(control) ? control.Superview : control;

			// Clear previous background color
			if (control.Layer != null && control.Layer.Name == SolidColorBrushLayer)
				control.Layer.BackgroundColor = NSColor.Clear.CGColor;

			// Remove previous background gradient layer if any
			RemoveBackgroundLayer(view);

			if (brushData == null || Brush.IsNullOrEmpty(brushData.Brush))
				return;

			control.WantsLayer = true;
			control.LayerContentsRedrawPolicy = NSViewLayerContentsRedrawPolicy.BeforeViewResize;

			if (brushData.Brush is SolidColorBrush solidColorBrush)
			{
				control.Layer.Name = SolidColorBrushLayer;

				var backgroundColor = solidColorBrush.Color;

				if (backgroundColor == Color.Default)
					control.Layer.BackgroundColor = NSColor.Clear.CGColor;
				else
					control.Layer.BackgroundColor = backgroundColor.ToCGColor();
			}
			else
			{
				var backgroundLayer = GetBackgroundLayer(control, brushData);

				if (backgroundLayer != null)
					view.InsertBackgroundLayer(backgroundLayer, 0);
			}
		}

		public static CAGradientLayer GetBackgroundLayer(this NSView control, BrushData brushData)
		{
			if (control == null)
				return null;

			if (brushData == null || Brush.IsNullOrEmpty(brushData.Brush))
				return null;

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
					AutoresizingMask = CAAutoresizingMask.HeightSizable | CAAutoresizingMask.WidthSizable,
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
					Frame = control.Bounds,
					AutoresizingMask = CAAutoresizingMask.HeightSizable | CAAutoresizingMask.WidthSizable,
					ContentsGravity = CALayer.GravityResizeAspectFill,
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

		public static NSImage GetBackgroundImage(this NSView control, BrushData brushData)
		{
			if (control == null || brushData == null || brushData.Brush == null || brushData.Brush.IsEmpty)
				return null;

			var backgroundLayer = control.GetBackgroundLayer(brushData);

			if (backgroundLayer == null)
				return null;

			if (backgroundLayer.Bounds.IsEmpty)
				return null;

			NSImage backgroundImage = new NSImage(new CGSize(backgroundLayer.Bounds.Width, backgroundLayer.Bounds.Height));
			backgroundImage.LockFocus();
			var context = NSGraphicsContext.CurrentContext.GraphicsPort;
			backgroundLayer.RenderInContext(context);
			backgroundImage.UnlockFocus();

			return backgroundImage;
		}

		public static void InsertBackgroundLayer(this NSView view, CAGradientLayer backgroundLayer, int index)
		{
			if (view == null)
				return;

			InsertBackgroundLayer(view.Layer, backgroundLayer, index);
		}

		public static void InsertBackgroundLayer(this CALayer layer, CAGradientLayer backgroundLayer, int index)
		{
			RemoveBackgroundLayer(layer);

			if (backgroundLayer != null)
				layer.InsertSublayer(backgroundLayer, index);
		}

		public static void RemoveBackgroundLayer(this NSView view)
		{
			if (view != null)
				RemoveBackgroundLayer(view.Layer);
		}

		public static void RemoveBackgroundLayer(this CALayer layer)
		{
			if (layer != null)
			{
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
		}

		static bool ShouldUseParentView(NSView view)
		{
			if ((view is NSButton || view is NSTextField || view is NSDatePicker || view is NSSlider || view is NSStepper) && view.Superview != null)
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