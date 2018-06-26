using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 42320, "Full page CarouselView with embedded Listview is hardly usable", PlatformAffected.Android)]
	public class Bugzilla42320 : TestContentPage
	{
		protected override void Init()
		{
			Content = new CarouselView
			{
				ItemsSource = Enumerable.Range(0, 10).ToList(),
				ItemTemplate = new DataTemplate(() =>
				{
					return new ListView
					{
						ItemsSource = Enumerable.Range(0, 100),
						ItemTemplate = new DataTemplate(() =>
						{
							return new ViewCell()
							{
								View = new Label { Text = "Scroll the ListView normally. If the CarouselView swipes easily or the ListView fails to scroll properly, this test has failed." }
							};
						})
					};
				})
			};
		}
	}
}