using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


#if UITEST
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8461, "[Bug] [iOS] [Shell] \"Nav Stack consistency error\"",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
	[NUnit.Framework.Category(UITestCategories.Navigation)]
#endif
	public class Issue8461 : TestShell
	{
		const string ButtonId = "PageButtonId";
		const string LayoutId = "LayoutId";

		protected override void Init()
		{
			var page1 = CreateContentPage("page 1");
			var page2 = CreateContentPage("page 2");

			var pushPageBtn = new Button();
			pushPageBtn.Text = "Push Page";
			pushPageBtn.AutomationId = ButtonId;
			pushPageBtn.Clicked += (sender, args) =>
			{
				Navigation.PushAsync(page2);
			};

			page1.Content = new StackLayout()
			{
				Children =
				{
					pushPageBtn
				}
			};

			page2.Content = new StackLayout()
			{
				AutomationId = LayoutId,
				Children =
				{
					new Label()
					{
						Text = "1. Swipe left to dismiss this page, but cancel the gesture before it completes"
					},
					new Label()
					{
						Text = "2. Swipe left to dismiss this page again, crashes immediately"
					}
				}
			};
		}

#if UITEST && __IOS__
		[Test]
		public void ShellSwipeToDismiss()
		{
			var pushButton = RunningApp.WaitForElement(ButtonId);
			Assert.AreEqual(1, pushButton.Length);

			RunningApp.Tap(ButtonId);
		
			var page2Layout = RunningApp.WaitForElement(LayoutId);
			Assert.AreEqual(1, page2Layout.Length);
			// Swiping from the layout without inertia causes the
			// dismiss gesture to fail, this is intended
			RunningApp.SwipeLeftToRight(LayoutId, 0.99, 750, false);
			// Second swipe gesture is successful and should remove
			// the page from the stack
			RunningApp.SwipeLeftToRight(0.99, 750);

			pushButton = RunningApp.WaitForElement(ButtonId);
			Assert.AreEqual(1, pushButton.Length);
		}
#endif

	}
}
