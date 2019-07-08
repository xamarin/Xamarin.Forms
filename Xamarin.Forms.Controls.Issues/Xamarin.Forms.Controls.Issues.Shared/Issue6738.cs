using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Controls;
using System;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Shell)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6738, "Flyout Navigation fails when coupled with tabs that have a stack", PlatformAffected.Android)]
	public class Issue6738 : TestShell
	{
		Tab flyoutContent = new Tab();
		Button pushPageButton = new Button { Text = "Tap to push new page to stack" };
		Button insertPageButton = new Button { Text = "Push another page to the stack, then go to tab two" };
		ContentPage pushedPage;
		Tab tabOne = new Tab { Title = "TabOne" };
		Tab tabTwo = new Tab { Title = "TabTwo " };


		void OnReturnTapped(object sender, EventArgs e)
		{
			ForceTabSwitch();
		}

		void OnPushTapped(object sender, EventArgs e)
		{
			pushedPage = new ContentPage { Content = insertPageButton };
			Navigation.PushAsync(pushedPage);
		}

		void OnInsertTapped(object sender, EventArgs e)
		{
			Navigation.InsertPageBefore(new ContentPage { Content = new Label { Text = "This is an extra page" } }, pushedPage);
			ForceTabSwitch();
		}

		void ForceTabSwitch()
		{
			if (CurrentItem != null)
			{
				if (CurrentItem.CurrentItem == tabOne)
				{
					CurrentItem.CurrentItem = tabTwo;
				}
				else
					CurrentItem.CurrentItem = tabOne;
			}
		}

		protected override void Init()
		{
			var tabOnePage = new ContentPage { Content = pushPageButton };
			var stackLayout = new StackLayout();
			stackLayout.Children.Add(new Label { Text = "If you've been here already, go to tab one now. Otherwise, go to Other Page in the flyout." });
			var returnButton = new Button { Text = "Go back to tab 1" };
			returnButton.AutomationId = "GoToFirstTabButton";
			returnButton.Clicked += OnReturnTapped;
			stackLayout.Children.Add(returnButton);

			var tabTwoPage = new ContentPage { Content =  stackLayout };
			tabOne.Items.Add(tabOnePage);
			tabTwo.Items.Add(tabTwoPage);

			pushPageButton.AutomationId = "PushPageButton";
			pushPageButton.Clicked += OnPushTapped;
			insertPageButton.AutomationId = "InsertPageButton";
			insertPageButton.Clicked += OnInsertTapped;
			flyoutContent.Items.Add(new ContentPage { Content = new Label { Text = "Go back to main page via the flyout" } });

			Items.Add(
					new FlyoutItem
					{
						Title = "Main",
						Items = { tabOne, tabTwo }
					}
			);
			Items.Add(new FlyoutItem {
				Title = "Other Page",
				Items = { flyoutContent }
			});
		}

#if UITEST
#if !(__ANDROID__ || __IOS__)
		[Ignore("Shell test is only supported on Android and iOS")]
#endif
		[Test]
		public void FlyoutNavigationBetweenItemsWithNavigationStacks()
		{
			RunningApp.WaitForElement(pushPageButton.AutomationId);
			RunningApp.Tap(pushPageButton.AutomationId);
			RunningApp.WaitForElement(insertPageButton.AutomationId);
			RunningApp.Tap(insertPageButton.AutomationId);

			TapInFlyout("Other Page");
			TapInFlyout("Main");

			RunningApp.WaitForElement("GoToFirstTabButton");
			RunningApp.Tap("GoToFirstTabButton");
			RunningApp.NavigateBack();
			RunningApp.NavigateBack();
		}
#endif
	}
}
