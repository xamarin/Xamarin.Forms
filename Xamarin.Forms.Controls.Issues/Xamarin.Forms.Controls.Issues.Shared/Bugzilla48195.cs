using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 48195, "YouTube video plays sound after power button is pressed", PlatformAffected.Android)]
	public class Bugzilla48195 : TestNavigationPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			Application.Current.On<Android>().ShouldHandleWebViewStateOnLifecycleChange(true);
			PushAsync(new WebPage());
		}
	}

	public class WebPage : ContentPage
	{
		public WebPage()
		{
			var browser = new WebView { Source = "https://www.youtube.com/watch?v=dQw4w9WgXcQ" };

			Content = browser;
		}
	}
}