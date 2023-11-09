using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8804,
		"[Bug] Android TabbedPage Swipe Gesture triggering wrong OnDisappearing",
		PlatformAffected.Android)]
	public partial class Issue8804 : TestTabbedPage
	{
		public Issue8804()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}

		private async void ContentPage1_Disappearing(object sender, System.EventArgs e)
		{
			await DisplayAlert("", "Page 1 Disappearing", "OK");
		}

		private async void ContentPage2_Disappearing(object sender, System.EventArgs e)
		{
			await DisplayAlert("", "Page 2 Disappearing", "OK");
		}
	}
}