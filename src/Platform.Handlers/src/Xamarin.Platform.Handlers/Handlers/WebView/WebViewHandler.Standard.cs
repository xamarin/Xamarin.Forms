using System;

namespace Xamarin.Platform.Handlers
{
	public partial class WebViewHandler : AbstractViewHandler<IWebView, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();

		public static void MapSource(IViewHandler handler, IWebView webView) { }
		public static void MapCookies(IViewHandler handler, IWebView webView) { }
		public static void MapCanGoBack(IViewHandler handler, IWebView webView) { }
		public static void MapCanGoForward(IViewHandler handler, IWebView webView) { }

		public static void MapGoBack(IViewHandler handler, IWebView webView) { }
		public static void MapGoForward(IViewHandler handler, IWebView webView) { }
		public static void MapReload(IViewHandler handler, IWebView webView) { }
		public static void MapEval(IViewHandler handler, IWebView webView) { }
		public static void MapEvaluateJavaScript(IViewHandler handler, IWebView webView) { }
	}
}