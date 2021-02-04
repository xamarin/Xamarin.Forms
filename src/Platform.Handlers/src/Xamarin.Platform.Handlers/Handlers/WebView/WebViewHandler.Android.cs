using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Webkit;
using Android.Widget;
using Xamarin.Forms;
using static Android.Views.ViewGroup;
using AWebView = Android.Webkit.WebView;

namespace Xamarin.Platform.Handlers
{
	public partial class WebViewHandler : AbstractViewHandler<IWebView, AWebView>, IWebViewDelegate2
	{
		public const string AssetBaseUrl = "file:///android_asset/";

		WebViewClient? _webViewClient;
		WebChromeClient? _webChromeClient;

		protected internal bool IgnoreSourceChanges { get; set; }
		protected internal string? UrlCanceled { get; set; }

		protected override AWebView CreateNativeView()
		{
			var aWebView = new AWebView(Context)
			{
#pragma warning disable 618 // This can probably be replaced with LinearLayout(LayoutParams.MatchParent, LayoutParams.MatchParent); just need to test that theory
				LayoutParameters = new AbsoluteLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent, 0, 0)
#pragma warning restore 618
			};

			if (aWebView.Settings != null)
			{
				aWebView.Settings.JavaScriptEnabled = true;
				aWebView.Settings.DomStorageEnabled = true;
			}

			_webViewClient = GetWebViewClient();
			aWebView.SetWebViewClient(_webViewClient);

			_webChromeClient = GetWebChromeClient();
			aWebView.SetWebChromeClient(_webChromeClient);

			return aWebView;
		}

		protected override void DisconnectHandler(AWebView nativeView)
		{
			nativeView.StopLoading();
			_webViewClient?.Dispose();
			_webChromeClient?.Dispose();
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
			TypedNativeView?.LoadDataWithBaseURL(baseUrl ?? AssetBaseUrl, html, "text/html", "UTF-8", null);
		}

		public void LoadUrl(string? url)
		{
			TypedNativeView?.LoadUrl(url);
		}

		protected virtual WebViewClient GetWebViewClient()
		{
			return new CustomWebViewClient(this);
		}

		protected virtual WebChromeClient GetWebChromeClient()
		{
			return new CustomWebChromeClient();
		}

		public class JavascriptResult : Java.Lang.Object, IValueCallback
		{
			readonly TaskCompletionSource<string> _source;

			public Task<string> JsResult { get { return _source.Task; } }

			public JavascriptResult()
			{
				_source = new TaskCompletionSource<string>();
			}

			public void OnReceiveValue(Java.Lang.Object? result)
			{
				if (result != null)
				{
					string json = ((Java.Lang.String)result).ToString();
					_source.SetResult(json);
				}
			}
		}

		public class CustomWebViewClient : WebViewClient
		{
			WebNavigationResult _navigationResult = WebNavigationResult.Success;
			WebViewHandler? _handler;
			string? _lastUrlNavigatedCancel;

			public CustomWebViewClient(WebViewHandler handler)
				=> _handler = handler ?? throw new ArgumentNullException("handler");

			protected CustomWebViewClient(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
			{
			}

			IWebView? WebView => _handler?.VirtualView;

			public override bool ShouldOverrideUrlLoading(AWebView? view, IWebResourceRequest? request)
				=> SendNavigatingCanceled(request?.Url?.ToString());

			public override void OnPageStarted(AWebView? view, string? url, Bitmap? favicon)
			{
				if (_handler?.VirtualView == null || string.IsNullOrWhiteSpace(url) || url == AssetBaseUrl)
					return;

				if (WebView != null)
					_handler.TypedNativeView?.SyncNativeCookiesToVirtualView(WebView, url!);

				var cancel = false;

				if (url != null && !url.Equals(_handler.UrlCanceled, StringComparison.OrdinalIgnoreCase))
					cancel = SendNavigatingCanceled(url);

				_handler.UrlCanceled = null;

				if (cancel)
				{
					_navigationResult = WebNavigationResult.Cancel;
					view?.StopLoading();
				}
				else
				{
					_navigationResult = WebNavigationResult.Success;
					base.OnPageStarted(view, url, favicon);
				}
			}

			public override void OnPageFinished(AWebView? view, string? url)
			{
				if (_handler?.VirtualView == null || url == AssetBaseUrl)
					return;

				var source = new UrlWebViewSource2 { Url = url };
				_handler.IgnoreSourceChanges = true;
				_handler.VirtualView.Source = source;
				_handler.IgnoreSourceChanges = false;

				bool navigate = _navigationResult != WebNavigationResult.Failure || (url != null && !url.Equals(_lastUrlNavigatedCancel, StringComparison.OrdinalIgnoreCase));
				_lastUrlNavigatedCancel = _navigationResult == WebNavigationResult.Cancel ? url : null;

				if (WebView != null)
				{
					if (navigate && url != null)
						_handler.TypedNativeView?.SyncNativeCookiesToVirtualView(WebView, url);

					_handler.TypedNativeView?.UpdateCanGoBackForward(WebView);
				}

				base.OnPageFinished(view, url);
			}

			public override void OnReceivedError(AWebView? view, IWebResourceRequest? request, WebResourceError? error)
			{
				if (request?.Url?.ToString() == _handler?.TypedNativeView?.Url)
				{
					_navigationResult =  WebNavigationResult.Failure;

					if (error?.ErrorCode == ClientError.Timeout)
						_navigationResult = WebNavigationResult.Timeout;
				}

				base.OnReceivedError(view, request, error);
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);

				if (disposing)
					_handler = null;
			}

			bool SendNavigatingCanceled(string? url)
			{
				// TODO: Send Navigating Canceled

				return true;
			}
		}

		public class CustomWebChromeClient : WebChromeClient
		{
			Activity? _activity;
			List<int>? _requestCodes;

			public override bool OnShowFileChooser(AWebView? webView, IValueCallback? filePathCallback, FileChooserParams? fileChooserParams)
			{
				base.OnShowFileChooser(webView, filePathCallback, fileChooserParams);
				return ChooseFile(filePathCallback, fileChooserParams?.CreateIntent(), fileChooserParams?.Title);
			}

			public void UnregisterCallbacks()
			{
				if (_requestCodes == null || _requestCodes.Count == 0 || _activity == null)
					return;

				// TODO: Port ActivityResultCallbackRegistry.

				_requestCodes = null;
			}

			protected bool ChooseFile(IValueCallback? filePathCallback, Intent? intent, string? title)
			{
				if (_activity == null)
					return false;

				Action<Result, Intent> callback = (resultCode, intentData) =>
				{
					if (filePathCallback == null)
						return;

					Java.Lang.Object? result = ParseResult(resultCode, intentData);
					filePathCallback.OnReceiveValue(result);
				};

				_requestCodes ??= new List<int>();

				// TODO: Port ActivityResultCallbackRegistry.
				int newRequestCode = 0;

				_requestCodes.Add(newRequestCode);

				_activity.StartActivityForResult(Intent.CreateChooser(intent, title), newRequestCode);

				return true;
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
					UnregisterCallbacks();
				base.Dispose(disposing);
			}

			protected virtual Java.Lang.Object? ParseResult(Result resultCode, Intent data)
			{
				return FileChooserParams.ParseResult((int)resultCode, data);
			}

			internal void SetContext(Context thisActivity)
			{
				_activity = thisActivity as Activity;

				if (_activity == null)
					Log.Warning(nameof(WebViewHandler), $"Failed to set the activity of the WebChromeClient, can't show pickers on the Webview");
			}
		}
	}
}