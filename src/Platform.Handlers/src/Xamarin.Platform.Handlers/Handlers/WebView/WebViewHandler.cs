namespace Xamarin.Platform.Handlers
{
	public partial class WebViewHandler
	{
		public static PropertyMapper<IWebView, WebViewHandler> WebViewMapper = new PropertyMapper<IWebView, WebViewHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IWebView.Source)] = MapSource,
			[nameof(IWebView.Cookies)] = MapCookies,
			[nameof(IWebView.CanGoBack)] = MapCanGoBack,
			[nameof(IWebView.CanGoForward)] = MapCanGoForward,
			Actions =
			{
				[nameof(IWebView.GoBack)] = MapGoBack,
				[nameof(IWebView.GoForward)] = MapGoForward,
				[nameof(IWebView.Reload)] = MapReload,
				[nameof(IWebView.Eval)] = MapEval,
				[nameof(IWebView.EvaluateJavaScriptAsync)] = MapEvaluateJavaScript
			}
		};

		public WebViewHandler() : base(WebViewMapper)
		{

		}

		public WebViewHandler(PropertyMapper mapper) : base(mapper ?? WebViewMapper)
		{

		}
	}
}