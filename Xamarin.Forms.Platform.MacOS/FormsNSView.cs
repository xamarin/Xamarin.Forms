using System;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class FormsNSView : NSView
	{

		public FormsNSView(IntPtr ptr) : this(false, CoreGraphics.CGRect.Empty)
		{

		}

		public FormsNSView() : this(false, CoreGraphics.CGRect.Empty)
		{

		}

		public FormsNSView(bool isFlipped, CoreGraphics.CGRect frame) : base(frame)
		{
			//_isFlipped = isFlipped;
		}

		public override void Layout()
		{

			base.Layout();
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

		public override bool IsFlipped
		{
			get
			{
				return _isFlipped;
			}
		}

		public NSColor BackgroundColor
		{
			get;
			set;
		}

		bool _isFlipped;

	}
}

