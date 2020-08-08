using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9210, "[Bug] iOS keyboard case flickers when switching entries", PlatformAffected.iOS)]
	public class Issue9210 : TestContentPage
	{
		protected override void Init()
		{
			var stackLayout = new StackLayout();
			stackLayout.Children.Add(new Entry() { Text="Some demo text." });
			stackLayout.Children.Add(new Entry() { Text = "Some other demo text." });

			Content = stackLayout;
		}
	}
}