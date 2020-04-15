using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10324, "Unable to intercept or disable mouse back button navigation on UWP", PlatformAffected.UWP)]
	public class Issue10324 : TestNavigationPage
	{
		protected override void Init()
		{
			Navigation.PushAsync(new IssueFirstPage());
		}

		class IssueFirstPage : ContentPage
		{
			public IssueFirstPage()
			{
				Navigation.PushAsync(new IssueTestPage());
			}
		}

		class IssueTestPage : ContentPage
		{
			public IssueTestPage()
			{
				Content = new Label { Text = "Hit Mouse BackButton/XButton1" };
			}

			protected override bool OnBackButtonPressed()
			{
				DisplayAlert("OnBackButtonPressed", "OnBackButtonPressed", "OK");
				return true;
			}
		}		
	}
}