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
	[Issue(IssueTracker.Bugzilla, 45067, "[UWP] No way of cleanly dismissing soft keyboard", PlatformAffected.Default)]
	public class Bugzilla45067 : TestContentPage
	{
		protected override void Init()
		{
			var username = new Entry
			{
				Placeholder = "Username"
			};
			var password = new Entry
			{
				Placeholder = "Password",
				IsPassword = true
			};
			username.Completed += (s, e) => password.Focus();
			password.Completed += (s, e) => password.Unfocus();
			Content = new StackLayout
			{
				Children =
				{
					username,
					password,
					new Button
					{
						Text = "Submit"
					}
				}
			};
		}
	}
}