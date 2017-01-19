using AppKit;
using CoreGraphics;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	sealed class CustomNSTableHeaderView : NSTableHeaderView
	{
		public CustomNSTableHeaderView() : this(0, null) { }
		public CustomNSTableHeaderView(double width, IVisualElementRenderer headerRenderer)
		{
			var view = new NSView { WantsLayer = true };
			view.Layer.BackgroundColor = NSColor.Purple.CGColor;
			AddSubview(view);
			Update(width, headerRenderer);
		}

		public override void DrawRect(CoreGraphics.CGRect dirtyRect)
		{
			//	Layer.BackgroundColor = Color.Pink.ToCGColor();
			//base.DrawRect(dirtyRect);
		}

		public void Update(double width, IVisualElementRenderer headerRenderer)
		{
			double height = 1;
			if (headerRenderer != null)
			{
				var headerView = headerRenderer.Element;
				var request = headerView.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);
				height = request.Request.Height;
				var bounds = new Rectangle(0, 0, width, height);
				Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(headerView, bounds);
				headerRenderer.NativeView.Frame = bounds.ToRectangleF();
				AddSubview(headerRenderer.NativeView);

			}
			Frame = new CGRect(0, 0, width, height);
		}

		//hides default text field
		public override NSAttributedString PageHeader => new NSAttributedString("");

		public override void Layout()
		{
			foreach (var view in Subviews)
				view.Frame = Frame;
			base.Layout();
		}
	}
}