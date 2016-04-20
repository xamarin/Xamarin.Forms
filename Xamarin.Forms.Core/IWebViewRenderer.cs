namespace Xamarin.Forms
{
	public interface IWebViewRenderer
	{
		void LoadHtml(string html, string baseUrl);
		void LoadUrl(string url);
	}
}