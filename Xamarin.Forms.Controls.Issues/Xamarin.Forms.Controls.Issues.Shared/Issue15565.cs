﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 15565, "[Bug] Shell TitleView and ToolBarItems rendering strange display on iOS 16",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
	[NUnit.Framework.Category(UITestCategories.TitleView)]
#endif
	public class Issue15565 : TestShell
	{
		protected override void Init()
		{
			AddTopTab(createContentPage("title 1"), "page 1");
			AddTopTab(createContentPage("title 2"), "page 2");
			AddTopTab(createContentPage("title 3"), "page 3");

			static ContentPage createContentPage(string titleView)
			{
				Label safeArea = new Label();
				ContentPage page = new ContentPage()
				{
					Content = new StackLayout()
					{
						Children =
						{
							new Label()
							{
								Text = "If the TitleView is not visible at the same time as the ToolbarItems, the test has failed.",
								AutomationId = "Instructions"
							},
							safeArea
						}
					}
				};

				page.ToolbarItems.Add(new ToolbarItem() { Text = "Item 1" });
				page.ToolbarItems.Add(new ToolbarItem() { Text = "Item 2" });

				if (!string.IsNullOrWhiteSpace(titleView))
				{
					SetTitleView(page,
						new Grid()
						{
							BackgroundColor = Color.Red,
							AutomationId = "TitleViewId",
							Children = { new Label() { Text = titleView, VerticalTextAlignment = TextAlignment.End } }
						});
				}

				return page;
			}
		}


#if UITEST

		[Test]
		public void TitleViewHeightIsNotZero()
		{
			var titleView = RunningApp.WaitForElement("TitleViewId")[0].Rect;
			var topTab = RunningApp.WaitForElement("page 1")[0].Rect;

			var titleViewBottom = titleView.Y + titleView.Height;
			var topTabTop = topTab.Y;

			Assert.GreaterOrEqual(topTabTop, titleViewBottom, "Title View is incorrectly positioned in iOS 16");
		}

		[Test]
		public void ToolbarItemsWithTitleViewAreRendering()
		{
			RunningApp.WaitForElement("Item 1");
			RunningApp.WaitForElement("Item 3");
		}

		[Test]
		public void NoDuplicateTitleViews()
		{
			var titleView = RunningApp.WaitForElement("TitleViewId");

			Assert.AreEqual(1, titleView.Length);

			RunningApp.Tap("page 1");
			RunningApp.Tap("page 2");
			RunningApp.Tap("page 3");

			titleView = RunningApp.WaitForElement("TitleViewId");

			Assert.AreEqual(1, titleView.Length);
		}
#endif

	}
}