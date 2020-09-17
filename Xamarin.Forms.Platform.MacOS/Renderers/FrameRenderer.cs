using System.ComponentModel;
using System.Drawing;
using AppKit;
using CoreAnimation;

namespace Xamarin.Forms.Platform.MacOS
{
	public class FrameRenderer : VisualElementRenderer<Frame>
	{
		const float DefaultCornerRadius = 5f;

		protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
				SetupLayer();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
				e.PropertyName == VisualElement.BackgroundProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.BorderColorProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.HasShadowProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.CornerRadiusProperty.PropertyName ||
				e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				SetupLayer();
		}

		protected override void SetBackgroundColor(Color color)
		{
			if (Element.BackgroundColor == Color.Default)
				Layer.BackgroundColor = NSColor.White.CGColor;
			else
				Layer.BackgroundColor = Element.BackgroundColor.ToCGColor();
		}

		protected override void SetBackground(Brush brush)
		{
			Layer.BackgroundColor = NSColor.White.CGColor;
			Layer.RemoveBackgroundLayer();

			if (!Brush.IsNullOrEmpty(brush))
			{
				if (brush is SolidColorBrush solidColorBrush)
				{
					var backgroundColor = solidColorBrush.Color;

					if (backgroundColor == Color.Default)
						Layer.BackgroundColor = NSColor.White.CGColor;
					else
						Layer.BackgroundColor = backgroundColor.ToCGColor();
				}
				else
				{
					BrushData brushData = new BrushData(brush, Element.FlowDirection);
					var backgroundLayer = this.GetBackgroundLayer(brushData);

					if (backgroundLayer != null)
					{
						Layer.BackgroundColor = NSColor.Clear.CGColor;
						Layer.InsertBackgroundLayer(backgroundLayer, 0);
						SetupLayerBorder(backgroundLayer);
					}
				}
			}
		}

		void SetupLayer()
		{
			SetupLayerBorder(Layer);

			if (Element.HasShadow)
			{
				Layer.ShadowRadius = 5;
				Layer.ShadowColor = NSColor.Black.CGColor;
				Layer.ShadowOpacity = 0.8f;
				Layer.ShadowOffset = new SizeF();
			}
			else
				Layer.ShadowOpacity = 0;

			Layer.RasterizationScale = NSScreen.MainScreen.BackingScaleFactor;
			Layer.ShouldRasterize = true;
		}

		void SetupLayerBorder(CALayer layer)
		{
			float cornerRadius = Element.CornerRadius;

			if (cornerRadius == -1f)
				cornerRadius = DefaultCornerRadius; // default corner radius

			layer.CornerRadius = cornerRadius;

			if (Element.BorderColor == Color.Default)
				layer.BorderColor = NSColor.Clear.CGColor;
			else
			{
				layer.BorderColor = Element.BorderColor.ToCGColor();
				layer.BorderWidth = 1;
			}
		}
	}
}