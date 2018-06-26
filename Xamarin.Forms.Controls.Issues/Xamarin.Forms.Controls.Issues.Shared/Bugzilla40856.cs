using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

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
	[Issue(IssueTracker.Bugzilla, 40856, "Multiple crashes in iOS 8.1 when popping a page containing a CarouselView", PlatformAffected.iOS)]
	public class Bugzilla40856 : TestNavigationPage
	{
		const string ButtonText = "Click me";
		const string Success = "Success";

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
				})
			};

			var button = new Button { Text = ButtonText };
			button.Clicked += (s, e) =>
			{
				PopAsync();
			};

			var contentPage = new ContentPage
			{
				Content = new StackLayout
				{
					Children = {
						new Label { Text = "Click the button to pop this page. If the app crashes, this test has failed." },
						button,
						cv
					}
				}
			};

			PushAsync(new ContentPage { Content = new Label { Text = Success } });
			PushAsync(contentPage);
		}

#if (UITEST)
		[Test]
		public void Bugzilla40856Test()
		{
			RunningApp.WaitForElement(q => q.Marked(ButtonText));
			RunningApp.Tap(q => q.Marked(ButtonText));
			RunningApp.WaitForElement(q => q.Marked(Success));
		}
#endif
	}
}