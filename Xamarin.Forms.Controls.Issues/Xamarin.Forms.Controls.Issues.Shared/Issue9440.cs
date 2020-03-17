﻿using Xamarin.Forms.CustomAttributes;
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
	[Issue(IssueTracker.Github, 9440, "Flyout closes with two or more taps", PlatformAffected.Android)]
	public class Issue9440 : TestShell
	{
		const string Test1 = "Test 1";
		const string Test2 = "Test 2";
		string _idIconElement = "shellIcon";
		protected override void Init()
		{
			this.AddFlyoutItem(CreatePage(Test1, "Page 1"), Test1);
			this.AddFlyoutItem(CreatePage(Test2, "Page 2"), Test2);

			ContentPage CreatePage(string title, string txt)
			{
				return new ContentPage 
				{ Title = title, 
					Content = new ScrollView 
					{ 
						Content = new Label 
						{ 
							Text = txt,
							TextColor = Color.Black
						} 
					} 
				};
			}
			FlyoutIcon = new FontImageSource
			{
				Glyph = "\uf2fb",
				FontFamily = Issue5132.DefaultFontFamily(),
				Size = 20,
				AutomationId = _idIconElement
			};
			FlyoutIcon.SetValue(AutomationProperties.HelpTextProperty, "This as Shell FlyoutIcon");
			FlyoutIcon.SetValue(AutomationProperties.NameProperty, "Shell Icon");
		}

#if UITEST
#if !(__ANDROID__ || __IOS__)
		[Ignore("Shell test is only supported on Android and iOS")]
#endif
		[Test]
		public void GitHubIssue9440()
		{
			RunningApp.WaitForElement(q => q.Marked(_idIconElement));
			DoubleTapInFlyout(Test1, _idIconElement);
			RunningApp.WaitForElement(q => q.Marked(Test1));
		}
#endif
	}

	public static class ShellExtention
	{
		public static Shell AddFlyoutItem(this Shell shell, ContentPage page, string title)
		{
			var item = new FlyoutItem
			{
				Title = title,
				Items =
				{
					new Tab
					{
						Title = title,
						Items =
						{
							page
						}
					}
				}
			};
			shell.Items.Add(item);

			return shell;
		}
	}
}