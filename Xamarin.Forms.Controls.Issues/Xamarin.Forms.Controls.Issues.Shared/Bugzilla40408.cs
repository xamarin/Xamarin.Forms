using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40408, "MasterDetailPage and TabbedPage only firing Appearing once", PlatformAffected.WinRT)]
#if UITEST
	[Category(UITestCategories.Navigation)]
#endif
	public class Bugzilla40408 : TestNavigationPage
	{
		const string Page1 = "Page 1";
		const string Page2 = "Page 2";
		const string Appearing = "Appearing";
		const string Disappearing = "Disappearing";
		const string Page3 = "Page 3";
		const string Content = "View Content";
		const string MasterDetailPage = "View Master Detail";
		const string TabbedPage2 = "View TabbedPage";
		const string Ok = "OK";
		protected override void Init()
		{
			BarBackgroundColor = Color.Red;
			
			var contentPage2 = new ContentPage();
			contentPage2.Title = Page2;
			contentPage2.BackgroundColor = Color.Green;

			contentPage2.Appearing += ContentPage2_Appearing;
			contentPage2.Disappearing += ContentPage2_Disappearing;
			
			var tabbedPage1 = new TabbedPage();
			tabbedPage1.Appearing += TabbedPage1_Appearing;
			tabbedPage1.Disappearing += TabbedPage1_Disappearing;

			var contentPage3 = new ContentPage() { Title = Page3 };
			contentPage3.BackgroundColor = Color.Pink;
			tabbedPage1.Children.Add(contentPage3);
			
			var masterDetailPage1 = new MasterDetailPage();
			masterDetailPage1.Title = Page3;
			var master1 = new ContentPage();
			master1.BackgroundColor = Color.Yellow;
			master1.Title = "Master 1";
			var detail1 = new ContentPage();
			detail1.Title = "Detail 1";
			detail1.BackgroundColor = Color.Purple;
			masterDetailPage1.Master = master1;
			masterDetailPage1.Detail = detail1;
			masterDetailPage1.BackgroundColor = Color.Yellow;

			masterDetailPage1.Appearing += MasterDetailPage1_Appearing;
			masterDetailPage1.Disappearing += MasterDetailPage1_Disappearing;
			
			var contentPage1 = new ContentPage();
			SetHasBackButton(contentPage1, true);
			contentPage1.BackgroundColor = Color.Blue;
			contentPage1.Title = Page1;
			var stack = new StackLayout();
			contentPage1.Content = stack;

			stack.Children.Add(new Button() { Text = Content, Command = new Command(() => PushAsync(contentPage2)) });
			stack.Children.Add(new Button() { Text = MasterDetailPage, Command = new Command(() => PushAsync(masterDetailPage1)) });
			stack.Children.Add(new Button() { Text = TabbedPage2, Command = new Command(() => PushAsync(tabbedPage1)) });
			stack.Children.Add(new Label()
			{
				Text = "Navigate to each page 2 times, and make sure the Display Alert shows up all the times.",
				TextColor = Color.White,
				HorizontalTextAlignment = TextAlignment.Center
			});
			
			PushAsync(contentPage1);
		}

		void ContentPage2_Disappearing(object sender, EventArgs e)
		{
			DisplayAlert(Disappearing, "ContentPage", Ok);
		}

		void TabbedPage1_Disappearing(object sender, EventArgs e)
		{
			DisplayAlert(Disappearing, "TabbedPage", Ok);
		}

		void MasterDetailPage1_Disappearing(object sender, EventArgs e)
		{
			DisplayAlert(Disappearing, "MasterDetailPage", Ok);
		}

		void TabbedPage1_Appearing(object sender, EventArgs e)
		{
			DisplayAlert(Appearing, "TabbedPage", Ok);
		}

		void ContentPage2_Appearing(object sender, EventArgs e)
		{
			DisplayAlert(Appearing, "ContentPage", Ok);
		}

		void MasterDetailPage1_Appearing(object sender, EventArgs e)
		{
			DisplayAlert(Appearing, "MasterDetailPage", Ok);
		}


#if UITEST
		[Test]
		public void OnAppearingEvents()
		{
			NavigateTo(Content, Page2);
			NavigateTo(Content, Page2);
			NavigateTo(TabbedPage2, Page3);
			NavigateTo(TabbedPage2, Page3);

			// This one fails in UWP

			NavigateTo(MasterDetailPage, Page3);
			NavigateTo(MasterDetailPage, Page3);
		}

		void NavigateTo(string goTo, string destination)
		{
			RunningApp.WaitForElement(Page1);
			RunningApp.Tap(goTo);
			RunningApp.WaitForElement(Appearing);
			RunningApp.Tap(Ok);
			RunningApp.WaitForElement(destination);
			RunningApp.NavigateBack();
			RunningApp.WaitForElement(Disappearing);
			RunningApp.Tap(Ok);
		}
#endif
	}
}