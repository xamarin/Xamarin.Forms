using System;
using System.ComponentModel;
using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	public class FormsApplicationDelegate : NSApplicationDelegate
	{

		Application _application;
		bool _isSuspended;
		NSWindow _window;


		protected FormsApplicationDelegate()
		{
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && _application != null)
				_application.PropertyChanged -= ApplicationOnPropertyChanged;

			base.Dispose(disposing);
		}


		protected void LoadApplication(Application application)
		{
			if (application == null)
				throw new ArgumentNullException("application");

			Application.Current = application;
			_application = application;

			application.PropertyChanged += ApplicationOnPropertyChanged;
		}

		public override void DidFinishLaunching(Foundation.NSNotification notification)
		{
			// check contents of launch options and evaluate why the app was launched and respond
			// initialize the important data structures
			// prepare you apps window and views for display
			// keep lightweight, anything long winded should be executed asynchronously on a secondary thread.
			// application:didFinishLaunchingWithOptions

			var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;
			var rect = NSWindow.FrameRectFor(NSScreen.MainScreen.Frame, style);
			var window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
			window.Display();
			window.MakeKeyAndOrderFront(NSApplication.SharedApplication);
			_window = window;
			if (_application == null)
				throw new InvalidOperationException("You MUST invoke LoadApplication () before calling base.FinishedLaunching ()");

			SetMainPage();
			_application.SendStart();
		}

		public override void DidBecomeActive(Foundation.NSNotification notification)
		{
			// applicationDidBecomeActive
			// execute any OpenGL ES drawing calls
			if (_application != null && _isSuspended)
			{
				_isSuspended = false;
				_application.SendResume();
			}

		}

		public override async void DidResignActive(Foundation.NSNotification notification)
		{
			// applicationWillResignActive
			if (_application != null)
			{
				_isSuspended = true;
				await _application.SendSleepAsync();
			}

		}

		void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Application.MainPage))
				UpdateMainPage();
		}

		void SetMainPage()
		{
			UpdateMainPage();
		}

		void UpdateMainPage()
		{
			if (_application.MainPage == null)
				return;

			var platformRenderer = (PlatformRenderer)_window.ContentViewController;
			_window.ContentViewController = _application.MainPage.CreateViewController();
			_window.ContentView.LayoutSubtreeIfNeeded();
			_window.ContentView.NeedsLayout = true;
			if (platformRenderer != null)
				((IDisposable)platformRenderer.Platform).Dispose();
		}
	}
}

