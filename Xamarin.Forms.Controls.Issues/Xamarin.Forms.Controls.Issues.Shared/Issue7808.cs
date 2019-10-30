using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7808, "[Enhancement] Allow providing custom ShellSectionRootRenderer",
		PlatformAffected.iOS)]

	public class Issue7808 : TestShell
	{
		const string CreateBottomTabButton = "CreateBottomTabButton";
		protected override void Init()
		{
			
			var page = CreateContentPage();
			page.Title = "Main";
			page.Content = CreateEntryInsetView();
			CurrentItem = Items.Last();
		}

		View CreateEntryInsetView()
		{
			var random = new Random();
			ScrollView view = null;
			view = new ScrollView()
			{
				Content = new StackLayout()
				{
					Children =
						{
							new Button()
							{
								Text = "Bottom Tab",
								AutomationId = CreateBottomTabButton,
								Command = new Command(() => AddBottomTab("bottom", "coffee.png"))
							}
						}
				}
			};
			return view;
		}

#if (UITEST && __IOS__)

		[Test]
		[Category(UITestCategories.Shell)]
		[Category(UITestCategories.ManualReview)]
		public void CustomShellSectionRootRendererTest()
		{
			RunningApp.WaitForElement(CreateBottomTabButton);
			RunningApp.Tap(CreateBottomTabButton);
			RunningApp.Screenshot("ShellSection View with Blue and header as Red background color");
			Assert.Inconclusive("Check that ShellSection View with Blue and header as Red background color");
		}
#endif
	}
}