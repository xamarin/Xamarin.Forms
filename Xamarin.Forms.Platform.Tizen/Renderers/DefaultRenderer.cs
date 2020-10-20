using Xamarin.Forms.Shapes;
using ElmSharp;
using ELayout = ElmSharp.Layout;

namespace Xamarin.Forms.Platform.Tizen
{
	public sealed class DefaultRenderer : VisualElementRenderer<VisualElement>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<VisualElement> e)
		{
			if (NativeView == null)
			{
				var control = new ELayout(Forms.NativeParent);
				SetNativeView(control);
			}
			base.OnElementChanged(e);
		}
	}

	// Following Shapes dummy renderers can be replaced to real renderers,
	// when `InitializationOptions.UseSkiaSharp` value is set to true in an application.
	// ```
	// var option = new InitializationOptions(app)
	// {
	//         UseSkiaSharp = true
	// };
	// Forms.Init(option);
	// ```

	public class EllipseRenderer : VisualElementRenderer<Ellipse>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Ellipse> e)
		{
			if (NativeView == null)
			{
				var control = new ELayout(Forms.NativeParent);
				SetNativeView(control);
			}
			base.OnElementChanged(e);
		}
	}

	public class LineRenderer : VisualElementRenderer<Line>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Line> e)
		{
			if (NativeView == null)
			{
				var control = new ELayout(Forms.NativeParent);
				SetNativeView(control);
			}
			base.OnElementChanged(e);
		}
	}

	public class PathRenderer : VisualElementRenderer<Path>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Path> e)
		{
			if (NativeView == null)
			{
				var control = new ELayout(Forms.NativeParent);
				SetNativeView(control);
			}
			base.OnElementChanged(e);
		}
	}

	public class PolygonRenderer : VisualElementRenderer<Shapes.Polygon>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Shapes.Polygon> e)
		{
			if (NativeView == null)
			{
				var control = new ELayout(Forms.NativeParent);
				SetNativeView(control);
			}
			base.OnElementChanged(e);
		}
	}

	public class PolylineRenderer : VisualElementRenderer<Polyline>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Polyline> e)
		{
			if (NativeView == null)
			{
				var control = new ELayout(Forms.NativeParent);
				SetNativeView(control);
			}
			base.OnElementChanged(e);
		}
	}

	public class RectangleRenderer : VisualElementRenderer<Shapes.Rectangle>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Shapes.Rectangle> e)
		{
			if (NativeView == null)
			{
				var control = new ELayout(Forms.NativeParent);
				SetNativeView(control);
			}
			base.OnElementChanged(e);
		}
	}
}
