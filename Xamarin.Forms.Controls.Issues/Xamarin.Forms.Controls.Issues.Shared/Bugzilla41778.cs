using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

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
	[Issue(IssueTracker.Bugzilla, 41778, "Slider Inside ScrollView Will Open MasterDetailPage.Master", PlatformAffected.iOS)]
	public class Bugzilla41778 : TestMasterDetailPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			Master = new ContentPage
			{
				Title = "Menu",
				BackgroundColor = Color.Blue
			};

			Detail = new DetailPageCS();
		}
	}

	public class DetailPageCS : ContentPage
	{
		public DetailPageCS()
		{
			var scrollView = new ScrollView { Content = new Slider() };
			scrollView.On<iOS>().SetShouldDelayContentTouches(false);

			Content = scrollView;
		}
	}
}