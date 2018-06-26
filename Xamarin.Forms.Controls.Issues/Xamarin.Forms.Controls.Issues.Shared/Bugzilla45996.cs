using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;

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
	[Issue(IssueTracker.Bugzilla, 45996, "Adding Items to carouselView after initial load does not work on uwp", PlatformAffected.WinRT)]
	public class Bugzilla45996 : TestContentPage
	{
		const string Success = "Success";
		const string ButtonText = "Click me";
		List<int> items = new List<int>();

		protected override void Init()
		{
			var carousel = new CarouselView { ItemsSource = items, ItemTemplate = new DataTemplate(() => new Label { Text = Success }), HeightRequest = 250 };
			var button = new Button { Text = ButtonText };
			button.Clicked += (sender, e) =>
			{
				items.Insert(0, 0);
			};
			var layout = new StackLayout { Children = { carousel, button } };
			Content = layout;
		}


#if UITEST
		[Test]
		public void Bugzilla45996Test()
		{
			RunningApp.WaitForElement(q => q.Marked(ButtonText));
			RunningApp.Tap(q => q.Marked(ButtonText));
			RunningApp.WaitForElement(q => q.Marked(Success));
		}
#endif
	}
}