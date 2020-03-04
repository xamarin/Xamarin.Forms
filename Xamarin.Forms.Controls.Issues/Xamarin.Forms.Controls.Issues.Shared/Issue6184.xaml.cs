using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Shell)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6184, "Throws exception when set isEnabled to false in ShellItem index > 5", PlatformAffected.iOS)]
	public partial class Issue6184 : TestShell
    {
        public Issue6184()
		{
#if APP

			InitializeComponent(); 
#endif
		}

		protected override void Init()
		{
		}

#if UITEST
#if !(__IOS__)
		[Ignore("This issue is just for iOS")]
#endif
		[Test]
		public void GitHubIssue6184()
		{
			RunningApp.WaitForElement(q => q.Marked("More"));
			RunningApp.Tap(q => q.Marked("Issue 5"));
			RunningApp.WaitForElement(q => q.Marked("Issue 5"));
		}
#endif
	}
}