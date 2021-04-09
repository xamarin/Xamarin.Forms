using System;

namespace Xamarin.Forms
{
	public class WebNavigatingEventArgs : DeferrableEventArgs, IWebNavigationEventArgs
	{
		public WebNavigatingEventArgs(WebNavigationEvent navigationEvent, WebViewSource source, string url)
			: base(true)
		{
			NavigationEvent = navigationEvent;
			Source = source;
			Url = url;
		}

		//[Obsolete("This will be replaced by deferral token")]
		public bool OldCancel { get; set; }

		public WebNavigationEvent NavigationEvent { get; }

		public WebViewSource Source { get; }

		public string Url { get; }
	}
}