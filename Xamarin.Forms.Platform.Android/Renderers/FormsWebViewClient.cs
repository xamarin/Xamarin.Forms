using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class FormsWebViewClient : WebViewClient
	{
		WebNavigationResult _navigationResult = WebNavigationResult.Success;
		WebViewRenderer _renderer;
		string _lastUrlNotOverridden;
		string _lastUrlNavigatedCancel;

		public FormsWebViewClient(WebViewRenderer renderer)
		{
			if (renderer == null)
				throw new ArgumentNullException("renderer");
			_renderer = renderer;
		}

		protected FormsWebViewClient(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{

		}

		public override bool ShouldOverrideUrlLoading(global::Android.Webkit.WebView view, IWebResourceRequest request)
		{
			// this event does not fire when the Source is explictly set in code
			_lastUrlNotOverridden = null;
			if (_renderer?.Element == null || request?.Url == null)
				return true;
			var url = request.Url.ToString();
			if (url == WebViewRenderer.AssetBaseUrl)
				return false;

			var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url }, url);
			_renderer.ElementController.SendNavigating(args);
			_lastUrlNotOverridden = args.Cancel ? null : url;

			return args.Cancel;
		}

		public override void OnPageStarted(global::Android.Webkit.WebView view, string url, Bitmap favicon)
		{
			if (_renderer?.Element == null || string.IsNullOrWhiteSpace(url) || url == WebViewRenderer.AssetBaseUrl)
				return;

			bool cancel = false;
			if (!url.Equals(_lastUrlNotOverridden, StringComparison.OrdinalIgnoreCase))
			{
				var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url }, url);
				_renderer.ElementController.SendNavigating(args);
				cancel = args.Cancel;
			}
			_lastUrlNotOverridden = null;
			_renderer.UpdateCanGoBackForward();
			if (cancel)
			{
				_navigationResult = WebNavigationResult.Cancel;
				_renderer.Control.StopLoading();
			}
			else
			{
				_navigationResult = WebNavigationResult.Success;
				base.OnPageStarted(view, url, favicon);
			}
		}

		public override void OnPageFinished(global::Android.Webkit.WebView view, string url)
		{
			if (_renderer?.Element == null || url == WebViewRenderer.AssetBaseUrl)
				return;

			var source = new UrlWebViewSource { Url = url };
			_renderer.IgnoreSourceChanges = true;
			_renderer.ElementController.SetValueFromRenderer(WebView.SourceProperty, source);
			_renderer.IgnoreSourceChanges = false;

			bool navigate = _navigationResult == WebNavigationResult.Failure ? !url.Equals(_lastUrlNavigatedCancel, StringComparison.OrdinalIgnoreCase) : true;
			_lastUrlNavigatedCancel = _navigationResult == WebNavigationResult.Cancel ? url : null;

			if (navigate)
			{
				var args = new WebNavigatedEventArgs(WebNavigationEvent.NewPage, source, url, _navigationResult);
				_renderer.ElementController.SendNavigated(args);
			}

			_renderer.UpdateCanGoBackForward();

			base.OnPageFinished(view, url);
		}

		[Obsolete("OnReceivedError is obsolete as of version 2.3.0. This method was deprecated in API level 23.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OnReceivedError(global::Android.Webkit.WebView view, ClientError errorCode, string description, string failingUrl)
		{
			_navigationResult = WebNavigationResult.Failure;
			if (errorCode == ClientError.Timeout)
				_navigationResult = WebNavigationResult.Timeout;
#pragma warning disable 618
			base.OnReceivedError(view, errorCode, description, failingUrl);
#pragma warning restore 618
		}

		public override void OnReceivedError(global::Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
		{
			_navigationResult = WebNavigationResult.Failure;
			if (error.ErrorCode == ClientError.Timeout)
				_navigationResult = WebNavigationResult.Timeout;
			base.OnReceivedError(view, request, error);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
				_renderer = null;
		}
	}
}