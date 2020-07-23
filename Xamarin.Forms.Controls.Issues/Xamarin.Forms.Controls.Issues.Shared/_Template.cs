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
	public class Issue1 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Content = new Label
			{
				AutomationId = "Issue1Label",
				Text = "See if I'm here"
			};

			BindingContext = new ViewModelIssue1();
		}

#if UITEST
		[Test]
		public void Issue1Test()
		{
			RunningApp.WaitForElement("Issue1Label");
			// Delete this and all other UITEST sections if there is no way to automate the test. Otherwise, be sure to rename the test and update the Category attribute on the class. Note that you can add multiple categories.
			RunningApp.Screenshot("I am at Issue1");
			RunningApp.WaitForElement(q => q.Marked("Issue1Label"));
			RunningApp.Screenshot("I see the Label");
		}
#endif
	}


	public class _11475 : TestNavigationPage
	{
		ContentPage Root() 
		{
			var page = new ContentPage();

			var button = new Button { Text = "Go" };

			button.Clicked += async (sender, args) => {
				DisplayAlert("derp", "derp", "derp");
				await Navigation.PushAsync(Second());
			};

			page.Content = button;

			return page;
		}

		ContentPage Second() 
		{
			var page = new ContentPage();

			var label = new Label { Text = "Hey" };

			page.Content = label;

			return page;
		}

		protected override void Init()
		{
			this.PushAsync(Root());
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewModelIssue1
	{
		public ViewModelIssue1()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class ModelIssue1
	{
		public ModelIssue1()
		{

		}
	}
}