using AppKit;
using Foundation;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.MacOS;

namespace Xamarin.Forms.ControlGallery.MacOS
{
	[Register("AppDelegate")]
	public class AppDelegate : FormsApplicationDelegate
	{

		public override void DidFinishLaunching(NSNotification notification)
		{
			Forms.Init();

			var app = new App();

			LoadApplication(app);
			base.DidFinishLaunching(notification);
		}
	}
}

