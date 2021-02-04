using System;
using System.IO;
using Foundation;
using UIKit;
using WebKit;
using Xamarin.Forms;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Platform.Handlers
{
	public partial class WebViewHandler : AbstractViewHandler<IWebView, WKWebView>, IWebViewDelegate2
	{
		static WKProcessPool? SharedPool;
		static bool FirstLoadFinished;
		static string? PendingUrl;

		protected override WKWebView CreateNativeView()
		{
			return new WKWebView(RectangleF.Empty, CreateConfiguration())
			{
				UIDelegate = new CustomWebViewUIDelegate(),
				BackgroundColor = UIColor.Clear,
				AutosizesSubviews = true
			};
		}

		protected override void ConnectHandler(WKWebView nativeView)
		{
			FirstLoadFinished = true;

			if (!string.IsNullOrWhiteSpace(PendingUrl))
			{
				var closure = PendingUrl;
				PendingUrl = null;

				// I realize this looks like the worst hack ever but iOS 11 and cookies are super quirky
				// and this is the only way I could figure out how to get iOS 11 to inject a cookie 
				// the first time a WkWebView is used in your app. This only has to run the first time a WkWebView is used 
				// anywhere in the application. All subsequents uses of WkWebView won't hit this hack
				// Even if it's a WkWebView on a new page.
				// read through this thread https://developer.apple.com/forums/thread/99674
				// Or Bing "WkWebView and Cookies" to see the myriad of hacks that exist
				// Most of them all came down to different variations of synching the cookies before or after the
				// WebView is added to the controller. This is the only one I was able to make work
				// I think if we could delay adding the WebView to the Controller until after ViewWillAppear fires that might also work
				// But we're not really setup for that
				// If you'd like to try your hand at cleaning this up then UI Test Issue12134 and Issue3262 are your final bosses

				// TODO: Invoke on MainThread 
				LoadUrl(closure);
			}
		}

		protected override void SetupDefaults(WKWebView nativeView)
		{
			FirstLoadFinished = false;
		}

		// https://developer.apple.com/forums/thread/99674
		// WKWebView and making sure cookies synchronize is really quirky
		// The main workaround I've found for ensuring that cookies synchronize 
		// is to share the Process Pool between all WkWebView instances.
		// It also has to be shared at the point you call init
		static WKWebViewConfiguration CreateConfiguration()
		{
			var config = new WKWebViewConfiguration();

			if (SharedPool == null)
				SharedPool = config.ProcessPool;
			else
				config.ProcessPool = SharedPool;
			
			return config;
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			SetDesiredSize(widthConstraint, heightConstraint);

			return base.GetDesiredSize(widthConstraint, heightConstraint);
		}

		public static void MapSource(WebViewHandler handler, IWebView webView)
		{
			ViewHandler.CheckParameters(handler, webView);

			IWebViewDelegate2 webViewDelegate = handler;

			handler.TypedNativeView?.UpdateSource(webView, webViewDelegate);
		}

		public static void MapCookies(WebViewHandler handler, IWebView webView)
		{
			ViewHandler.CheckParameters(handler, webView);

			handler.TypedNativeView?.UpdateCookies(webView);
		}

		public static void MapCanGoBack(WebViewHandler handler, IWebView webView)
		{
			ViewHandler.CheckParameters(handler, webView);

			handler.TypedNativeView?.UpdateCanGoBack(webView);
		}

		public static void MapCanGoForward(WebViewHandler handler, IWebView webView)
		{
			ViewHandler.CheckParameters(handler, webView);

			handler.TypedNativeView?.UpdateCanGoForward(webView);
		}

		public static void MapGoBack(WebViewHandler handler, IWebView webView)
		{
			ViewHandler.CheckParameters(handler, webView);

			handler.TypedNativeView?.UpdateGoBack(webView);
		}

		public static void MapGoForward(WebViewHandler handler, IWebView webView)
		{
			ViewHandler.CheckParameters(handler, webView);

			handler.TypedNativeView?.UpdateGoForward(webView);
		}

		public static void MapReload(WebViewHandler handler, IWebView webView)
		{
			ViewHandler.CheckParameters(handler, webView);

			handler.TypedNativeView?.UpdateReload(webView);
		}

		public static void MapEval(WebViewHandler handler, IWebView webView)
		{
			ViewHandler.CheckParameters(handler, webView);

			handler.TypedNativeView?.UpdateEval(webView);
		}

		public static void MapEvaluateJavaScript(WebViewHandler handler, IWebView webView)
		{
			ViewHandler.CheckParameters(handler, webView);

			handler.TypedNativeView?.UpdateEvaluateJavaScript(webView);
		}

		public void LoadHtml(string? html, string? baseUrl)
		{
			if (html != null)
				TypedNativeView?.LoadHtmlString(html, baseUrl == null ? new NSUrl(NSBundle.MainBundle.BundlePath, true) : new NSUrl(baseUrl, true));
		}

		public async void LoadUrl(string? url)
		{
			try
			{
				if (TypedNativeView == null)
					return;

				var uri = new Uri(url);
				var safeHostUri = new Uri($"{uri.Scheme}://{uri.Authority}", UriKind.Absolute);
				var safeRelativeUri = new Uri($"{uri.PathAndQuery}{uri.Fragment}", UriKind.Relative);
				NSUrlRequest request = new NSUrlRequest(new Uri(safeHostUri, safeRelativeUri));

				bool hasCookiesToLoad = url != null && TypedNativeView.HasCookiesToLoad(VirtualView, url);

				if (!FirstLoadFinished && hasCookiesToLoad && !NativeVersion.IsAtLeast(13))
				{
					PendingUrl = url;
					return;
				}

				FirstLoadFinished = true;

				if (VirtualView != null && url != null)
					await TypedNativeView.SyncNativeCookies(VirtualView, url);

				TypedNativeView.LoadRequest(request);
			}
			catch (UriFormatException formatException)
			{
				// If we got a format exception trying to parse the URI, it might be because
				// someone is passing in a local bundled file page. If we can find a better way
				// to detect that scenario, we should use it; until then, we'll fall back to 
				// local file loading here and see if that works:
				if (!LoadFile(url))
				{
					Log.Warning(nameof(WebViewHandler), $"Unable to Load Url {url}: {formatException}");
				}
			}
			catch (Exception exc)
			{
				Log.Warning(nameof(WebViewHandler), $"Unable to Load Url {url}: {exc}");
			}
		}

		bool LoadFile(string? url)
		{
			try
			{
				var file = Path.GetFileNameWithoutExtension(url);
				var ext = Path.GetExtension(url);

				var nsUrl = NSBundle.MainBundle.GetUrlForResource(file, ext);

				if (nsUrl == null)
				{
					return false;
				}

				TypedNativeView?.LoadFileUrl(nsUrl, nsUrl);

				return true;
			}
			catch (Exception ex)
			{
				Log.Warning(nameof(WebViewHandler), $"Could not load {url} as local file: {ex}");
			}

			return false;
		}

		void SetDesiredSize(double width, double height)
		{
			if (TypedNativeView != null)
			{
				var x = TypedNativeView.Frame.X;
				var y = TypedNativeView.Frame.Y;

				TypedNativeView.Frame = new RectangleF(x, y, width, height);
			}
		}

		class CustomWebViewNavigationDelegate : WKNavigationDelegate
		{
			readonly WebViewHandler _handler;
			WebNavigationEvent _lastEvent;

			public CustomWebViewNavigationDelegate(WebViewHandler handler)
			{
				_handler = handler ?? throw new ArgumentNullException("handler");
			}

			IWebView? WebView => _handler.VirtualView;

			public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
			{
				_handler.TypedNativeView?.UpdateCanGoBackForward(WebView);
			}

			public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
			{
				_handler.TypedNativeView?.UpdateCanGoBackForward(WebView);
			}

			public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
			{
				if (webView.IsLoading)
					return;

				var url = GetCurrentUrl();

				if (url == $"file://{NSBundle.MainBundle.BundlePath}/")
					return;

				if (WebView != null)
					WebView.Source = new UrlWebViewSource2 { Url = url };

				ProcessNavigated(url);
			}

			async void ProcessNavigated(string url)
			{
				try
				{
					var nativeView = _handler?.TypedNativeView;

					if (nativeView != null && WebView != null && WebView.Cookies != null)
						await nativeView.SyncNativeCookiesToVirtualView(WebView, url);
				}
				catch (Exception exc)
				{
					Log.Warning(nameof(WebViewHandler), $"Failed to Sync Cookies {exc}");
				}

				_handler?.TypedNativeView?.UpdateCanGoBackForward(WebView);
			}

			public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
			{
			}

			// https://stackoverflow.com/questions/37509990/migrating-from-uiwebview-to-wkwebview
			public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
			{
				var navEvent = WebNavigationEvent.NewPage;
				var navigationType = navigationAction.NavigationType;
				switch (navigationType)
				{
					case WKNavigationType.LinkActivated:
						navEvent = WebNavigationEvent.NewPage;

						if (navigationAction.TargetFrame == null)
							webView?.LoadRequest(navigationAction.Request);

						break;
					case WKNavigationType.FormSubmitted:
						navEvent = WebNavigationEvent.NewPage;
						break;
					case WKNavigationType.BackForward:
						break;
					case WKNavigationType.Reload:
						navEvent = WebNavigationEvent.Refresh;
						break;
					case WKNavigationType.FormResubmitted:
						navEvent = WebNavigationEvent.NewPage;
						break;
					case WKNavigationType.Other:
						navEvent = WebNavigationEvent.NewPage;
						break;
				}

				_lastEvent = navEvent;
				_handler.TypedNativeView?.UpdateCanGoBackForward(WebView);
			}

			string GetCurrentUrl()
			{
				return _handler?.TypedNativeView?.Url?.AbsoluteUrl?.ToString() ?? string.Empty;
			}
		}

		class CustomWebViewUIDelegate : WKUIDelegate
		{
			static readonly string LocalOK = NSBundle.FromIdentifier("com.apple.UIKit").GetLocalizedString("OK");
			static readonly string LocalCancel = NSBundle.FromIdentifier("com.apple.UIKit").GetLocalizedString("Cancel");

			public override void RunJavaScriptAlertPanel(WKWebView webView, string message, WKFrameInfo frame, Action completionHandler)
			{
				PresentAlertController(
					webView,
					message,
					okAction: _ => completionHandler()
				);
			}

			public override void RunJavaScriptConfirmPanel(WKWebView webView, string message, WKFrameInfo frame, Action<bool> completionHandler)
			{
				PresentAlertController(
					webView,
					message,
					okAction: _ => completionHandler(true),
					cancelAction: _ => completionHandler(false)
				);
			}

			public override void RunJavaScriptTextInputPanel(
				WKWebView webView, string prompt, string? defaultText, WKFrameInfo frame, Action<string> completionHandler)
			{
				PresentAlertController(
					webView,
					prompt,
					defaultText: defaultText,
					okAction: x => completionHandler(x.TextFields[0].Text ?? string.Empty),
					cancelAction: _ => completionHandler(string.Empty)
				);
			}

			static string GetJsAlertTitle(WKWebView webView)
			{
				// Emulate the behavior of UIWebView dialogs.
				// The scheme and host are used unless local html content is what the webview is displaying,
				// in which case the bundle file name is used.

				if (webView.Url != null && webView.Url.AbsoluteString != $"file://{NSBundle.MainBundle.BundlePath}/")
					return $"{webView.Url.Scheme}://{webView.Url.Host}";

				return new NSString(NSBundle.MainBundle.BundlePath).LastPathComponent;
			}

			static UIAlertAction AddOkAction(UIAlertController controller, Action handler)
			{
				var action = UIAlertAction.Create(LocalOK, UIAlertActionStyle.Default, (_) => handler());
				controller.AddAction(action);
				controller.PreferredAction = action;
				return action;
			}

			static UIAlertAction AddCancelAction(UIAlertController controller, Action handler)
			{
				var action = UIAlertAction.Create(LocalCancel, UIAlertActionStyle.Cancel, (_) => handler());
				controller.AddAction(action);
				return action;
			}

			static void PresentAlertController(
				WKWebView webView,
				string message,
				string? defaultText = null,
				Action<UIAlertController>? okAction = null,
				Action<UIAlertController>? cancelAction = null)
			{
				var controller = UIAlertController.Create(GetJsAlertTitle(webView), message, UIAlertControllerStyle.Alert);

				if (defaultText != null)
					controller.AddTextField((textField) => textField.Text = defaultText);

				if (okAction != null)
					AddOkAction(controller, () => okAction(controller));

				if (cancelAction != null)
					AddCancelAction(controller, () => cancelAction(controller));

				GetTopViewController(UIApplication.SharedApplication.GetKeyWindow()?.RootViewController)?
					.PresentViewController(controller, true, null);
			}

			static UIViewController? GetTopViewController(UIViewController? viewController)
			{
				if (viewController is UINavigationController navigationController)
					return GetTopViewController(navigationController.VisibleViewController);

				if (viewController is UITabBarController tabBarController)
					return GetTopViewController(tabBarController.SelectedViewController);

				if (viewController?.PresentedViewController != null)
					return GetTopViewController(viewController.PresentedViewController);

				return viewController;
			}
		}
	}
}