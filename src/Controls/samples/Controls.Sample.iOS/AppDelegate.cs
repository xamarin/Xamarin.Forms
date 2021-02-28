using Foundation;
using UIKit;
using Microsoft.Maui;

#if !NET6_0
using Microsoft.Maui.Controls;
#endif

using Maui.Controls.Sample;

namespace Sample.iOS
{
	[Register("AppDelegate")]
	public class AppDelegate : MauiUIApplicationDelegate<MyApp>
	{
		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			Microsoft.Maui.Controls.Compatibility.Forms.Init();

			return base.FinishedLaunching(application, launchOptions);
		}
	}
}