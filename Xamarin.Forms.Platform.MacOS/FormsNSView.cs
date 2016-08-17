using System;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class FormsNSView : NSView
	{

		public FormsNSView()
		{
			WantsLayer = true;
		}
		public FormsNSView(CoreGraphics.CGRect frame) : base(frame)
		{
			WantsLayer = true;
		}

		public override void DrawRect(CoreGraphics.CGRect dirtyRect)
		{
			if (BackgroundColor != NSColor.Clear && BackgroundColor != null)
			{
				BackgroundColor.Set();
				NSGraphics.RectFill(this.Bounds);
			}

			base.DrawRect(dirtyRect);
		}


		public NSColor BackgroundColor
		{
			get;
			set;
		}
	}
}

