﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 1, "Issue Description", PlatformAffected.Default)]
	public class Bugzilla1 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Content = new Label
			{
				AutomationId = "PageLabel",
				Text = "See if I'm here"
			};
		}

#if UITEST
		[Test]
		public void Bugzilla1Test ()
		{
			RunningApp.Screenshot ("I am at Bugzilla1");
			RunningApp.WaitForElement (q => q.Marked ("PageLabel"));
			RunningApp.Screenshot ("I see the Label");
		}
#endif
	}
}