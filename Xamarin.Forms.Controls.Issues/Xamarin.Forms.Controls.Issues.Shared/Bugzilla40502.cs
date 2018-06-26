using System.Linq;
using Xamarin.Forms.CustomAttributes;

#if UITEST
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Bugzilla, 40502, "CarouselView NRE on MainPage change WinRT", PlatformAffected.WinRT)]
	public class Bugzilla40502 : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(new Bugzilla40856());
		}
	}
}