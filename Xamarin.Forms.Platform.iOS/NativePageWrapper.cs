using System;

using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class NativePageWrapper : Page
	{
		public UIViewController ViewController { get; }

		public NativePageWrapper (UIViewController viewController)
		{
			ViewController = viewController;
		}
	}
}
