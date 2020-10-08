﻿using System;

using Xamarin.Forms.CustomAttributes;
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
	[Issue(IssueTracker.Bugzilla, 41842, "Set FlyoutPage.Detail = New Page() twice will crash the application when set FlyoutLayoutBehavior = FlyoutLayoutBehavior.Split", PlatformAffected.WinRT)]
	public class Bugzilla41842 : TestFlyoutPage
	{
		protected override void Init()
		{
			FlyoutLayoutBehavior = FlyoutLayoutBehavior.Split;

			Flyout = new Page() { Title = "Flyout" };

			Detail = new NavigationPage(new Page());
			Detail = new NavigationPage(new ContentPage { Content = new Label { Text = "Success" } });
		}

#if UITEST
		[Test]
		public void Bugzilla41842Test()
		{
			RunningApp.WaitForElement(q => q.Marked("Success"));
		}
#endif
	}
}
