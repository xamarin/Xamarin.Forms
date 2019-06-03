using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;

#if UITEST
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6323, "TabbedPage Page not watching icon changes", PlatformAffected.UWP)]
	public class Issue6323 : TestTabbedPage
	{
		protected override void Init()
		{
			Device.StartTimer(TimeSpan.FromSeconds(2), () =>
			{
				Children.Add(new ContentPage
				{
					Content = new Label { Text = "Success" }
				});
				return false;
			});
		}

#if UITEST && __WINDOWS__
		[Test]
		public void Issue6323Test()
		{
			RunningApp.WaitForElement("Success");
		}
#endif
	}
}
