﻿using Xamarin.Forms.Controls;
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
	[Issue(IssueTracker.Github, 8145, "Shell System.ObjectDisposedException: 'Cannot access a disposed object. Object name: 'Android.Support.Design.Widget.BottomSheetDialog'.'", PlatformAffected.Android)]
	public class Issue8145 : TestShell
	{
		string _titleElement = "Connect";
		protected override void Init()
		{
			Title = "Shell";
			Items.Add(new FlyoutItem
			{
				Title = _titleElement,
				Items = {
					new Tab {
							Title = "notme",
							Items = {
										new ContentPage { Title = "notme",  Content = new Label  { Text = "Click More, then choose the target. If it does not crash, this test has passed." } }
									}
						},new Tab {
							Title = "notme",
							Items = {
										new ContentPage { Title = "notme",  Content = new Label  { Text = "Click More, then choose the target. If it does not crash, this test has passed." } }
									}
						},new Tab {
							Title = "notme",
							Items = {
										new ContentPage { Title = "notme",  Content = new Label  { Text = "Click More, then choose the target. If it does not crash, this test has passed." } }
									}
						},new Tab {
							Title = "notme",
							Items = {
										new ContentPage { Title = "notme",  Content = new Label  { Text = "Click More, then choose the target. If it does not crash, this test has passed." } }
									}
						},new Tab {
							Title = "notme",
							Items = {
										new ContentPage { Title = "notme",  Content = new Label  { Text = "Click More, then choose the target. If it does not crash, this test has passed." } }
									}
						},new Tab {
							Title = "notme",
							Items = {
										new ContentPage { Title = "notme",  Content = new Label  { Text = "Click More, then choose the target. If it does not crash, this test has passed." } }
									}
						},new Tab {
							Title = "notme",
							Items = {
										new ContentPage { Title = "notme",  Content = new Label  { Text = "Click More, then choose the target. If it does not crash, this test has passed." } }
									}
						},new Tab {
							Title = "notme",
							Items = {
										new ContentPage { Title = "notme",  Content = new Label  { Text = "Click More, then choose the target. If it does not crash, this test has passed." } }
									}
						},
					new Tab {
							Title = "target",
							Items = {
										new ContentPage { Title = "Target",  Content = new Label  { Text = "Success" } }
									}
						}
				}
			});
		}

#if UITEST
#if !(__ANDROID__ || __IOS__)
		[Ignore("Shell test is only supported on Android and iOS")]
#endif
		[Test]
		public void Issue8145ShellToolbarDisposedException()
		{
			RunningApp.WaitForElement("More");
			RunningApp.Tap("More");
			RunningApp.WaitForElement("target");
			RunningApp.Tap("target");
			RunningApp.WaitForElement("Success");
		}
#endif
	}
}
