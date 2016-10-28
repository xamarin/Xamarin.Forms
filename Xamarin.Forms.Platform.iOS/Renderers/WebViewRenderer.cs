using System;
using System.ComponentModel;
using System.Drawing;
using Foundation;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	public class WebViewRenderer : UIWebView, IVisualElementRenderer, IWebViewDelegate
	{
		EventTracker _events;
		bool _ignoreSourceChanges;
		WebNavigationEvent _lastBackForwardEvent;
		VisualElementPackager _packager;
#pragma warning disable 0414
		VisualElementTracker _tracker;
#pragma warning restore 0414
		public WebViewRenderer() : base(RectangleF.Empty)
		{
		}

		public VisualElement Element { get; private set; }

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return NativeView.GetSizeRequest(widthConstraint, heightConstraint, 44, 44);
		}

		public void SetElement(VisualElement element)
		{
			var oldElement = Element;
			Element = element;
			Element.PropertyChanged += HandlePropertyChanged;
			((WebView)Element).EvalRequested += OnEvalRequested;
			((WebView)Element).GoBackRequested += OnGoBackRequested;
			((WebView)Element).GoForwardRequested += OnGoForwardRequested;
			Delegate = new CustomWebViewDelegate(this);

			BackgroundColor = UIColor.Clear;

			AutosizesSubviews = true;

			_tracker = new VisualElementTracker(this);

			_packager = new VisualElementPackager(this);
			_packager.Load();

			_events = new EventTracker(this);
			_events.LoadEvents(this);

			Load();

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

			if (Element != null && !string.IsNullOrEmpty(Element.AutomationId))
				AccessibilityIdentifier = Element.AutomationId;

			if (element != null)
				element.SendViewInitialized(this);
		}

		public void SetElementSize(Size size)
		{
			Layout.LayoutChildIntoBoundingRegion(Element, new Rectangle(Element.X, Element.Y, size.Width, size.Height));
		}

		public void LoadHtml(string html, string baseUrl)
		{
			if (html != null)
				LoadHtmlString(html, baseUrl == null ? new NSUrl(NSBundle.MainBundle.BundlePath, true) : new NSUrl(baseUrl, true));
		}

		public void LoadUrl(string url)
		{
			LoadRequest(new NSUrlRequest(new NSUrl(url)));
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			// ensure that inner scrollview properly resizes when frame of webview updated
			ScrollView.Frame = Bounds;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (IsLoading)
					StopLoading();

				Element.PropertyChanged -= HandlePropertyChanged;
				((WebView)Element).EvalRequested -= OnEvalRequested;
				((WebView)Element).GoBackRequested -= OnGoBackRequested;
				((WebView)Element).GoForwardRequested -= OnGoForwardRequested;

				_tracker?.Dispose();
				_packager?.Dispose();
			}

			base.Dispose(disposing);
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			var changed = ElementChanged;
			if (changed != null)
				changed(this, e);
		}

		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == WebView.SourceProperty.PropertyName)
				Load();
		}

		void Load()
		{
			if (_ignoreSourceChanges)
				return;

			if (((WebView)Element).Source != null)
				((WebView)Element).Source.Load(this);

			UpdateCanGoBackForward();
		}

		void OnEvalRequested(object sender, EvalRequested eventArg)
		{
			EvaluateJavascript(eventArg.Script);
		}

		void OnGoBackRequested(object sender, EventArgs eventArgs)
		{
			if (CanGoBack)
			{
				_lastBackForwardEvent = WebNavigationEvent.Back;
				GoBack();
			}

			UpdateCanGoBackForward();
		}

		void OnGoForwardRequested(object sender, EventArgs eventArgs)
		{
			if (CanGoForward)
			{
				_lastBackForwardEvent = WebNavigationEvent.Forward;
				GoForward();
			}

			UpdateCanGoBackForward();
		}

		void UpdateCanGoBackForward()
		{
			((WebView)Element).CanGoBack = CanGoBack;
			((WebView)Element).CanGoForward = CanGoForward;
		}

		class CustomWebViewDelegate : UIWebViewDelegate
		{
			readonly WebViewRenderer _renderer;
			WebNavigationEvent _lastEvent;

			public CustomWebViewDelegate(WebViewRenderer renderer)
			{
				if (renderer == null)
					throw new ArgumentNullException("renderer");
				_renderer = renderer;
			}

			WebView WebView
			{
				get { return (WebView)_renderer.Element; }
			}

			public override void LoadFailed(UIWebView webView, NSError error)
			{
				var url = GetCurrentUrl();
				WebView.SendNavigated(new WebNavigatedEventArgs(_lastEvent, new UrlWebViewSource { Url = url }, url, WebNavigationResult.Failure));

				_renderer.UpdateCanGoBackForward();
			}

			public override void LoadingFinished(UIWebView webView)
			{
				if (webView.IsLoading)
					return;

				_renderer._ignoreSourceChanges = true;
				var url = GetCurrentUrl();
				((IElementController)WebView).SetValueFromRenderer(WebView.SourceProperty, new UrlWebViewSource { Url = url });
				_renderer._ignoreSourceChanges = false;

				var args = new WebNavigatedEventArgs(_lastEvent, WebView.Source, url, WebNavigationResult.Success);
				WebView.SendNavigated(args);

				_renderer.UpdateCanGoBackForward();
			}

			public override void LoadStarted(UIWebView webView)
			{
			}

			public override bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
			{
				var navEvent = WebNavigationEvent.NewPage;
				switch (navigationType)
				{
					case UIWebViewNavigationType.LinkClicked:
						navEvent = WebNavigationEvent.NewPage;
						break;
					case UIWebViewNavigationType.FormSubmitted:
						navEvent = WebNavigationEvent.NewPage;
						break;
					case UIWebViewNavigationType.BackForward:
						navEvent = _renderer._lastBackForwardEvent;
						break;
					case UIWebViewNavigationType.Reload:
						navEvent = WebNavigationEvent.Refresh;
						break;
					case UIWebViewNavigationType.FormResubmitted:
						navEvent = WebNavigationEvent.NewPage;
						break;
					case UIWebViewNavigationType.Other:
						navEvent = WebNavigationEvent.NewPage;
						break;
				}

				_lastEvent = navEvent;
				var lastUrl = request.Url.ToString();
				var args = new WebNavigatingEventArgs(navEvent, new UrlWebViewSource { Url = lastUrl }, lastUrl);

				WebView.SendNavigating(args);
				_renderer.UpdateCanGoBackForward();
				return !args.Cancel;
			}

			string GetCurrentUrl()
			{
				return _renderer?.Request?.Url?.AbsoluteUrl?.ToString();
			}
		}

		#region IPlatformRenderer implementation

		public UIView NativeView
		{
			get { return this; }
		}

		public UIViewController ViewController
		{
			get { return null; }
		}

		#endregion
	}
}