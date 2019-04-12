using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 43527, "[UWP] Detail title does not update when wrapped in a NavigationPage", PlatformAffected.WinRT)]
	public class Bugzilla43527 : TestMasterDetailPage
	{
		protected override void Init()
		{
			Master = new ContentPage
			{
				Title = "Master",
				BackgroundColor = Color.Red
			};

			Detail = new NavigationPage(new TestPage());
		}

		class TestPage : ContentPage
		{
			public TestPage()
			{
				Title = "Test Page";
				AutomationId = "B43527TestPage";

				Content = new StackLayout
				{
					Children = {
						new Label { Text = "Hello Page" },
						new Button { Text = "Change Title", Command = new Command(() => Title = $"New Title: {DateTime.Now.Second}") }
					}
				};
			}
		}

#if UITEST && __WINDOWS__
		[Test]
		public void TestB43527UpdateTitle()
		{
			try
			{

				RunningApp.WaitForElement(q => q.Marked("Change Title"));
				RunningApp.WaitForElement(q => q.Marked("Test Page"));
				RunningApp.Tap(q => q.Marked("Change Title"));
				RunningApp.WaitForElement(q => q.Marked("Test Page"), timeoutMessage: "title changed. Element not found");
			}
			catch (TimeoutException ex)
			{
				Assert.AreEqual("title changed. Element not found", ex.Message);
			}
		}
#endif
	}
}