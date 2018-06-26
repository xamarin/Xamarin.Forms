using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40631, "Setting CarouselView.Position throw exception (v2.2.0.18-pre3)", PlatformAffected.Android)]
	public class Bugzilla40631 : TestContentPage
	{
		const int Position = 3;
		protected override void Init()
		{
			var cv = new CarouselView
			{
				ItemsSource = Enumerable.Range(0, 10).ToList(),
				ItemTemplate = new DataTemplate(() =>
				{
					var label = new Label();
					label.SetBinding(Label.TextProperty, ".");
					return label;
				}),
				Position = Position,
				HeightRequest = 100
			};

			Content = new StackLayout
			{
				Children = {
						new Label { Text = $"The CarouselView should display '{Position}'. If it does not, this test has failed." },
						cv
					}
			};
		}

#if (UITEST)
		[Test]
		public void Bugzilla40631Test()
		{
			RunningApp.WaitForElement(q => q.Marked(Position.ToString()));
		}
#endif
	}
}