using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

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
	[Issue(IssueTracker.Bugzilla, 45702, "Disabling back press on modal page causes app to crash", PlatformAffected.Android)]
	public class Bugzilla45702 : TestMasterDetailPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			Master = new ContentPage() { Title = "hello" };
			Detail = new ContentPage()
			{
				Title = "world",
				Content = new ContentView
				{
					Content = new Label
					{
						HorizontalTextAlignment = TextAlignment.Center,
						VerticalTextAlignment = TextAlignment.Center,
						Text =
							"Pressing Back button to go to home screen should not crash the app when MasterDetailPage is nested under NavigationPage"
					}
				}
			};
		}
	}
}