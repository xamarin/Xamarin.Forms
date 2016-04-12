using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

using Xamarin.Forms.CustomAttributes;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
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
		class WrapperApp : IApp
		{
			readonly IApp _inner;

			public WrapperApp(IApp inner)
			{
				_inner = inner;
			}

			public AppResult[] Query(Func<AppQuery, AppQuery> query = null)
			{
				return _inner.Query(query);
			}

			public AppResult[] Query(string marked)
			{
				return _inner.Query(marked);
			}

			public AppWebResult[] Query(Func<AppQuery, AppWebQuery> query)
			{
				return _inner.Query(query);
			}

			public T[] Query<T>(Func<AppQuery, AppTypedSelector<T>> query)
			{
				return _inner.Query(query);
			}

			public string[] Query(Func<AppQuery, InvokeJSAppQuery> query)
			{
				return _inner.Query(query);
			}

			public AppResult[] Flash(Func<AppQuery, AppQuery> query = null)
			{
				return _inner.Flash(query);
			}

			public AppResult[] Flash(string marked)
			{
				return _inner.Flash(marked);
			}

			public void EnterText(string text)
			{
				_inner.EnterText(text);
			}

			public void EnterText(Func<AppQuery, AppQuery> query, string text)
			{
				_inner.EnterText(query, text);
			}

			public void EnterText(string marked, string text)
			{
				_inner.EnterText(marked, text);
			}

			public void EnterText(Func<AppQuery, AppWebQuery> query, string text)
			{
				_inner.EnterText(query, text);
			}

			public void ClearText(Func<AppQuery, AppQuery> query)
			{
				_inner.ClearText(query);
			}

			public void ClearText(string marked)
			{
				_inner.ClearText(marked);
			}

			public void ClearText()
			{
				_inner.ClearText();
			}

			public void PressEnter()
			{
				_inner.PressEnter();
			}

			public void DismissKeyboard()
			{
				_inner.DismissKeyboard();
			}

			public void Tap(Func<AppQuery, AppQuery> query)
			{
				_inner.Tap(query);
			}

			public void Tap(string marked)
			{
				_inner.Tap(marked);
			}

			public void Tap(Func<AppQuery, AppWebQuery> query)
			{
				_inner.Tap(query);
			}

			public void TapCoordinates(float x, float y)
			{
				_inner.TapCoordinates(x, y);
			}

			public void TouchAndHold(Func<AppQuery, AppQuery> query)
			{
				_inner.TouchAndHold(query);
			}

			public void TouchAndHold(string marked)
			{
				_inner.TouchAndHold(marked);
			}

			public void TouchAndHoldCoordinates(float x, float y)
			{
				_inner.TouchAndHoldCoordinates(x, y);
			}

			public void DoubleTap(Func<AppQuery, AppQuery> query)
			{
				_inner.DoubleTap(query);
			}

			public void DoubleTap(string marked)
			{
				_inner.DoubleTap(marked);
			}

			public void DoubleTapCoordinates(float x, float y)
			{
				_inner.DoubleTapCoordinates(x, y);
			}

			public void PinchToZoomIn(Func<AppQuery, AppQuery> query, TimeSpan? duration = null)
			{
				_inner.PinchToZoomIn(query, duration);
			}

			public void PinchToZoomIn(string marked, TimeSpan? duration = null)
			{
				_inner.PinchToZoomIn(marked, duration);
			}

			public void PinchToZoomInCoordinates(float x, float y, TimeSpan? duration)
			{
				_inner.PinchToZoomInCoordinates(x, y, duration);
			}

			public void PinchToZoomOut(Func<AppQuery, AppQuery> query, TimeSpan? duration = null)
			{
				_inner.PinchToZoomOut(query, duration);
			}

			public void PinchToZoomOut(string marked, TimeSpan? duration = null)
			{
				_inner.PinchToZoomOut(marked, duration);
			}

			public void PinchToZoomOutCoordinates(float x, float y, TimeSpan? duration)
			{
				_inner.PinchToZoomOutCoordinates(x, y, duration);
			}

			public void WaitFor(Func<bool> predicate, string timeoutMessage = "Timed out waiting...", TimeSpan? timeout = null,
				TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
			{
				_inner.WaitFor(predicate, timeoutMessage, timeout, retryFrequency, postTimeout);
			}

			public AppResult[] WaitForElement(Func<AppQuery, AppQuery> query, string timeoutMessage = "Timed out waiting for element...",
				TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
			{
				return _inner.WaitForElement(query, timeoutMessage, timeout, retryFrequency, postTimeout);
			}

			public AppResult[] WaitForElement(string marked, string timeoutMessage = "Timed out waiting for element...",
				TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
			{
				return _inner.WaitForElement(marked, timeoutMessage, timeout, retryFrequency, postTimeout);
			}

			public AppWebResult[] WaitForElement(Func<AppQuery, AppWebQuery> query, string timeoutMessage = "Timed out waiting for element...",
				TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
			{
				return _inner.WaitForElement(query, timeoutMessage, timeout, retryFrequency, postTimeout);
			}

			public void WaitForNoElement(Func<AppQuery, AppQuery> query, string timeoutMessage = "Timed out waiting for no element...",
				TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
			{
				_inner.WaitForNoElement(query, timeoutMessage, timeout, retryFrequency, postTimeout);
			}

			public void WaitForNoElement(string marked, string timeoutMessage = "Timed out waiting for no element...",
				TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
			{
				_inner.WaitForNoElement(marked, timeoutMessage, timeout, retryFrequency, postTimeout);
			}

			public void WaitForNoElement(Func<AppQuery, AppWebQuery> query, string timeoutMessage = "Timed out waiting for no element...",
				TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
			{
				_inner.WaitForNoElement(query, timeoutMessage, timeout, retryFrequency, postTimeout);
			}

			public FileInfo Screenshot(string title)
			{
				// do nothing here, this is the entire point of this otherwise massive class
				return null;
			}

			public void SwipeRight()
			{
#pragma warning disable 618
				_inner.SwipeRight();
#pragma warning restore 618
			}

#if __WINDOWS__
			public void SwipeLeftToRight(double swipePercentage = 0.9)
			{
				_inner.SwipeLeftToRight(swipePercentage);
			}

			public void SwipeLeftToRight(string marked, double swipePercentage = 0.9)
			{
				_inner.SwipeLeftToRight(marked, swipePercentage);
			}
#else
			public void SwipeLeftToRight(double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
			{
				_inner.SwipeLeftToRight(swipePercentage, swipeSpeed, withInertia);
			}

			public void SwipeLeftToRight(string marked, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
			{
				_inner.SwipeLeftToRight(marked, swipePercentage, swipeSpeed, withInertia);
			}
#endif

			public void SwipeLeft()
			{
#pragma warning disable 618
				_inner.SwipeLeft();
#pragma warning restore 618
			}

#if __WINDOWS__
			public void SwipeRightToLeft(double swipePercentage = 0.9)
			{
				_inner.SwipeRightToLeft(swipePercentage);
			}

			public void SwipeRightToLeft(string marked, double swipePercentage = 0.9)
			{
				_inner.SwipeRightToLeft(marked, swipePercentage);
			}

			public void SwipeLeftToRight(Func<AppQuery, AppQuery> query, double swipePercentage = 0.9)
			{
				_inner.SwipeLeftToRight(query, swipePercentage);
			}

			public void SwipeRightToLeft(Func<AppQuery, AppQuery> query, double swipePercentage = 0.9)
			{
				_inner.SwipeRightToLeft(query, swipePercentage);
			}

			public void ScrollUp(Func<AppQuery, AppQuery> query = null, ScrollStrategy strategy = ScrollStrategy.Auto)
			{
				_inner.ScrollUp(query, strategy);
			}

			public void ScrollUp(string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto)
			{
				_inner.ScrollUp(withinMarked, strategy);
			}

			public void ScrollDown(Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto)
			{
				_inner.ScrollDown(withinQuery, strategy);
			}

			public void ScrollDown(string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto)
			{
				_inner.ScrollDown(withinMarked, strategy);
			}

			public void ScrollTo(string toMarked, string withinMarked = null, ScrollStrategy strategy = ScrollStrategy.Auto, TimeSpan? timeout = null)
			{
				_inner.ScrollTo(toMarked, withinMarked, strategy, timeout);
			}

			public void ScrollUpTo(string toMarked, string withinMarked = null, ScrollStrategy strategy = ScrollStrategy.Auto, TimeSpan? timeout = null)
			{
				_inner.ScrollUpTo(toMarked, withinMarked, strategy, timeout);
			}

			public void ScrollUpTo(Func<AppQuery, AppWebQuery> toQuery, string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto, TimeSpan? timeout = null)
			{
				_inner.ScrollUpTo(toQuery, withinMarked, strategy, timeout);
			}

			public void ScrollDownTo(string toMarked, string withinMarked = null, ScrollStrategy strategy = ScrollStrategy.Auto, TimeSpan? timeout = null)
			{
				_inner.ScrollDownTo(toMarked, withinMarked, strategy, timeout);
			}

			public void ScrollDownTo(Func<AppQuery, AppWebQuery> toQuery, string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto, TimeSpan? timeout = null)
			{
				_inner.ScrollDownTo(toQuery, withinMarked, strategy, timeout);
			}

			public void ScrollUpTo(Func<AppQuery, AppQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto, TimeSpan? timeout = null)
			{
				_inner.ScrollUpTo(toQuery, withinQuery, strategy, timeout);
			}

			public void ScrollUpTo(Func<AppQuery, AppWebQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto, TimeSpan? timeout = null)
			{
				_inner.ScrollUpTo(toQuery, withinQuery, strategy, timeout);
			}

			public void ScrollDownTo(Func<AppQuery, AppQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto, TimeSpan? timeout = null)
			{
				_inner.ScrollDownTo(toQuery, withinQuery, strategy, timeout);
			}

			public void ScrollDownTo(Func<AppQuery, AppWebQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto, TimeSpan? timeout = null)
			{
				_inner.ScrollDownTo(toQuery, withinQuery, strategy, timeout);
			}
#else
			public void SwipeRightToLeft(double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
			{
				_inner.SwipeRightToLeft(swipePercentage, swipeSpeed, withInertia);
			}

			public void SwipeRightToLeft(string marked, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
			{
				_inner.SwipeRightToLeft(marked, swipePercentage, swipeSpeed, withInertia);
			}

			public void SwipeLeftToRight(Func<AppQuery, AppQuery> query, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
			{
				_inner.SwipeLeftToRight(query, swipePercentage, swipeSpeed, withInertia);
			}

			public void SwipeRightToLeft(Func<AppQuery, AppQuery> query, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
			{
				_inner.SwipeRightToLeft(query, swipePercentage, swipeSpeed, withInertia);
			}

			public void ScrollUp(Func<AppQuery, AppQuery> query = null, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500,
				bool withInertia = true)
			{
				_inner.ScrollUp(query, strategy, swipePercentage, swipeSpeed, withInertia);
			}

			public void ScrollUp(string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500,
				bool withInertia = true)
			{
				_inner.ScrollUp(withinMarked, strategy, swipePercentage, swipeSpeed, withInertia);
			}

			public void ScrollDown(Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
				int swipeSpeed = 500, bool withInertia = true)
			{
				_inner.ScrollDown(withinQuery, strategy, swipePercentage, swipeSpeed, withInertia);
			}

			public void ScrollDown(string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500,
				bool withInertia = true)
			{
				_inner.ScrollDown(withinMarked, strategy, swipePercentage, swipeSpeed, withInertia);
			}

			public void ScrollTo(string toMarked, string withinMarked = null, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
				int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
			{
				_inner.ScrollTo(toMarked, withinMarked, strategy, swipePercentage, swipeSpeed, withInertia, timeout);
			}

			public void ScrollUpTo(string toMarked, string withinMarked = null, ScrollStrategy strategy = ScrollStrategy.Auto,
				double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
			{
				_inner.ScrollUpTo(toMarked, withinMarked, strategy, swipePercentage, swipeSpeed, withInertia, timeout);
			}

			public void ScrollUpTo(Func<AppQuery, AppWebQuery> toQuery, string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
				int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
			{
				_inner.ScrollUpTo(toQuery, withinMarked, strategy, swipePercentage, swipeSpeed, withInertia, timeout);
			}

			public void ScrollDownTo(string toMarked, string withinMarked = null, ScrollStrategy strategy = ScrollStrategy.Auto,
				double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
			{
				_inner.ScrollDownTo(toMarked, withinMarked, strategy, swipePercentage, swipeSpeed, withInertia, timeout);
			}

			public void ScrollDownTo(Func<AppQuery, AppWebQuery> toQuery, string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
				int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
			{
				_inner.ScrollDownTo(toQuery, withinMarked, strategy, swipePercentage, swipeSpeed, withInertia, timeout);
			}

			public void ScrollUpTo(Func<AppQuery, AppQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
				int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
			{
				_inner.ScrollUpTo(toQuery, withinQuery, strategy, swipePercentage, swipeSpeed, withInertia, timeout);
			}

			public void ScrollUpTo(Func<AppQuery, AppWebQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
				int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
			{
				_inner.ScrollUpTo(toQuery, withinQuery, strategy, swipePercentage, swipeSpeed, withInertia, timeout);
			}

			public void ScrollDownTo(Func<AppQuery, AppQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
				int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
			{
				_inner.ScrollDownTo(toQuery, withinQuery, strategy, swipePercentage, swipeSpeed, withInertia, timeout);
			}

			public void ScrollDownTo(Func<AppQuery, AppWebQuery> toQuery, Func<AppQuery, AppQuery> withinQuery = null, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67,
				int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = null)
			{
				_inner.ScrollDownTo(toQuery, withinQuery, strategy, swipePercentage, swipeSpeed, withInertia, timeout);
			}
#endif

			public void SetOrientationPortrait()
			{
				_inner.SetOrientationPortrait();
			}

			public void SetOrientationLandscape()
			{
				_inner.SetOrientationLandscape();
			}

			public void Repl()
			{
				_inner.Repl();
			}

			public void Back()
			{
				_inner.Back();
			}

			public void PressVolumeUp()
			{
				_inner.PressVolumeUp();
			}

			public void PressVolumeDown()
			{
				_inner.PressVolumeDown();
			}

			public object Invoke(string methodName, object argument = null)
			{
				return _inner.Invoke(methodName, argument);
			}

			public object Invoke(string methodName, object[] arguments)
			{
				return _inner.Invoke(methodName, arguments);
			}

			public void DragCoordinates(float fromX, float fromY, float toX, float toY)
			{
				_inner.DragCoordinates(fromX, fromY, toX, toY);
			}

#if !__WINDOWS__
			public void DragAndDrop(Func<AppQuery, AppQuery> @from, Func<AppQuery, AppQuery> to)
			{
				_inner.DragAndDrop(@from, to);
			}

			public void DragAndDrop(string @from, string to)
			{
				_inner.DragAndDrop(@from, to);
			}
#endif

			public void SetSliderValue(string marked, double value)
			{
				_inner.SetSliderValue(marked, value);
			}

			public void SetSliderValue(Func<AppQuery, AppQuery> query, double value)
			{
				_inner.SetSliderValue(query, value);
			}

			public AppPrintHelper Print => _inner.Print;

			public IDevice Device => _inner.Device;

			public ITestServer TestServer => _inner.TestServer;
		}

		static IApp InitializeApp ()
		{
			IApp app = null;
#if __ANDROID__
			app = ConfigureApp.Android.ApkFile (AppPaths.ApkPath).Debug ().StartApp ();
#elif __IOS__
			app = ConfigureApp.iOS.InstalledApp (AppPaths.BundleId).Debug ()
				//Uncomment to run from a specific iOS SIM, get the ID from XCode -> Devices
				//.DeviceIdentifier("55555555-5555-5555-5555-555555555555")
				.StartApp ();
#endif
			if (app == null)
				throw new NullReferenceException ("App was not initialized.");

			return new WrapperApp(app);
		}

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

			app.Tap (q => q.Button ("Go to Test Cases"));
			app.WaitForElement (q => q.Raw ("* marked:'TestCasesIssueList'"));

			app.EnterText (q => q.Raw ("* marked:'SearchBarGo'"), cellName);

			app.WaitForElement (q => q.Raw ("* marked:'SearchButton'"));
			app.Tap (q => q.Raw ("* marked:'SearchButton'"));
		}

		public static IApp Setup (Type pageType = null)
		{
			IApp runningApp = null;
			try {
				runningApp = InitializeApp ();
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
