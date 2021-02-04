using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using WebKit;
using Xamarin.Forms;
using Xamarin.Platform.Handlers;

namespace Xamarin.Platform
{
	public static class WebViewExtensions
	{
		static HashSet<string> LoadedCookies = new HashSet<string>();

		public static void UpdateSource(this WKWebView nativeWebView, IWebView webView)
		{
			nativeWebView.UpdateSource(webView, null);
		}

		public static void UpdateSource(this WKWebView nativeWebView, IWebView webView, IWebViewDelegate2? webViewDelegate)
		{
			if (webViewDelegate != null)
				webView.Source.Load(webViewDelegate);

			nativeWebView.UpdateCanGoBackForward(webView);
		}

		public static void UpdateCookies(this WKWebView nativeWebView, IWebView webView)
		{

		}

		public static void UpdateCanGoBack(this WKWebView nativeWebView, IWebView webView)
		{
			webView.CanGoBack = nativeWebView.CanGoBack;
		}

		public static void UpdateCanGoForward(this WKWebView nativeWebView, IWebView webView)
		{
			webView.CanGoForward = nativeWebView.CanGoForward;
		}

		public static void UpdateGoBack(this WKWebView nativeWebView, IWebView webView)
		{
			if (nativeWebView.CanGoBack)
				nativeWebView.GoBack();

			nativeWebView.UpdateCanGoBackForward(webView);
		}

		public static void UpdateGoForward(this WKWebView nativeWebView, IWebView webView)
		{
			if (nativeWebView.CanGoForward)
				nativeWebView.GoForward();

			nativeWebView.UpdateCanGoBackForward(webView);
		}

		public static async void UpdateReload(this WKWebView nativeWebView, IWebView webView)
		{
			try
			{
				var url = nativeWebView.Url?.AbsoluteUrl?.ToString() ?? string.Empty;
				await nativeWebView.SyncNativeCookies(webView, url);
			}
			catch (Exception exc)
			{
				Log.Warning(nameof(WebViewHandler), $"Syncing Existing Cookies Failed: {exc}");
			}

			nativeWebView.Reload();
		}

		public static void UpdateEval(this WKWebView nativeWebView, IWebView webView)
		{
			nativeWebView.UpdateEval(webView, string.Empty);
		}

		public static void UpdateEval(this WKWebView nativeWebView, IWebView webView, string script)
		{

		}

		public static void UpdateEvaluateJavaScript(this WKWebView nativeWebView, IWebView webView)
		{
			nativeWebView.UpdateEvaluateJavaScript(webView, string.Empty);
		}

		public static void UpdateEvaluateJavaScript(this WKWebView nativeWebView, IWebView webView, string script)
		{
		
		}

		internal static void UpdateCanGoBackForward(this WKWebView nativeWebView, IWebView? webView)
		{
			if (webView == null)
				return;

			nativeWebView.UpdateCanGoBack(webView);
			nativeWebView.UpdateCanGoForward(webView);
		}

		internal static async Task SyncNativeCookiesToVirtualView(this WKWebView nativeWebView, IWebView webView, string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return;

			var myCookieJar = webView.Cookies;
			if (myCookieJar == null)
				return;

			var uri = CreateUriForCookies(url);
			if (uri == null)
				return;

			var cookies = myCookieJar.GetCookies(uri);
			var retrieveCurrentWebCookies = await nativeWebView.GetCookiesFromNativeStore(webView, url);

			foreach (var nscookie in retrieveCurrentWebCookies)
			{
				if (cookies[nscookie.Name] == null)
				{
					string cookieH = $"{nscookie.Name}={nscookie.Value}; domain={nscookie.Domain}; path={nscookie.Path}";

					myCookieJar.SetCookies(uri, cookieH);
				}
			}

			foreach (Cookie cookie in cookies)
			{
				NSHttpCookie? nSHttpCookie = null;

				foreach (var findCookie in retrieveCurrentWebCookies)
				{
					if (findCookie.Name == cookie.Name)
					{
						nSHttpCookie = findCookie;
						break;
					}
				}

				if (nSHttpCookie == null)
					cookie.Expired = true;
				else
					cookie.Value = nSHttpCookie.Value;
			}

			await nativeWebView.SyncNativeCookies(webView, url);
		}

		internal static async Task SyncNativeCookies(this WKWebView nativeWebView, IWebView webView, string url)
		{
			var uri = CreateUriForCookies(url);

			if (uri == null)
				return;

			var myCookieJar = webView.Cookies;

			if (myCookieJar == null)
				return;

			await nativeWebView.InitialCookiePreloadIfNecessary(webView, url);

			var cookies = myCookieJar.GetCookies(uri);

			if (cookies == null)
				return;

			var retrieveCurrentWebCookies = await nativeWebView.GetCookiesFromNativeStore(webView, url);

			List<NSHttpCookie> deleteCookies = new List<NSHttpCookie>();
			foreach (var cookie in retrieveCurrentWebCookies)
			{
				if (cookies[cookie.Name] != null)
					continue;

				deleteCookies.Add(cookie);
			}

			List<Cookie> cookiesToSet = new List<Cookie>();

			foreach (Cookie cookie in cookies)
			{
				bool changeCookie = true;

				// This code is used to only push updates to cookies that have changed.
				// This doesn't quite work on on iOS 10 if we have to delete any cookies.
				// I haven't found a way on iOS 10 to remove individual cookies. 
				// The trick we use on Android with writing a cookie that expires doesn't work
				// So on iOS10 if the user wants to remove any cookies we just delete 
				// the cookie for the entire domain inside of DeleteCookies and then rewrite
				// all the cookies
				if (NativeVersion.IsAtLeast(11) || deleteCookies.Count == 0)
				{
					foreach (var nsCookie in retrieveCurrentWebCookies)
					{
						// if the cookie value hasn't changed don't set it again
						if (nsCookie.Domain == cookie.Domain &&
							nsCookie.Name == cookie.Name &&
							nsCookie.Value == cookie.Value)
						{
							changeCookie = false;
							break;
						}
					}
				}

				if (changeCookie)
					cookiesToSet.Add(cookie);
			}

			await nativeWebView.SetCookie(cookiesToSet);
			await nativeWebView.DeleteCookies(deleteCookies);
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

		internal static bool HasCookiesToLoad(this WKWebView nativeWebView, IWebView? webView, string url)
		{
			if (nativeWebView == null)
				return false;

			var uri = CreateUriForCookies(url);

			if (uri == null)
				return false;

			var myCookieJar = webView?.Cookies;

			if (myCookieJar == null)
				return false;

			var cookies = myCookieJar.GetCookies(uri);

			if (cookies == null)
				return false;

			return cookies.Count > 0;
		}

		internal static async Task InitialCookiePreloadIfNecessary(this WKWebView nativeWebView, IWebView webView, string url)
		{
			var myCookieJar = webView.Cookies;

			if (myCookieJar == null)
				return;

			var uri = CreateUriForCookies(url);

			if (uri == null)
				return;

			if (!LoadedCookies.Add(uri.Host))
				return;

			// pre ios 11 we sync cookies after navigated
			if (!NativeVersion.IsAtLeast(11))
				return;

			var cookies = myCookieJar.GetCookies(uri);
			var existingCookies = await nativeWebView.GetCookiesFromNativeStore(webView, url);
			foreach (var nscookie in existingCookies)
			{
				if (cookies[nscookie.Name] == null)
				{
					string cookieH = $"{nscookie.Name}={nscookie.Value}; domain={nscookie.Domain}; path={nscookie.Path}";
					myCookieJar.SetCookies(uri, cookieH);
				}
			}
		}

		internal static async Task<List<NSHttpCookie>> GetCookiesFromNativeStore(this WKWebView nativeWebView, IWebView webView, string url)
		{
			NSHttpCookie[]? _initialCookiesLoaded = null;

			if (NativeVersion.IsAtLeast(11))
			{
				_initialCookiesLoaded = await nativeWebView.Configuration.WebsiteDataStore.HttpCookieStore.GetAllCookiesAsync();
			}
			else
			{
				// I haven't found a different way to get the cookies pre ios 11
				var cookieString = await webView.EvaluateJavaScriptAsync("document.cookie");

				if (cookieString != null)
				{
					CookieContainer extractCookies = new CookieContainer();
					var uri = CreateUriForCookies(url);

					foreach (var cookie in cookieString.Split(';'))
						extractCookies.SetCookies(uri, cookie);

					var extracted = extractCookies.GetCookies(uri);
					_initialCookiesLoaded = new NSHttpCookie[extracted.Count];
					for (int i = 0; i < extracted.Count; i++)
					{
						_initialCookiesLoaded[i] = new NSHttpCookie(extracted[i]);
					}
				}
			}

			_initialCookiesLoaded ??= new NSHttpCookie[0];

			List<NSHttpCookie> existingCookies = new List<NSHttpCookie>();
			string domain = CreateUriForCookies(url)?.Host ?? string.Empty;
			foreach (var cookie in _initialCookiesLoaded)
			{
				// we don't care that much about this being accurate
				// the cookie container will split the cookies up more correctly
				if (!cookie.Domain.Contains(domain) && !domain.Contains(cookie.Domain))
					continue;

				existingCookies.Add(cookie);
			}

			return existingCookies;
		}

		internal static async Task SetCookie(this WKWebView nativeWebView, List<Cookie> cookies)
		{
			if (NativeVersion.IsAtLeast(11))
			{
				foreach (var cookie in cookies)
					await nativeWebView.Configuration.WebsiteDataStore.HttpCookieStore.SetCookieAsync(new NSHttpCookie(cookie));
			}
			else
			{
				nativeWebView.Configuration.UserContentController.RemoveAllUserScripts();

				if (cookies.Count > 0)
				{
					WKUserScript wKUserScript = new WKUserScript(new NSString(GetCookieString(cookies)), WKUserScriptInjectionTime.AtDocumentStart, false);

					nativeWebView.Configuration.UserContentController.AddUserScript(wKUserScript);
				}
			}
		}

		internal static async Task DeleteCookies(this WKWebView nativeWebView, List<NSHttpCookie> cookies)
		{
			if (NativeVersion.IsAtLeast(11))
			{
				foreach (var cookie in cookies)
					await nativeWebView.Configuration.WebsiteDataStore.HttpCookieStore.DeleteCookieAsync(cookie);
			}
			else
			{
				var wKWebsiteDataStore = WKWebsiteDataStore.DefaultDataStore;

				// This is the only way I've found to delete cookies on pre ios 11
				// I tried to set an expired cookie but it doesn't delete the cookie
				// So, just deleting the whole domain is the best option I've found
				WKWebsiteDataStore
					.DefaultDataStore
					.FetchDataRecordsOfTypes(WKWebsiteDataStore.AllWebsiteDataTypes, (NSArray records) =>
					{
						for (nuint i = 0; i < records.Count; i++)
						{
							var record = records.GetItem<WKWebsiteDataRecord>(i);

							foreach (var deleteme in cookies)
							{
								if (record.DisplayName.Contains(deleteme.Domain) || deleteme.Domain.Contains(record.DisplayName))
								{
									WKWebsiteDataStore.DefaultDataStore.RemoveDataOfTypes(record.DataTypes,
										  new[] { record }, () => { });

									break;
								}

							}
						}
					});
			}
		}

		internal static string GetCookieString(List<Cookie> existingCookies)
		{
			StringBuilder cookieBuilder = new StringBuilder();

			foreach (Cookie jCookie in existingCookies)
			{
				cookieBuilder.Append("document.cookie = '");
				cookieBuilder.Append(jCookie.Name);
				cookieBuilder.Append("=");

				if (jCookie.Expired)
				{
					cookieBuilder.Append($"; Max-Age=0");
					cookieBuilder.Append($"; expires=Sun, 31 Dec 2000 00:00:00 UTC");
				}
				else
				{
					cookieBuilder.Append(jCookie.Value);
					cookieBuilder.Append($"; Max-Age={jCookie.Expires.Subtract(DateTime.UtcNow).TotalSeconds}");
				}

				if (!string.IsNullOrWhiteSpace(jCookie.Domain))
				{
					cookieBuilder.Append($"; Domain={jCookie.Domain}");
				}
				if (!string.IsNullOrWhiteSpace(jCookie.Domain))
				{
					cookieBuilder.Append($"; Path={jCookie.Path}");
				}
				if (jCookie.Secure)
				{
					cookieBuilder.Append($"; Secure");
				}
				if (jCookie.HttpOnly)
				{
					cookieBuilder.Append($"; HttpOnly");
				}

				cookieBuilder.Append("';");
			}

			return cookieBuilder.ToString();
		}
	}
}