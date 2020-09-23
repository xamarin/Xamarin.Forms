using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{

#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Bugzilla)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 52318, "MDP OnAppearing/Disappearing triggers for all pages in navigationstack backgrounding/foregrounding app", PlatformAffected.Android)]
	public class MasterDetailPage52318 : TestMasterDetailPage
	{
		protected override void Init()
		{
			Master = new ContentPage { Title = "Master page", Content = new Label { Text = "Master page" } };
			Detail = new NavigationPage(new ContentPage52318());
		}

#if UITEST && __IOS__
		[Test]
		public void CorrectOnAppearingEventsFire()
		{
			ContentPage52318.CorrectOnAppearingEventsFire(RunningApp);
		}
#endif
	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 52318, "NavigationPage OnAppearing/Disappearing triggers for all pages in navigationstack backgrounding/foregrounding app", PlatformAffected.iOS, NavigationBehavior.SetApplicationRoot, issueTestNumber: 1)]
	public class NavigationPage52318 : TestNavigationPage
	{
		public NavigationPage52318() : base()
		{

		}

		protected async override void Init()
		{
			await PushAsync(new ContentPage52318());
		}

#if UITEST && __IOS__
		[Test]
		public void CorrectOnAppearingEventsFire()
		{
			ContentPage52318.CorrectOnAppearingEventsFire(RunningApp);
		}
#endif
	}


#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Bugzilla)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 52318, "TabbedPage OnAppearing/Disappearing triggers for all pages in navigationstack backgrounding/foregrounding app", PlatformAffected.Android, issueTestNumber: 2)]
	public class TabbedPage52318 : TestTabbedPage
	{
		protected override void Init()
		{
			Children.Add(new NavigationPage(new ContentPage52318()));
			Children.Add(new NavigationPage(new ContentPage52318()));
			Children.Add(new NavigationPage(new ContentPage52318()));
			Children.Add(new NavigationPage(new ContentPage52318()));
			Children.Add(new NavigationPage(new ContentPage52318()));
		}

#if UITEST && __IOS__
		[Test]
		public void CorrectOnAppearingEventsFire()
		{
			ContentPage52318.CorrectOnAppearingEventsFire(RunningApp);
		}
#endif
	}

#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Bugzilla)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 52318, "Shell OnAppearing/Disappearing triggers for all pages in navigationstack backgrounding/foregrounding app", PlatformAffected.Android, issueTestNumber: 3)]
	public class ShellPage52318 : TestShell
	{
		protected override void Init()
		{
			AddFlyoutItem(new ContentPage52318(), "Flyout Item");
		}

#if UITEST && __IOS__
		[Test]
		public void CorrectOnAppearingEventsFire()
		{
			ContentPage52318.CorrectOnAppearingEventsFire(RunningApp);
		}
#endif
	}

	public class ContentPage52318 : ContentPage
	{

#if UITEST && __IOS__
		public static void CorrectOnAppearingEventsFire(IApp RunningApp)
		{
			RunningApp.WaitForElement("Page: 1 appearing.");
			RunningApp.Tap("OK");
			RunningApp.Tap("PushPage");
			RunningApp.WaitForElement("Page: 2 appearing.");
			RunningApp.Tap("OK");
			RunningApp.Tap("PushPage");
			RunningApp.WaitForElement("Page: 3 appearing.");
			RunningApp.Tap("OK");
			RunningApp.SendAppToBackground(TimeSpan.FromSeconds(2));
			RunningApp.WaitForElement("Page: 3 appearing.");
			RunningApp.Tap("OK");
			RunningApp.WaitForNoElement("Page: 1 appearing.");
			RunningApp.WaitForNoElement("Page: 2 appearing.");
		}
#endif

		public ContentPage52318()
		{
			var stackLayout = new StackLayout();
			var label = new Label
			{
				Text = "Tap on the Navigate button as many times as you like to add to the navigation stack. An alert should be visible on page appearing. Hit the Home button and come back. Only the last page should alert."
			};
			stackLayout.Children.Add(label);

			var button = new Button
			{
				Text = "Navigate to a new page",
				Command = new Command(async () =>
				{
					await Navigation.PushAsync(new ContentPage52318());
				}),
				AutomationId = "PushPage"
			};
			stackLayout.Children.Add(button);

			Content = stackLayout;
		}

		protected override void OnAppearing()
		{
			int count = (Parent as NavigableElement).Navigation.NavigationStack.Count;
			Title = $"Page: {count}";
			Device.BeginInvokeOnMainThread(() => DisplayAlert("", Title + " appearing.", "OK"));
			base.OnAppearing();
		}
	}
}