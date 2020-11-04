using System;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public class WebNavigatingEventArgs : WebNavigationEventArgs
	{
		public WebNavigatingEventArgs(WebNavigationEvent navigationEvent, WebViewSource source, string url) : base(navigationEvent, source, url)
		{
		}

		public bool Cancel { get; set; }

		public Func<Task<bool>> CancelTask { get; set; }
	}
}