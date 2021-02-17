using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IWebView : IView
	{
		WebViewSource2 Source { get; set; }
		CookieContainer Cookies { get; set; }
		bool CanGoBack { get; }
		bool CanGoForward { get; }

		void GoBack();
		void GoForward();
		void Reload();
		void Eval(string script);
		Task<string> EvaluateJavaScriptAsync(string script);
	}
}