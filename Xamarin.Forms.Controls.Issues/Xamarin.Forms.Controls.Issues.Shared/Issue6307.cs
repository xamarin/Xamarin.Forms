using System;
using System.Collections.Generic;
using System.Linq;
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
	[Issue(IssueTracker.Github, 6307,
	"Setting BackButtonBehavior IsEnabled to False causes an exception when loading page", PlatformAffected.Android)]

#if UITEST
	[Category(UITestCategories.Shell)]
#endif
	public class Issue6307 : TestShell
	{
		const string StatusLabel = "StatusLabel";

		protected override void Init()
		{
			var page = CreateContentPage();
			Shell.SetBackgroundColor(this, Color.Green);

			page.Content = new StackLayout()
			{
				Children =
				{
					new Label()
					{
						AutomationId = StatusLabel,
						Text = "You should be able to navigate between pages 😎"
					}
				}
			};

			CurrentItem = Items.Last();

			AddTopTab("Tab 1");
			AddTopTab("Tab 2");
			AddTopTab("Tab 3");

			foreach (var item in Items[0].Items[0].Items)
			{
				Shell.SetBackButtonBehavior((ContentPage)item.Content, new BackButtonBehavior() { IsEnabled = false });
			}
		}

		#region Use Base.AddTopTab in 4.3+

		void AddTopTab(string title)
		{
			var page = CreateContentPage();
			if (Items.Count == 0)
			{
				var item = AddContentPage<ShellItem, ShellSection>(page);
				item.Items[0].Items[0].Title = title ?? page.Title;
				return;
			}

			Items[0].Items[0].Items.Add(new ShellContent()
			{
				Title = title ?? page.Title,
				Content = page,
				AutomationId = title
			});
		}

		TShellItem AddContentPage<TShellItem, TShellSection>(ContentPage contentPage = null)
			where TShellItem : ShellItem
			where TShellSection : ShellSection
		{
			contentPage = contentPage ?? new ContentPage();
			TShellItem item = Activator.CreateInstance<TShellItem>();
			item.Title = contentPage.Title;
			TShellSection shellSection = Activator.CreateInstance<TShellSection>();
			Items.Add(item);
			item.Items.Add(shellSection);

			shellSection.Items.Add(new ShellContent()
			{
				Content = contentPage
			});

			return item;
		}

		#endregion

#if UITEST
		[Test]
		public void ShellBackButtonDisabledTests()
		{
			var tab1 = "Tab 1";
			RunningApp.WaitForElement(tab1);
			RunningApp.Tap(tab1);
			RunningApp.Tap("Tab 2");
			RunningApp.Tap("Tab 3");
		}
#endif
	}
}
