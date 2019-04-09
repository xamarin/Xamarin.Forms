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
	[Issue(IssueTracker.Github, 1, "Issue Description", PlatformAffected.Default)]
	public class Github1 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Content = new Label
			{
				AutomationId = "Github1Label",
				Text = "See if I'm here"
			};

			BindingContext = new ViewModelGithub1();
		}

#if UITEST
		[Test]
		public void Github1Test() 
		{
			// Delete this and all other UITEST sections if there is no way to automate the test. Otherwise, be sure to rename the test and update the Category attribute on the class. Note that you can add multiple categories.
			RunningApp.Screenshot("I am at Github 1");
			RunningApp.WaitForElement(q => q.Marked("Github1Label"));
			RunningApp.Screenshot("I see the Label");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class ViewModelGithub1
	{
		public ViewModelGithub1()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class ModelGithub1
	{
		public ModelGithub1()
		{

		}
	}
}