using AWebView = Android.Webkit.WebView;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class WebViewHandlerTests
	{
		AWebView GetNativeWebView(WebViewHandler webViewHandler) =>
			(AWebView)webViewHandler.View;

		string GetNativeSource(WebViewHandler webViewHandler) =>
			GetNativeWebView(webViewHandler).Url;
	}
}