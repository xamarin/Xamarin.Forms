using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreTelephony;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class PageLifecycleManager : IDisposable
	{
		NSObject _activateObserver;
		NSObject _resignObserver;
		bool _disposed;
		bool _appeared;
		IPageController _pageController;

		public PageLifecycleManager(IPageController pageController)
		{
			if (pageController == null)
				throw new ArgumentNullException("You need to provide a Page Element ");

			_pageController = pageController;

			_activateObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, n => HandlePageAppearing());

			_resignObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillResignActiveNotification, n => HandlePageDisappearing());
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				if (_activateObserver != null)
				{
					NSNotificationCenter.DefaultCenter.RemoveObserver(_activateObserver);
					_activateObserver = null;
				}

				if (_resignObserver != null)
				{
					NSNotificationCenter.DefaultCenter.RemoveObserver(_resignObserver);
					_resignObserver = null;
				}

				HandlePageDisappearing();

				_pageController = null;
			}

			_disposed = true;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void HandlePageAppearing()
		{
			if (!_appeared)
			{
				_appeared = true;
				_pageController?.SendAppearing();
			}
		}

		public void HandlePageDisappearing()
		{
			if (!_appeared || _pageController == null)
				return;

			_appeared = false;
			_pageController.SendDisappearing();
		}
	}
}