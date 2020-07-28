﻿using System;
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
	[Issue(IssueTracker.Github, 11523, "[Bug] FlyoutBehavior.Disabled removes back-button from navbar",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issue11523 : TestShell
	{
		protected override async void Init()
		{
			ContentPage contentPage = new ContentPage()
			{
				Content =
					new StackLayout()
					{
						Children = 
						{
							new Label()
							{
								Text = "This Page Should Have a Back Button that you should click",
								AutomationId = "PageLoaded"
							}
						}
					}
			};

			var firstPage = AddBottomTab("First Page");
			firstPage.Content =
					new StackLayout()
					{
						Children =
						{
							new Label()
							{
								Text = "This Page Should Have a Hamburger Menu Icon when you return to it",
								
							}
						}
					};

			await Task.Delay(1000);

			contentPage.Appearing += (_, __) =>
			{
				this.FlyoutBehavior = FlyoutBehavior.Disabled;
			};

			contentPage.Disappearing += (_, __) =>
			{
				this.FlyoutBehavior = FlyoutBehavior.Flyout;
			};

			await Navigation.PushAsync(contentPage);
		}

#if UITEST
		[Test]
		public void BackButtonStillVisibleWhenFlyoutBehaviorDisabled()
		{
			RunningApp.WaitForElement("PageLoaded");
			RunningApp.WaitForElement(BackButtonAutomationId);
			RunningApp.Tap(BackButtonAutomationId);
			RunningApp.WaitForElement(FlyoutIconAutomationId);
		}
#endif
	}
}
