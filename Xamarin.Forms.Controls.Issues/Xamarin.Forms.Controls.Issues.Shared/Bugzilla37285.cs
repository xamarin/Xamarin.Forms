using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 9937285, "Possible to enter text into Picker control", PlatformAffected.iOS)]
	public class Bugzilla37285 : TestContentPage
	{
		protected override void Init()
		{
			var picker = new Picker { ItemsSource = Enumerable.Range(0, 100).Select(c=> c.ToString()).ToList() };

			var stack = new StackLayout { Children = { picker } };

			Content = stack;
		}
	}
}