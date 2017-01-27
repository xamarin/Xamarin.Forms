using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1653, "ScrollView exceeding bounds",
		PlatformAffected.Android | PlatformAffected.iOS | PlatformAffected.WinPhone)]
	public partial class Issue1653 : ContentPage
	{
		public Issue1653()
		{
			InitializeComponent();

			for (var i = 0; i < 40; i++)
				addonGroupStack.Children.Add(new Label { Text = "Testing 123" });
		}
	}
#endif
}