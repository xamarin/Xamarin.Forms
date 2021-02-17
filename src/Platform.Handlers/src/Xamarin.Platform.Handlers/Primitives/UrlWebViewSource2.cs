using System.ComponentModel;

namespace Xamarin.Forms
{
	public partial class UrlWebViewSource2 : WebViewSource2
	{
		public string? Url { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void Load(IWebViewDelegate renderer)
		{
			renderer.LoadUrl(Url);
		}
	}
}