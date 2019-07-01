using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest.Queries;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2577, "Hamburger icon not shown when using FormsAppCompatActivity", PlatformAffected.Android)]
	public class Issue2577 : TestMasterDetailPage
	{
		protected override void Init()
		{
			Master = new ContentPage { Title = "master page" };
			Detail = new NavigationPage(new DetailPage());
		}

		class DetailPage : ContentPage
		{
			public NavigationPage ParentPage => Parent as NavigationPage;

			public DetailPage()
			{
				var button = new Button { Text = "Click me", AutomationId = "NavigateButton" };
				button.Clicked += async (o, s) =>
				{
					var button2 = new Button { Text = "Toggle back button", AutomationId = "ToggleBackButton" };

					var page = new ContentPage { Content = new StackLayout { Children = {
							new Label { Text = "If there is no hamburger button, this test has failed. If you cannot toggle the back arrow, this test has failed." },
							button2
						} } };

					button2.Clicked += (o2, s2) =>
					{
						NavigationPage.SetHasBackButton(page, !NavigationPage.GetHasBackButton(page));
					};

					NavigationPage.SetHasBackButton(page, false);
					await ParentPage.PushAsync(page);
				};
				Content = button;
			}
		}

#if UITEST
		[Test]
		public void Issue2577Test()
		{
			RunningApp.WaitForElement("NavigateButton");
			RunningApp.Tap("NavigateButton");

			RunningApp.WaitForElement("ToggleBackButton");

			RunningApp.Screenshot("Hamburger menu icon is visible");

			AppResult[] items = RunningApp.Query("OK");
			Assert.AreNotEqual(items.Length, 0);
			RunningApp.Tap("OK");

			RunningApp.Screenshot("Flyout menu is showing");

			RunningApp.SwipeRightToLeft();

			RunningApp.Tap("ToggleBackButton");

			items = RunningApp.Query("OK");
			Assert.AreEqual(items.Length, 0);

			RunningApp.Screenshot("Back arrow is showing");

			var backArrow = RunningApp.Query(e => e.Class("AppCompatImageButton")).First(e => e.Label == null);

			RunningApp.TapCoordinates(backArrow.Rect.CenterX, backArrow.Rect.CenterY);

			RunningApp.WaitForElement("NavigateButton");

			RunningApp.Screenshot("Back at first screen");
		}
#endif
	}
}