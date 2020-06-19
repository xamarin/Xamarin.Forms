using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11107, "[Bug][iOS] Shell Navigation implicitly adds Tabbar",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issue11107 : TestShell
	{
		protected async override void Init()
		{
			Shell.SetTabBarIsVisible(this, false);
			Shell.SetNavBarHasShadow(this, false);

			ContentPage firstPage = new ContentPage()
			{
				Content = new StackLayout()
				{
					Children =
					{
						new Label()
						{
							Text = "If this page has a tab bar the test has failed",
							AutomationId = "Page1Loaded"
						}
					}
				}
			};

			ContentPage secondPage = new ContentPage()
			{
				Content = new StackLayout()
				{
					Children =
					{
						new Label()
						{
							Text = "Hold Please!! Or fail the test if nothing happens."
						}
					}
				}
			};

			var item1 = AddFlyoutItem(firstPage, "Page1");
			item1.Items[0].Title = "Tab 1";
			item1.Items[0].AutomationId = "Tab1AutomationId";
			var item2 = AddFlyoutItem(secondPage, "Page2");

			item1.Route = "FirstPage";
			Routing.RegisterRoute("Issue11107HeaderPage", typeof(Issue11107HeaderPage));

			CurrentItem = item2;
			await Task.Delay(2000);
			await GoToAsync("//FirstPage/Issue11107HeaderPage");
		}

		[Preserve(AllMembers = true)]
		public class Issue11107HeaderPage : ContentPage
		{
			public Issue11107HeaderPage()
			{
				Content = new StackLayout()
				{
					Children =
					{
						new Label()
						{
							Text = "If this page has a tab bar the test has failed",
							AutomationId = "SecondPageLoaded"
						},
						new Label()
						{
							Text = "Click the Back Button"
						}
					}
				};
			}
		}


#if UITEST
		[Test]
		public void TabShouldntBeVisibleWhenThereIsOnlyOnePage()
		{
			RunningApp.WaitForElement("SecondPageLoaded");
			RunningApp.WaitForNoElement("Tab1AutomationId");
			TapBackArrow();
			RunningApp.WaitForElement("Page1Loaded");
			RunningApp.WaitForNoElement("Tab1AutomationId");
		}
#endif
	}
}
