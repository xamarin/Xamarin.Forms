using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	public class _8954WebView : WebView
	{

	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8954, "Navigated event fired at startup for custom WebView on Android", PlatformAffected.Android)]
	public class Issue8954 : TestContentPage
	{
		protected override void Init()
		{
			var webView = new _8954WebView()
			{
				Source = "https://www.microsoft.com/en-us/",
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			webView.Navigating += (object sender, WebNavigatingEventArgs e) =>
			{
				Debug.WriteLine("Navigating");
			};
			webView.Navigated += (object sender, WebNavigatedEventArgs e) =>
			{
				Debug.WriteLine("Navigated");
			};

			var stackLayout = new StackLayout();
			stackLayout.Children.Add(webView);
			Content = stackLayout;
		}
	}
}