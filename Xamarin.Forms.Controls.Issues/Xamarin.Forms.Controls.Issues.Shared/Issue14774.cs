using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14774, "[Bug] [iOS] TabbedPage tabs titles overlap with Xamarin.Forms 5.0.0.2196", PlatformAffected.iOS)]
	public class Issue14774 : TestTabbedPage
	{
		protected override void Init()
		{
			Children.Add(new ContentPage
			{
				Title = "LonglonglongTab1",
				Content = new Label
				{
					Text = "Test 1"
				}
			});

			Children.Add(new ContentPage
			{
				Title = "LonglonglongTab2",
				Content = new Label
				{
					Text = "Test 2"
				}
			});

			Children.Add(new ContentPage
			{
				Title = "LonglonglongTab3",
				Content = new Label
				{
					Text = "Test 3"
				}
			});

			Children.Add(new ContentPage
			{
				Title = "LonglonglongTab4",
				Content = new Label
				{
					Text = "Test 4"
				}
			});
		}
	}
}
