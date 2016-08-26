using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	class CustomNSTableHeaderView : NSTableHeaderView
	{

		public CustomNSTableHeaderView(CoreGraphics.CGRect frame, IVisualElementRenderer headerRenderer)
		{

			Frame = frame;
			AddSubview(headerRenderer.NativeView);
		}

		public override void Layout()
		{

			foreach (var view in Subviews)
			{
				view.Frame = Frame;

			}
			base.Layout();
		}
	}
}
