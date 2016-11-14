using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Xamarin.Forms.CustomAttributes;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;

#endif

namespace Xamarin.Forms.Controls
{
	internal static class AppPaths
    {
        public static string ApkPath = "../../../Xamarin.Forms.ControlGallery.Android/bin/Debug/AndroidControlGallery.AndroidControlGallery-Signed.apk";

		// Have to continue using the old BundleId for now; Test Cloud doesn't like
		// when you change the BundleId
        public static string BundleId = "com.xamarin.quickui.controlgallery";
    }

#if UITEST
	internal static class AppSetup
	{
		static IApp InitializeApp (bool isolate = false)
		{
			IApp app = null;
#if __ANDROID__

			app = isolate ? InitializeAndroidApp() : ConnectToAndroidApp();

#elif __IOS__
			//app = InitializeiOSApp();
			app = isolate ? InitializeiOSApp() : ConnectToiOSApp();
#endif
			if (app == null)
				throw new NullReferenceException ("App was not initialized.");

			// Wrap the app in ScreenshotConditional so it only takes screenshots if the SCREENSHOTS symbol is specified
			return new ScreenshotConditionalApp(app);
		}

#if __ANDROID__
		static IApp InitializeAndroidApp()
		{
			return ConfigureApp.Android.ApkFile(AppPaths.ApkPath).Debug().StartApp();
		}

		static IApp ConnectToAndroidApp()
		{
			try
			{
				var app = ConfigureApp.Android.ApkFile(AppPaths.ApkPath).Debug().ConnectToApp();
				// Attempt to talk to the app server; if the app isn't running, this will throw an exception
				app.TestServer.Get("");

				return app;
			}
			catch (Exception)
			{
				// The app either wasn't already running or we couldn't connect to it; start a new instance
			}

			return InitializeAndroidApp();
		}
#endif

#if __IOS__
		static IApp InitializeiOSApp() 
		{ 
			// Running on a device
			var app = ConfigureApp.iOS.InstalledApp(AppPaths.BundleId).Debug()
				//Uncomment to run from a specific iOS SIM, get the ID from XCode -> Devices
				.StartApp();

			// Running on the simulator
			//var app = ConfigureApp.iOS
			//				  .PreferIdeSettings()
			//				  .AppBundle("../../../Xamarin.Forms.ControlGallery.iOS/bin/iPhoneSimulator/Debug/XamarinFormsControlGalleryiOS.app")
			//				  .Debug()
			//				  .StartApp();

			return app;
		}

		static IApp ConnectToiOSApp() 
		{
			try
			{
				// TODO EZH Change this back to device 
				// Running on a device
				var app = ConfigureApp.iOS.InstalledApp(AppPaths.BundleId).Debug()
					.ConnectToApp();

				// Running on the simulator
				//var app = ConfigureApp.iOS
				//				  .PreferIdeSettings()
				//				  .AppBundle("../../../Xamarin.Forms.ControlGallery.iOS/bin/iPhoneSimulator/Debug/XamarinFormsControlGalleryiOS.app")
				//				  .Debug()
				//				  .ConnectToApp();

				return app;
			}
			catch (Exception) 
			{ 
			}

			return InitializeiOSApp();
		}
#endif

		static void NavigateToIssue (Type type, IApp app)
		{
			var typeIssueAttribute = type.GetTypeInfo ().GetCustomAttribute <IssueAttribute> ();

			string cellName = "";
			if (typeIssueAttribute.IssueTracker.ToString () != "None" &&
				typeIssueAttribute.IssueNumber != 1461 &&
				typeIssueAttribute.IssueNumber != 342) {
				cellName = typeIssueAttribute.IssueTracker.ToString ().Substring(0, 1) + typeIssueAttribute.IssueNumber.ToString ();
			} else {
				cellName = typeIssueAttribute.Description;
			}

			try
			{
				// Attempt the direct way of navigating to the test page
#if __ANDROID__

				if (bool.Parse((string)app.Invoke("NavigateToTest", cellName)))
				{
					return;
				}
#endif
#if __IOS__
				if (bool.Parse(app.Invoke("navigateToTest:", cellName).ToString()))
				{
					return;
				}
#endif
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Could not directly invoke test, using UI navigation. {ex}");
			}
			
			// Fall back to the "manual" navigation method
			app.Tap (q => q.Button ("Go to Test Cases"));
			app.WaitForElement (q => q.Raw ("* marked:'TestCasesIssueList'"));

			app.EnterText (q => q.Raw ("* marked:'SearchBarGo'"), cellName);

			app.WaitForElement (q => q.Raw ("* marked:'SearchButton'"));
			app.Tap (q => q.Raw ("* marked:'SearchButton'"));
		}

		public static IApp Setup (Type pageType = null, bool isolate = false)
		{
			IApp runningApp = null;
			try {
				runningApp = InitializeApp (isolate);
			} catch (Exception e) {
				Assert.Inconclusive ($"App did not start for some reason: {e}");
			}
			
			if (pageType != null)
				NavigateToIssue (pageType, runningApp);

			return runningApp;
		}
	}
#endif

	public abstract class TestPage : Page
	{
#if UITEST
		public IApp RunningApp { get; private set; }
#endif

		protected TestPage ()
		{
#if APP
			Init ();
#endif
		}

#if UITEST
		[SetUp]
		public void Setup ()
		{
			RunningApp = AppSetup.Setup (GetType ());
		}
#endif

		protected abstract void Init ();
	}


	public abstract class TestContentPage : ContentPage
	{
#if UITEST
		public IApp RunningApp { get; private set; }
#endif

		protected TestContentPage ()
		{
#if APP
			Init ();
#endif
		}

#if UITEST
		[SetUp]
		public void Setup ()
		{
			RunningApp = AppSetup.Setup (GetType ());
		}
#endif

		protected abstract void Init ();
	}

	public abstract class TestCarouselPage : CarouselPage
	{
#if UITEST
		public IApp RunningApp { get; private set; }
#endif

		protected TestCarouselPage ()
		{
#if APP
			Init ();
#endif
		}

#if UITEST
		[SetUp]
		public void Setup ()
		{
			RunningApp = AppSetup.Setup (GetType ());
		}
#endif

		protected abstract void Init ();
	}

	public abstract class TestMasterDetailPage : MasterDetailPage
	{
#if UITEST
		public IApp RunningApp { get; private set; }
#endif

		protected TestMasterDetailPage ()
		{
#if APP
			Init ();
#endif
		}

#if UITEST
		[SetUp]
		public void Setup ()
		{
			RunningApp = AppSetup.Setup (GetType ());
		}
#endif

		protected abstract void Init ();
	}

	public abstract class TestNavigationPage : NavigationPage
	{
#if UITEST
		public IApp RunningApp { get; private set; }
#endif

		protected TestNavigationPage ()
		{
#if APP
			Init ();
#endif
		}

#if UITEST
		[SetUp]
		public void Setup ()
		{
			RunningApp = AppSetup.Setup (GetType ());
		}
#endif

		protected abstract void Init ();
	}

	public abstract class TestTabbedPage : TabbedPage
	{
#if UITEST
		public IApp RunningApp { get; private set; }
#endif

		protected TestTabbedPage ()
		{
#if APP
			Init ();
#endif
		}

#if UITEST
		[SetUp]
		public void Setup ()
		{
			RunningApp = AppSetup.Setup (GetType ());
		}
#endif

		protected abstract void Init ();
	}
}
