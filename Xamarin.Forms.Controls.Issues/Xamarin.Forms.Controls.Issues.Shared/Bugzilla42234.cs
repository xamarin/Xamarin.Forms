using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

#if UITEST
using Xamarin.UITest;
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
	[Issue(IssueTracker.Bugzilla, 42234, "CarouselView containing (ListView + Footer) creates clickable footer that crashes application", PlatformAffected.Default)]
	public class Bugzilla42234 : TestContentPage
	{
		const string Footer = "footer";
		protected override void Init()
		{
			Content = new CarouselView
			{
				ItemsSource = Enumerable.Range(0, 10).ToList(),
				ItemTemplate = new DataTemplate(() =>
				{
					return new ListView
					{
						ItemsSource = Enumerable.Range(0, 10),
						ItemTemplate = new DataTemplate(() =>
						{
							return new ViewCell()
							{
								View = new Label { Text = "Tap the space below this ListView. If the app crashes, this test has failed." }
							};
						}),
						FooterTemplate = new DataTemplate(() => new Grid { HeightRequest = 100, AutomationId = Footer })
					};
				})
			};
		}

#if UITEST
		[Test]
		public void Bugzilla42234Test()
		{
			RunningApp.WaitForElement(q => q.Marked(Footer));
			Assert.DoesNotThrow(() => RunningApp.Tap(q => q.Marked(Footer)));
		}
#endif
	}
}