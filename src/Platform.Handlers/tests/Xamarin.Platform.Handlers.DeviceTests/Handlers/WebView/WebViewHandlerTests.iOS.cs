using WebKit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class WebViewHandlerTests
	{
		WKWebView GetNativeWebView(WebViewHandler webViewHandler) =>
			(WKWebView)webViewHandler.View;

		string GetNativeSource(WebViewHandler webViewHandler) =>
			GetNativeWebView(webViewHandler).Url.AbsoluteString;
	}
}