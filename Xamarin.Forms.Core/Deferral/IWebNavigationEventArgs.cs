using System;
namespace Xamarin.Forms
{
	public interface IWebNavigationEventArgs
	{
		WebNavigationEvent NavigationEvent { get; }

		WebViewSource Source { get; }

		string Url { get; }
	}
}
