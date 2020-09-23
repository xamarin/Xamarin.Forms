using System;
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
	[Issue(IssueTracker.Github, 12055, "Multiple PageAppearing/OnAppearing Issues",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
	[NUnit.Framework.Category(UITestCategories.LifeCycle)]
#endif
	public class Issue12055 : TestShell
	{
		Label label1;
		Label label2;
		Label label3;

		protected override void Init()
		{
			var page1 = AddTopTab("Tab 1");
			var page2 = AddTopTab("Tab 2");
			var page3 = AddBottomTab("Tab 3");

			label1 = new Label();
			label2 = new Label();
			label3 = new Label();

			page1.Content = new StackLayout()
			{
				Children =
				{
					new Label(){ Text = "If you see `Test Failed` it means too many OnAppearing events fired"},
					label1,
					label2,
					label3
				}
			};
			
			page1.Appearing += OnPage1Appearing;
			page2.Appearing += OnPage2Appearing;
			page3.Appearing += OnPage3Appearing;
		}

		private void OnPage1Appearing(object sender, EventArgs e)
		{
			label1.Text = "Correct Appearing Fired";
		}

		private void OnPage2Appearing(object sender, EventArgs e)
		{
			label2.Text = "Test Failed";
		}

		private void OnPage3Appearing(object sender, EventArgs e)
		{
			label3.Text = "Test Failed";
		}

#if UITEST
		[Test]
		public void OnApperingOnlyFiresForVisibleTab()
		{
			RunningApp.WaitForElement("Correct Appearing Fired");
			RunningApp.WaitForNoElement("Test Failed");
		}
#endif
	}
}
