using Xamarin.Forms;
using Xamarin.Platform;

namespace Sample
{
	public class WebView : Xamarin.Forms.View, IWebView
	{
		public WebViewSource2 Source { get; set; }
		
		public new double Width
		{
			get { return WidthRequest; }
			set { WidthRequest = value; }
		}

		public new double Height
		{
			get { return HeightRequest; }
			set { HeightRequest = value; }
		}
	}
}