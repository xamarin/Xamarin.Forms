using System;
using System.ComponentModel;

namespace Xamarin.Forms
{
	public abstract class WebViewSource2
	{
		public static implicit operator WebViewSource2(Uri url)
		{
			return new UrlWebViewSource2 { Url = url?.AbsoluteUri };
		}

		public static implicit operator WebViewSource2(string url)
		{
			return new UrlWebViewSource2 { Url = url };
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract void Load(IWebViewDelegate renderer);
	}
}