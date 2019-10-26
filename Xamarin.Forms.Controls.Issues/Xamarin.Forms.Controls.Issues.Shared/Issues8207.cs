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
	[Issue(IssueTracker.Github, 8207, "[Bug] Shell Flyout Items on UWP aren't showing the Title",
	PlatformAffected.UWP)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issues8207 : TestShell
	{
		protected override void Init()
		{
			var page = new ContentPage()
			{
				Content = new StackLayout()
				{
					Children =
					{
						new Label
						{
							Text = "Hello xamarin forms"
						}
					}
				}
			};

			var flyoutItem = new FlyoutItem
			{
				Title = "Dashboard",
				Icon = "coffee.png",

			};

			flyoutItem.Items.Add(new ShellSection()
			{
				Items =
				{
					new ShellContent()
					{
						Content = page
					}
				}
			});
			
			Items.Add(flyoutItem);
		}
	}
}
