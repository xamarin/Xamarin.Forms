using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Threading;
using System.ComponentModel;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Shell Flyout Header Footer",
		PlatformAffected.All)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class ShellFlyoutHeaderFooter : TestShell
	{
		protected override void Init()
		{
			var page = new ContentPage();

			AddFlyoutItem(page, "Flyout Item");
			page.Content = new StackLayout()
			{
				Children =
				{
					new Label()
					{
						Text = "Open the Flyout and Toggle the Header and Footer. If it changes after each click test has passed",
						AutomationId = "PageLoaded"
					}
				}
			};

			Items.Add(new MenuItem()
			{
				Text = "Toggle Header/Footer Template",
				Command = new Command(() =>
				{
					if(FlyoutHeaderTemplate == null)
					{
						FlyoutHeaderTemplate = new DataTemplate(() => new Label() { Text = "Header Template" });
						FlyoutFooterTemplate = new DataTemplate(() => new Label() { Text = "Footer Template" });
					}
					else if (FlyoutHeaderTemplate != null)
					{
						FlyoutHeaderTemplate = null;
						FlyoutFooterTemplate = null;
					}
				}),
				AutomationId = "ToggleHeaderFooterTemplate"
			});

			Items.Add(new MenuItem()
			{
				Text = "Toggle Header/Footer View",
				Command = new Command(() =>
				{
					if (FlyoutHeader != null)
					{
						FlyoutHeader = null;
						FlyoutFooter = null;
					}
					else
					{
						FlyoutHeader = new Label() { Text = "Header View" };
						FlyoutFooter = new Label() { Text = "Footer View" };
					}
				}),
				AutomationId = "ToggleHeaderFooter"
			});
		}


#if UITEST

		[Test]
		public void FlyoutTests()
		{
			RunningApp.WaitForElement("PageLoaded");
			ShowFlyout();

			RunningApp.Tap("ToggleHeaderFooter");
			RunningApp.WaitForElement("Header View");
			RunningApp.WaitForElement("Footer View");

			RunningApp.Tap("ToggleHeaderFooterTemplate");
			RunningApp.WaitForElement("Header Template");
			RunningApp.WaitForElement("Footer Template");
			RunningApp.WaitForNoElement("Header View");
			RunningApp.WaitForNoElement("Footer View");

			RunningApp.Tap("ToggleHeaderFooterTemplate");
			RunningApp.WaitForElement("Header View");
			RunningApp.WaitForElement("Footer View");
			RunningApp.WaitForNoElement("Header Template");
			RunningApp.WaitForNoElement("Footer Template");

			RunningApp.Tap("ToggleHeaderFooter");
			RunningApp.WaitForNoElement("Header Template");
			RunningApp.WaitForNoElement("Footer Template");
			RunningApp.WaitForNoElement("Header View");
			RunningApp.WaitForNoElement("Footer View");
		}

#endif
	}
}
