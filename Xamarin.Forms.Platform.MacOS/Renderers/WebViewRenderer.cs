using System;
using System.ComponentModel;
using AppKit;
using Foundation;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.MacOS
{
	public class WebViewRenderer : ViewRenderer<WebView, WebKit.WebView>, IWebViewDelegate
	{
		bool _disposed;
		bool _ignoreSourceChanges;
		WebNavigationEvent _lastBackForwardEvent;
		WebNavigationEvent _lastEvent;

		IElementController ElementController => Element as IElementController;

		void IWebViewDelegate.LoadHtml(string html, string baseUrl)
		{
			if (html != null)
				Control.MainFrame.LoadHtmlString(html, baseUrl == null ? new NSUrl(NSBundle.MainBundle.BundlePath, true) : new NSUrl(baseUrl, true));
		}

		void IWebViewDelegate.LoadUrl(string url)
		{
			Control.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl(url)));
		}

		protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WebKit.WebView
					{
						AutoresizingMask = NSViewResizingMask.WidthSizable,
						AutoresizesSubviews = true
					});
					Control.OnFinishedLoading += NSWebViewFinishedLoad;
					Control.OnFailedLoading += NSWebViewFailedLoadWithError;

					Element.EvalRequested += OnEvalRequested;
					Element.GoBackRequested += OnGoBackRequested;
					Element.GoForwardRequested += OnGoForwardRequested;

					BackgroundColor = NSColor.Clear;
				}
			}

			Load();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == WebView.SourceProperty.PropertyName)
				Load();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;
				Control.OnFinishedLoading -= NSWebViewFinishedLoad;
				Control.OnFailedLoading -= NSWebViewFailedLoadWithError;
				Element.EvalRequested -= OnEvalRequested;
				Element.GoBackRequested -= OnGoBackRequested;
				Element.GoForwardRequested -= OnGoForwardRequested;
			}
			base.Dispose(disposing);
		}


		void Load()
		{
			if (_ignoreSourceChanges)
				return;

			Element?.Source?.Load(this);

			UpdateCanGoBackForward();
		}

		void UpdateCanGoBackForward()
		{
			Element.CanGoBack = Control.CanGoBack();
			Element.CanGoForward = Control.CanGoForward();
		}

		void OnEvalRequested(object sender, EvalRequested eventArg)
		{
			EvaluateJavascript(eventArg.Script);
		}

		void OnGoBackRequested(object sender, EventArgs eventArgs)
		{
			if (Control.CanGoBack())
			{
				_lastBackForwardEvent = WebNavigationEvent.Back;
				Control.GoBack();
			}

			UpdateCanGoBackForward();
		}

		void OnGoForwardRequested(object sender, EventArgs eventArgs)
		{
			if (Control.CanGoForward())
			{
				_lastBackForwardEvent = WebNavigationEvent.Forward;
				Control.GoForward();
			}

			UpdateCanGoBackForward();
		}

		void NSWebViewFailedLoadWithError(object sender, WebKit.WebResourceErrorEventArgs e)
		{
			Element?.SendNavigated(new WebNavigatedEventArgs(_lastEvent, new UrlWebViewSource { Url = Control.MainFrameUrl }, Control.MainFrameUrl, WebNavigationResult.Failure));

			UpdateCanGoBackForward();
		}

		void NSWebViewFinishedLoad(object sender, WebKit.WebResourceCompletedEventArgs e)
		{
			if (Control.IsLoading)
				return;

			_ignoreSourceChanges = true;
			ElementController?.SetValueFromRenderer(WebView.SourceProperty, new UrlWebViewSource { Url = Control.MainFrameUrl });
			_ignoreSourceChanges = false;

			Element?.SendNavigated(new WebNavigatedEventArgs(_lastEvent, Element?.Source, Control.MainFrameUrl, WebNavigationResult.Success));

			UpdateCanGoBackForward();
		}

		void EvaluateJavascript(string script)
		{
			throw new NotImplementedException();
		}
	}
}
