using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public class WebViewStub : StubBase, IWebView
	{
		public WebViewSource2 Source { get; set; }
		public bool CanGoBack { get; set; }
		public bool CanGoForward { get; set; }
		public CookieContainer Cookies { get; set; }

		public void Eval(string script) { }
		public Task<string> EvaluateJavaScriptAsync(string script) { return Task.FromResult(string.Empty); }
		public void GoBack() { }
		public void GoForward() { }
		public void Reload() { }
	}
}