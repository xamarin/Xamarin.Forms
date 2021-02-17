using System.ComponentModel;

namespace Xamarin.Forms
{
	public class HtmlWebViewSource2 : WebViewSource2
	{
		public string? BaseUrl { get; set; }

		public string? Html { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void Load(IWebViewDelegate renderer)
		{
			renderer.LoadHtml(Html, BaseUrl);
		}
	}
}