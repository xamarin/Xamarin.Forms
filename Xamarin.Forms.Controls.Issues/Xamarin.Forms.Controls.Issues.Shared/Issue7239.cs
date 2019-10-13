using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest.Queries;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7239, "[Bug] Activity restarts with resolution change, page UI doesnt change", PlatformAffected.Android)]
#if UITEST
	[Category(UITestCategories.Page)]
#endif
	public class Issue7239 : TestContentPage
	{
		const string RefreshButton = "RefreshButton";
		const string ChangeDensityButton = "ChangeDensityButton";

		protected override void Init()
		{
			var densityLabel = new DensityLabel()
			{
				FontSize = 80
			};

			Content = new StackLayout()
			{
				Children =
				{
					densityLabel,
					new ChangeDensityButton(),
					new Button()
					{
						AutomationId = RefreshButton,
						Text = "Click to refresh",
						Command = new Command(()=> densityLabel?.RefreshValue())
					}
				}
			};
		}

#if UITEST && __ANDROID__
		[Test]
		[UiTest(typeof(Page))]
		public void Issue7239TestDensityChangeIssue()
		{
			RunningApp.WaitForElement(RefreshButton);
			RunningApp.Tap(ChangeDensityButton);
			RunningApp.Tap(RefreshButton);

			var finalDensity = RunningApp.GetScreenDensity();
			Assert.AreEqual(1.5f, finalDensity);
		}
#endif
	}
}