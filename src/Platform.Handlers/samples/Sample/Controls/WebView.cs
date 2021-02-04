using System;
using System.Net;
using Xamarin.Forms;
using Xamarin.Platform;

namespace Sample
{
	public class WebView : Xamarin.Forms.View, IWebView
	{
		public WebViewSource2 Source { get; set; }
		public CookieContainer Cookies { get; set; }
		public bool CanGoBack { get; set; }
		public bool CanGoForward { get; set; }

		public new double Width
		{
			get { return WidthRequest; }
			set { WidthRequest = value; }
		}

		public new double Height
		{
			get { return HeightRequest; }
			set { HeightRequest = value; }
		}

		public Action GoBack { get; set; }
		public Action GoForward { get; set; }
		public Action Reload { get; set; }
		public Action<string> Eval { get; set; }
		public Action<string> EvaluateJavaScript { get; set; }

		void IWebView.GoBack()
		{
			GoBack?.Invoke();
		}

		void IWebView.GoForward()
		{
			GoForward?.Invoke();
		}

		void IWebView.Reload()
		{
			Reload?.Invoke();
		}

		void IWebView.Eval(string script)
		{
			Eval?.Invoke(script);
		}

		void IWebView.EvaluateJavaScript(string script)
		{
			EvaluateJavaScript?.Invoke(script);
		}
	}
}