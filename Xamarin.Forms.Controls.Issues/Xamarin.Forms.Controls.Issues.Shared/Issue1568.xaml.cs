using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.TestCasesPages
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1568, "StackLayout, Grid issue",
		PlatformAffected.Android | PlatformAffected.iOS | PlatformAffected.WinPhone)]
	public partial class Issue1568 : ContentPage
	{
		public Issue1568()
		{
			InitializeComponent();
		}
	}
#endif
}