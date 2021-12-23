﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11769, "[Bug] Shell throws exception when delay adding Shell Content", issueTestNumber: 2)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif

	public class Issue11769_DelayedShellContent : TestShell
	{
		protected override void Init()
		{
			Items.Add(new FlyoutItem()
			{
				Items =
				{
					new Tab()
				}
			});

			Device.InvokeOnMainThreadAsync(() =>
			{
				var shellContent = new ShellContent()
				{
					Content = new ContentPage()
					{
						Content = new Label() { Text = "Success" }
					}
				};

				Items[0].Items[0].Items.Add(shellContent);
			});
		}

#if UITEST
		[Test]
		public void DelayedAddingOfShellContentDoesntCrash()
		{
			RunningApp.WaitForElement("Success");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11769, "[Bug] Shell throws exception when delay adding Shell Section", issueTestNumber: 1)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif

	public class Issue11769_DelayedShellSection : TestShell
	{
		protected override void Init()
		{
			Items.Add(new FlyoutItem());
			Device.InvokeOnMainThreadAsync(() =>
			{
				var tab = new Tab()
				{
					Items =
					{
						new ShellContent()
						{
							Content = new ContentPage()
							{
								Content = new Label() { Text = "Success" }
							}
						}
					}
				};

				Items[0].Items.Add(tab);
			});
		}

#if UITEST
		[Test]
		public void DelayedAddingOfShellSectionDoesntCrash()
		{
			RunningApp.WaitForElement("Success");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11769, "[Bug] Shell throws exception when delay adding Shell Item", issueTestNumber: 0)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif

	public class Issue11769_DelayedShellItem : TestShell
	{
		protected override void Init()
		{
			Device.InvokeOnMainThreadAsync(() =>
			{
				var page = AddBottomTab("Flyout Item");
				page.Content = new Label()
				{
					Text = "Success"
				};
			});
		}

#if UITEST
		[Test]
		public void DelayedAddingOfShellItemDoesntCrash()
		{
			RunningApp.WaitForElement("Success");
		}
#endif
	}
}
