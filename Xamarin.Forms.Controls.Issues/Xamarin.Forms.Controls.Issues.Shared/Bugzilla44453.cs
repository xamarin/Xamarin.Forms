﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

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
	[Issue(IssueTracker.Bugzilla, 44453, "[UWP] ToolbarItem Text hard to see when BarTextColor is light", PlatformAffected.WinRT)]
	public class Bugzilla44453 : TestFlyoutPage
	{
		protected override void Init()
		{
			var content = new ContentPage
			{
				Title = "UWPToolbarItemColor",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children =
					{
						new Label
						{
							LineBreakMode = LineBreakMode.WordWrap,
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "The toolbar secondary items should not have white text on a light background"
						}
					}
				}
			};

			FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
			Flyout = new ContentPage
			{
				Title = "Flyout"
			};
			Detail = new NavigationPage(content)
			{
				BarBackgroundColor = Color.Green,
				BarTextColor = Color.White
			};

			Detail.ToolbarItems.Add(new ToolbarItem("Test Secondary Item", null, delegate
			{ }, ToolbarItemOrder.Secondary));
		}
	}
}