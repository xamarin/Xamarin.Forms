using System;
using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class PlatformRenderer : NSViewController
	{
		internal PlatformRenderer(Platform platform)
		{
			Platform = platform;
			View = new NSView(NSApplication.SharedApplication.Windows[0].Frame);
		}

		public Platform Platform { get; set; }


		public override void ViewDidAppear()
		{
			Platform.DidAppear();
			base.ViewDidAppear();
		}

		public override void ViewDidLayout()
		{
			base.ViewDidLayout();
			Platform.LayoutSubviews();
		}

		public override void ViewWillAppear()
		{
			Platform.WillAppear();
			base.ViewWillAppear();
		}
	}
}

