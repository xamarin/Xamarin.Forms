using System;
using System.Diagnostics.Contracts;
using System.Linq;

using UIKit;

using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms
{
	public sealed class ViewControllerSegueTarget : SegueTarget
	{
		public UIViewController ViewController { get; }

		internal ViewControllerSegueTarget(UIViewController viewController)
		{
			if (viewController == null)
				throw new ArgumentNullException();
			ViewController = viewController;
		}

		public override Page ToPage()
		{
			return GetPage(ViewController);
		}

		static Page GetPage(UIViewController vc)
		{
			if (vc == null)
				return null;
			return (vc as IVisualElementRenderer)?.Element as Page
			    ?? GetPage(vc.ChildViewControllers.FirstOrDefault());
		}
	}
}
