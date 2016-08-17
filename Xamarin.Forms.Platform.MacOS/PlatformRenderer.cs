using System;
using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class PlatformRenderer : NSViewController
	{
		internal PlatformRenderer(Platform platform) : base(nibNameOrNull: null, nibBundleOrNull: null)
		{
			Platform = platform;
			View = new NSView(new CGRect(0, 0, 800, 800));
		}

		public Platform Platform { get; set; }


		public override void ViewDidAppear()
		{
			Platform.DidAppear();
			base.ViewDidAppear();
		}

		public override void ViewDidLayout()
		{
			Platform.LayoutSubviews();
			base.ViewDidLayout();
		}

		//public override void ViewDidLayoutSubviews()
		//{
		//	base.ViewDidLayoutSubviews();
		//	Platform.LayoutSubviews();
		//}

		public override void ViewWillAppear()
		{
			//View.BackgroundColor = UIColor.White;
			Platform.WillAppear();
			base.ViewWillAppear();
		}
	}
}

