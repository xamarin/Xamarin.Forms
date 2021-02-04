using System;
using System.Collections.Generic;
using System.Net;
using Android.Webkit;
using Xamarin.Forms;
using AWebView = Android.Webkit.WebView;

namespace Xamarin.Platform
{
	public static class WebViewExtensions
	{
		static HashSet<string> LoadedCookies = new HashSet<string>();

		public static void UpdateSource(this AWebView nativeWebView, IWebView webView)
		{
			nativeWebView.UpdateSource(webView, null);
		}

		public static void UpdateSource(this AWebView nativeWebView, IWebView webView, IWebViewDelegate2? webViewDelegate)
		{
			if (webViewDelegate != null)
				webView.Source.Load(webViewDelegate);

			nativeWebView.UpdateCanGoBackForward(webView);
		}

		public static void UpdateCookies(this AWebView nativeWebView, IWebView webView)
		{

		}

		public static void UpdateCanGoBack(this AWebView nativeWebView, IWebView webView)
		{
			webView.CanGoBack = nativeWebView.CanGoBack();
		}

		public static void UpdateCanGoForward(this AWebView nativeWebView, IWebView webView)
		{
			webView.CanGoForward = nativeWebView.CanGoForward();
		}

		public static void UpdateGoBack(this AWebView nativeWebView, IWebView webView)
		{
			if (nativeWebView.CanGoBack())
				nativeWebView.GoBack();
			
			nativeWebView.UpdateCanGoBackForward(webView);
		}

		public static void UpdateGoForward(this AWebView nativeWebView, IWebView webView)
		{
			if (nativeWebView.CanGoForward())
				nativeWebView.GoForward();
			
			nativeWebView.UpdateCanGoBackForward(webView);
		}

		public static void UpdateReload(this AWebView nativeWebView, IWebView webView)
		{
			nativeWebView.SyncNativeCookies(webView, nativeWebView.Url?.ToString() ?? string.Empty);
			nativeWebView.Reload();
		}

		public static void UpdateEval(this AWebView nativeWebView, IWebView webView)
		{
			nativeWebView.UpdateEval(webView, string.Empty);
		}

		public static void UpdateEval(this AWebView nativeWebView, IWebView webView, string script)
		{

		}

		public static void UpdateEvaluateJavaScript(this AWebView nativeWebView, IWebView webView)
		{
			nativeWebView.UpdateEvaluateJavaScript(webView, string.Empty);
		}

		public static void UpdateEvaluateJavaScript(this AWebView nativeWebView, IWebView webView, string script)
		{

		}

		internal static void UpdateCanGoBackForward(this AWebView nativeWebView, IWebView webView)
		{
			nativeWebView.UpdateCanGoBack(webView);
			nativeWebView.UpdateCanGoForward(webView);
		}

		internal static void SyncNativeCookiesToVirtualView(this AWebView nativeWebView, IWebView webView, string url)
		{
			var myCookieJar = webView.Cookies;

			if (myCookieJar == null)
				return;

			var uri = CreateUriForCookies(url);

			if (uri == null)
				return;

			var cookies = myCookieJar.GetCookies(uri);
			var retrieveCurrentWebCookies = GetCookiesFromNativeStore(url);

			foreach (Cookie cookie in cookies)
			{
				var nativeCookie = retrieveCurrentWebCookies[cookie.Name];
				if (nativeCookie == null)
					cookie.Expired = true;
				else
					cookie.Value = nativeCookie.Value;
			}

			nativeWebView.SyncNativeCookies(webView, url);
		}

		internal static void SyncNativeCookies(this AWebView nativeWebView, IWebView webView, string url)
		{
			var uri = CreateUriForCookies(url);

			if (uri == null)
				return;

			var myCookieJar = webView.Cookies;

			if (myCookieJar == null)
				return;

			nativeWebView.InitialCookiePreloadIfNecessary(webView, url);

			var cookies = myCookieJar.GetCookies(uri);

			if (cookies == null)
				return;

			var retrieveCurrentWebCookies = GetCookiesFromNativeStore(url);

			var cookieManager = CookieManager.Instance;
			cookieManager?.SetAcceptCookie(true);

			for (var i = 0; i < cookies.Count; i++)
			{
				var cookie = cookies[i];
				var cookieString = cookie.ToString();
				cookieManager?.SetCookie(cookie.Domain, cookieString);
			}

			foreach (Cookie cookie in retrieveCurrentWebCookies)
			{
				if (cookies[cookie.Name] != null)
					continue;

				var cookieString = $"{cookie.Name}=; max-age=0;expires=Sun, 31 Dec 2017 00:00:00 UTC";
				cookieManager?.SetCookie(cookie.Domain, cookieString);
			}
		}

		internal static void InitialCookiePreloadIfNecessary(this AWebView nativeWebView, IWebView webView, string url)
		{
			if (nativeWebView == null)
				return;

			var myCookieJar = webView.Cookies;

			if (myCookieJar == null)
				return;

			var uri = CreateUriForCookies(url);
			if (uri == null)
				return;

			if (!LoadedCookies.Add(uri.Host))
				return;

			var cookies = myCookieJar.GetCookies(uri);

			if (cookies != null)
			{
				var existingCookies = GetCookiesFromNativeStore(url);
				foreach (Cookie cookie in existingCookies)
				{
					if (cookies[cookie.Name] == null)
						myCookieJar.Add(cookie);
				}
			}
		}

		internal static Uri? CreateUriForCookies(string url)
		{
			if (url == null)
				return null;

			if (url.Length > 2000)
				url = url.Substring(0, 2000);

			if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
			{
				if (string.IsNullOrWhiteSpace(uri.Host))
					return null;

				return uri;
			}

			return null;
		}

		internal static CookieCollection GetCookiesFromNativeStore(string url)
		{
			CookieContainer existingCookies = new CookieContainer();
			var cookieManager = CookieManager.Instance;
			var currentCookies = cookieManager?.GetCookie(url);
			var uri = CreateUriForCookies(url);

			if (currentCookies != null)
			{
				foreach (var cookie in currentCookies.Split(';'))
					existingCookies.SetCookies(uri, cookie);
			}

			return existingCookies.GetCookies(uri);
		}
	}
}
