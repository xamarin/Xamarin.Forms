using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.IsEnabled)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 999999, "ScrollView set to disabled will still allow scrolling", PlatformAffected.All)]
	public class ScrollViewIsEnabled : TestNavigationPage
	{
		const string InitiallyEnabled = "Initially Enabled";
		const string InitiallyNotEnabled = "Initially Not Enabled";

		protected override void Init()
		{
			var initiallyEnabled = new Button { Text = InitiallyEnabled };
			initiallyEnabled.Clicked += (sender, args) => { Navigation.PushAsync(ScrollViewTestPage(true)); };

			var initiallyNotEnabled = new Button { Text = InitiallyNotEnabled };
			initiallyNotEnabled.Clicked += (sender, args) => { Navigation.PushAsync(ScrollViewTestPage(false)); };

			var layout = new StackLayout { Children = { initiallyNotEnabled, initiallyEnabled } };

			var root = new ContentPage { Content = layout };

			PushAsync(root);
		}

		static ContentPage ScrollViewTestPage(bool initiallyEnabled)
		{
			var scrollViewContents = new StackLayout();
			for (int n = 0; n < 100; n++)
			{
				scrollViewContents.Children.Add(new Label() { Text = n.ToString() });
			}

			var sv = new ScrollView { Content = scrollViewContents, IsEnabled = initiallyEnabled };
			var layout = new StackLayout { Margin = new Thickness(5, 40, 5, 0) };

			var toggleButton = new Button { Text = $"Toggle IsEnabled (currently {sv.IsEnabled})" };

			toggleButton.Clicked += (sender, args) =>
			{
				sv.IsEnabled = !sv.IsEnabled;
				toggleButton.Text = $"Toggle IsEnabled (currently {sv.IsEnabled})";
			};

			var instructions = new Label
			{
				Text = @"Attempt to scroll the ScrollView below. 
If 'IsEnabled' is false and the ScrollView scrolls, this test has failed. 
If 'IsEnabled' is true and the ScrollView does not scroll, this test has failed. 
Use the toggle button to check both values of 'IsEnabled'."
			};

			layout.Children.Add(instructions);
			layout.Children.Add(toggleButton);
			layout.Children.Add(sv);

			return new ContentPage { Content = layout };
		}

		//#if UITEST
//		[Test]
//		public void _$BZ$Test()
//		{
//			//RunningApp.WaitForElement(q => q.Marked(""));
//		}
//#endif
	}
}