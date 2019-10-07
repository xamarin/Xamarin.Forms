using System;
using System.ComponentModel;
using Android.Content;
using Android.Webkit;
using Android.OS;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Internals;
using MixedContentHandling = Android.Webkit.MixedContentHandling;
using AWebView = Android.Webkit.WebView;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.Android
{
	public class WebViewRenderer : ViewRenderer<WebView, AWebView>, IWebViewDelegate
	{
		public const string AssetBaseUrl = "file:///android_asset/";

		WebNavigationEvent _eventState;
		WebViewClient _webViewClient;
		FormsWebChromeClient _webChromeClient;
		bool _isDisposed = false;
		protected internal IWebViewController ElementController => Element;
		protected internal bool IgnoreSourceChanges { get; set; }
		protected internal string UrlCanceled { get; set; }

		public WebViewRenderer(Context context) : base(context)
		{
			AutoPackage = false;			
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use WebViewRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public WebViewRenderer()
		{
			AutoPackage = false;
		}

		public void LoadHtml(string html, string baseUrl)
		{
			_eventState = WebNavigationEvent.NewPage;
			Control.LoadDataWithBaseURL(baseUrl ?? AssetBaseUrl, html, "text/html", "UTF-8", null);
		}

		public void LoadUrl(string url)
		{
			if (!SendNavigatingCanceled(url))
			{
				_eventState = WebNavigationEvent.NewPage;
				Control.LoadUrl(url);
			}	
		}

		protected internal bool SendNavigatingCanceled(string url)
		{
			if (Element == null || string.IsNullOrWhiteSpace(url))
				return true;

			if (url == AssetBaseUrl)
				return false;

			var args = new WebNavigatingEventArgs(_eventState, new UrlWebViewSource { Url = url }, url);
			ElementController.SendNavigating(args);
			UpdateCanGoBackForward();
			UrlCanceled = args.Cancel ? null : url;
			return args.Cancel;
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			_isDisposed = true;
			if (disposing)
			{
				if (Element != null)
				{
					Control?.StopLoading();

					ElementController.EvalRequested -= OnEvalRequested;
					ElementController.GoBackRequested -= OnGoBackRequested;
					ElementController.GoForwardRequested -= OnGoForwardRequested;
					ElementController.ReloadRequested -= OnReloadRequested;
					ElementController.EvaluateJavaScriptRequested -= OnEvaluateJavaScriptRequested;

					_webViewClient?.Dispose();
					_webChromeClient?.Dispose();
				}
			}

			base.Dispose(disposing);
		}

		protected virtual WebViewClient GetWebViewClient()
		{
			return new FormsWebViewClient(this);
		}

		protected virtual FormsWebChromeClient GetFormsWebChromeClient()
		{
			return new FormsWebChromeClient();
		}

		protected override Size MinimumSize()
		{
			return new Size(Context.ToPixels(40), Context.ToPixels(40));
		}

		protected override AWebView CreateNativeControl()
		{
			return new AWebView(Context);
		}

		internal WebNavigationEvent GetCurrentWebNavigationEvent()
		{
			return _eventState;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				var webView = CreateNativeControl();
#pragma warning disable 618 // This can probably be replaced with LinearLayout(LayoutParams.MatchParent, LayoutParams.MatchParent); just need to test that theory
				webView.LayoutParameters = new global::Android.Widget.AbsoluteLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent, 0, 0);
#pragma warning restore 618

				_webViewClient = GetWebViewClient();
				webView.SetWebViewClient(_webViewClient);

				_webChromeClient = GetFormsWebChromeClient();
				_webChromeClient.SetContext(Context);
				webView.SetWebChromeClient(_webChromeClient);

				if(Context.IsDesignerContext())
				{
					SetNativeControl(webView);
					return;
				}

				webView.Settings.JavaScriptEnabled = true;
				webView.Settings.DomStorageEnabled = true;
				SetNativeControl(webView);
			}

			if (e.OldElement != null)
			{
				var oldElementController = e.OldElement as IWebViewController;
				oldElementController.EvalRequested -= OnEvalRequested;
				oldElementController.EvaluateJavaScriptRequested -= OnEvaluateJavaScriptRequested;
				oldElementController.GoBackRequested -= OnGoBackRequested;
				oldElementController.GoForwardRequested -= OnGoForwardRequested;
				oldElementController.ReloadRequested -= OnReloadRequested;
			}

			if (e.NewElement != null)
			{
				var newElementController = e.NewElement as IWebViewController;
				newElementController.EvalRequested += OnEvalRequested;
				newElementController.EvaluateJavaScriptRequested += OnEvaluateJavaScriptRequested;
				newElementController.GoBackRequested += OnGoBackRequested;
				newElementController.GoForwardRequested += OnGoForwardRequested;
				newElementController.ReloadRequested += OnReloadRequested;

				UpdateMixedContentMode();
				UpdateEnableZoomControls();
				UpdateDisplayZoomControls();
			}

			Load();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			switch (e.PropertyName)
			{
				case "Source":
					Load();
					break;
				case "MixedContentMode":
					UpdateMixedContentMode();
					break;
				case "EnableZoomControls":
					UpdateEnableZoomControls();
					break;
				case "DisplayZoomControls":
					UpdateDisplayZoomControls();
					break;
			}
		}

		void Load()
		{
			if (IgnoreSourceChanges)
				return;

			Element.Source?.Load(this);

			UpdateCanGoBackForward();
		}

		void OnEvalRequested(object sender, EvalRequested eventArg)
		{
			LoadUrl("javascript:" + eventArg.Script);
		}

		async Task<string> OnEvaluateJavaScriptRequested(string script)
		{
			var jsr = new JavascriptResult();

			Control.EvaluateJavascript(script, jsr);

			return await jsr.JsResult.ConfigureAwait(false);
		}

		void OnGoBackRequested(object sender, EventArgs eventArgs)
		{
			if (Control.CanGoBack())
			{
				_eventState = WebNavigationEvent.Back;
				Control.GoBack();
			}	

			UpdateCanGoBackForward();
		}

		void OnGoForwardRequested(object sender, EventArgs eventArgs)
		{
			if (Control.CanGoForward())
			{
				_eventState = WebNavigationEvent.Forward;
				Control.GoForward();
			}	

			UpdateCanGoBackForward();
		}

		void OnReloadRequested(object sender, EventArgs eventArgs)
		{
			_eventState = WebNavigationEvent.Refresh;
			Control.Reload();
		}

		protected internal void UpdateCanGoBackForward()
		{
			if (Element == null || Control == null)
				return;
			ElementController.CanGoBack = Control.CanGoBack();
			ElementController.CanGoForward = Control.CanGoForward();
		}

		void UpdateMixedContentMode()
		{
			if (Control != null && ((int)Forms.SdkInt >= 21))
			{
				Control.Settings.MixedContentMode = (MixedContentHandling)Element.OnThisPlatform().MixedContentMode();
			}
		}

		void UpdateEnableZoomControls()
		{
			var value = Element.OnThisPlatform().ZoomControlsEnabled();
			Control.Settings.SetSupportZoom(value);
			Control.Settings.BuiltInZoomControls = value;
		}

		void UpdateDisplayZoomControls()
		{
			Control.Settings.DisplayZoomControls = Element.OnThisPlatform().ZoomControlsDisplayed();
		}

		class JavascriptResult : Java.Lang.Object, IValueCallback
		{
			TaskCompletionSource<string> source;
			public Task<string> JsResult { get { return source.Task; } }

			public JavascriptResult()
			{
				source = new TaskCompletionSource<string>();
			}

			public void OnReceiveValue(Java.Lang.Object result)
			{
				string json = ((Java.Lang.String)result).ToString();
				source.SetResult(json);
			}
		}
	}
}