using System;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class PageLifecycleManager
	{
		IPageController _pageController;

		public PageLifecycleManager(IPageController pageController)
		{
			_pageController = pageController ?? throw new ArgumentNullException("You need to provide a Page Element");
		}

		public void HandlePageAppearing()
		{
			// UISplitViewController ping pongs this event when you minimize an app
			if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Active)
				return;

			_pageController?.SendAppearing();

		}

		public void HandlePageDisappearing()
		{
			if (_pageController == null)
				return;

			_pageController.SendDisappearing();
		}
	}
}