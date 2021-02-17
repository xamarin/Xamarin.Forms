using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class WebViewHandlerTests : HandlerTestBase<WebViewHandler>
	{
#if __IOS__
		[Fact(Skip = "Currently Fails on iOS")]
#else
		[Fact()]
#endif
		public async Task UrlSourceInitializesCorrectly()
		{
			var webView = new WebViewStub()
			{
				Source = "https://xamarin.com/"
			};

			var url = ((UrlWebViewSource2)webView.Source).Url;

			await ValidatePropertyInitValue(webView, () => url, GetNativeSource, url);
		}
	}
}