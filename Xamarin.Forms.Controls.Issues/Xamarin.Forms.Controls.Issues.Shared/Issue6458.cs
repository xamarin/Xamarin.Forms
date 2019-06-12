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
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6458, "[Android] Fix load TitleIcon on non app compact", PlatformAffected.Android)]
	public class Issue6458 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			NavigationPage.SetTitleIconImageSource(this, new FileImageSource { File = "bank.png" });
			Content = new Label
			{
				AutomationId = "IssuePageLabel",
				Text = "Make sure you run this on Non AppCompact Activity"
			};
		}

#if UITEST && __ANDROID__
		[Test]
		public void Issue6458Test()
		{
			RunningApp.WaitForElement(q => q.Marked("IssuePageLabel"));
			RunningApp.Screenshot("You should see the bank icon on the ActionBar");
		}
#endif
	}
}