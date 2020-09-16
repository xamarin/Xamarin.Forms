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
			var page = AddFlyoutItem("Flyout Item");
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
				Text = "Toggle Header/Footer",
				Command = new Command(() =>
				{
					if(FlyoutHeader != null)
					{
						FlyoutHeaderTemplate = new DataTemplate(() => new Label() { Text = "Header Template" });
						FlyoutFooterTemplate = new DataTemplate(() => new Label() { Text = "Footer Template" });

						FlyoutHeader = null;
						FlyoutFooter = null;
					}
					else if (FlyoutHeaderTemplate != null)
					{
						FlyoutHeader = null;
						FlyoutFooter = null;
					}
					else
					{
						FlyoutHeaderTemplate = null;
						FlyoutFooterTemplate = null;
						FlyoutHeader = new Label() { Text = "Header View" };
						FlyoutFooter = new Label() { Text = "Footer View" };
					}
				}),
				AutomationId = "Toggle"
			});
		}


#if UITEST

		[Test]
		public void FlyoutTests()
		{
			RunningApp.WaitForElement("PageLoaded");
			ShowFlyout();
			RunningApp.Tap("Toggle");
			RunningApp.WaitForElement("Header View");
			RunningApp.WaitForElement("Footer View");
			RunningApp.Tap("Toggle");
			RunningApp.WaitForElement("Header Template");
			RunningApp.WaitForElement("Footer Template");
			RunningApp.Tap("Toggle");
			RunningApp.WaitForNoElement("Header Template");
			RunningApp.WaitForNoElement("Footer Template");
		}

#endif
	}
}
