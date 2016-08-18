using System;
using AppKit;
using Foundation;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.MacOS;

namespace Xamarin.Forms.ControlGallery.MacOS
{
	[Register("AppDelegate")]
	public class AppDelegate : FormsApplicationDelegate
	{

		NSWindow _window;
		public AppDelegate()
		{
			var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

			//var rect = new CoreGraphics.CGRect(0, 0, 1000, 1000);
			var rect = NSWindow.FrameRectFor(NSScreen.MainScreen.Frame, style);
			_window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
		}

		public override NSWindow MainWindow
		{
			get { return _window; }
		}

		public override void DidFinishLaunching(NSNotification notification)
		{
			Forms.Init();

			var app = new App();

			LoadApplication(app);
			base.DidFinishLaunching(notification);
		}
	}
}

