using System;
using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12603,
	"[Bug] ScrollView ScrollToAsync behaves different on Android and iOS",
	PlatformAffected.iOS)]
	public partial class Issue12603 : ContentPage
	{
		public Issue12603()
		{
#if APP
			InitializeComponent();
#endif
		}
#if APP
		private async void Button_Clicked(object sender, EventArgs e)
		{
			await scrollview.ScrollToAsync(lastLabel, ScrollToPosition.Start, true);
		}
#endif
	}
}