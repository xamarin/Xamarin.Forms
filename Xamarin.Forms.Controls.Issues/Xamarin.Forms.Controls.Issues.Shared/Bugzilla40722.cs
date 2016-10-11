using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40722, "Using FormsAppCompatActivity calls OnDisappearing on device sleep")]
	public class Bugzilla40722 : TestContentPage 
	{
		protected override void Init()
		{
			Content = new Label
			{
				AutomationId = "IssuePageLabel",
				Text = "Sleep the device. If \"Disappearing!\" is displayed in the Output debug window, this test has failed."
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			System.Diagnostics.Debug.WriteLine("Appearing!");
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			System.Diagnostics.Debug.WriteLine("Disappearing!");
		}
	}
}