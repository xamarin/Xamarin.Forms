using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12755, "[UWP] BoxView gestures not being recognised when BackgroundColor set to Default",
		PlatformAffected.UWP)]
	public partial class Issue12755 : ContentPage
	{
		public Issue12755()
		{
#if APP
			InitializeComponent();   
#endif
		}

#if APP
		async void OnBoxViewTapped(object sender, System.EventArgs e)
		{
			await DisplayAlert("Alert", "You have been alerted", "OK");
		}

		void OnClearButtonClicked(object sender, System.EventArgs e)
		{
			if (boxView.BackgroundColor == Color.Pink)
			{
				boxView.BackgroundColor = Color.Default;
			}
			else
			{
				boxView.BackgroundColor = Color.Pink;
			}
		}
#endif
	}
}