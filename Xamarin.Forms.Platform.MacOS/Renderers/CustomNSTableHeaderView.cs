using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	sealed class CustomNSTableHeaderView : NSTableHeaderView
	{
		public CustomNSTableHeaderView(double width, IVisualElementRenderer headerRenderer)
		{
			var view = new NSView { WantsLayer = true };
			view.Layer.BackgroundColor = NSColor.White.CGColor;
			AddSubview(view);
			AddSubview(headerRenderer.NativeView);
			Update(width, headerRenderer);
		}

		public void Update(double width, IVisualElementRenderer headerRenderer)
		{
			var headerView = headerRenderer.Element;
			var request = headerView.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);
			Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(headerView, new Rectangle(0, 0, width, request.Request.Height));
			Frame = new CoreGraphics.CGRect(0, 0, width, request.Request.Height);
		}

		public override void Layout()
		{
			foreach (var view in Subviews)
				view.Frame = Frame;
			base.Layout();
		}
	}
}