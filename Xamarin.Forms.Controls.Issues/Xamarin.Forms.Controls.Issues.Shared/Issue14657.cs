using System;
using System.Threading;
using System.Threading.Tasks;
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
	[Category(UITestCategories.Github10000)]
	[Category(UITestCategories.Shell)]
	[Category(UITestCategories.TitleView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14657, "Shell pages are not released from memory", PlatformAffected.Android)]
	public class Issue14657 : TestShell
	{
		static int pageCount = 0;
		protected override void Init()
		{
			Routing.RegisterRoute(nameof(Issue14657_ChildPage), typeof(Issue14657_ChildPage));

			var rootPage = CreateRootPage();

			AddContentPage(rootPage);
		}

		ContentPage CreateRootPage()
		{
			var rootPage = CreateContentPage("Home page");
			rootPage.Content = new StackLayout()
			{
				Children =
				{
					new Button()
					{
						Command = new Command(CollectMemory),
						Text = "Force GC",
						AutomationId = "GC_14657"
					},
					new Button()
					{
						Command = new Command(async () => await GoToChild()),
						Text = "Go to child page",
						AutomationId = "GoToChild_14657"
					}
				}
			};
			Shell.SetTitleView(rootPage, new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				Children = { new Label { Text = "Root Page" } }
			});

			return rootPage;
		}

		public class Issue14657_ChildPage : ContentPage
		{
			public Issue14657_ChildPage()
			{
				Interlocked.Increment(ref pageCount);

				Content = new StackLayout
				{
					Children =
					{
						new Label()
						{
							Text = $"{pageCount}",
							AutomationId = "CountLabel",
							TextColor = Color.Black
						},
						new Button()
						{
							Command = new Command(CollectMemory),
							Text = "Force GC",
							AutomationId = "GC_14657"
						}
					}
				};
			}

			~Issue14657_ChildPage()
			{
				Interlocked.Decrement(ref pageCount);
			}
		}

		async Task GoToChild()
		{
			await GoToAsync(nameof(Issue14657_ChildPage));
		}

		static void CollectMemory()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

#if UITEST
		[Test]
		public void Issue14657Test()
		{
			RunningApp.Tap("GoToChild_14657");
			RunningApp.WaitForFirstElement("CountLabel")
				.AssertHasText("1");
			RunningApp.NavigateBack();
			RunningApp.Tap("GC_14657");

			RunningApp.Tap("GoToChild_14657");
			RunningApp.WaitForFirstElement("CountLabel")
				.AssertHasText("1");
			RunningApp.NavigateBack();
			RunningApp.Tap("GC_14657");

			RunningApp.Tap("GoToChild_14657");
			RunningApp.WaitForFirstElement("CountLabel")
				.AssertHasText("1");
			RunningApp.NavigateBack();
		}

#endif
	}
}