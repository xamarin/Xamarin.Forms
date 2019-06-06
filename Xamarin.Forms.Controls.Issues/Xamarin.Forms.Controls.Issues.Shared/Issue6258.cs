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
	[Issue(IssueTracker.Github, 6258, "[Android] ContextActions icon not working",
		PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.ListView)]
#endif
	public class Issue6258 : TestNavigationPage
	{
		protected override void Init()
		{
			var page = new ContentPage();

			PushAsync(page);

			page.Content = new ListView()
			{
				ItemsSource = new [] {"1"},
				ItemTemplate = new DataTemplate(() =>
				{
					ViewCell cells = new ViewCell();

					cells.ContextActions.Add(new MenuItem()
					{
						IconImageSource = "coffee.png",
						AutomationId = "coffee.png",
						//Text = "Menuitem Text"
					});

					cells.View = new StackLayout()
					{
						Children =
						{
							new Label()
							{
								Text = "test",
								AutomationId = "ListViewItem"
							}
						}
					};

					return cells;
				})
			};
		}

#if UITEST
		[Test]
		public void ContextActionsIconImageSource()
		{
			RunningApp.WaitForElement("ListViewItem");
			RunningApp.ActivateContextMenu("ListViewItem");
			RunningApp.WaitForElement("coffee.png");
		}
#endif
	}
}
