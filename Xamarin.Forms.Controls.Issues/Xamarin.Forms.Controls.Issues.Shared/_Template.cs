using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 1, "Issue Description", PlatformAffected.Default)]
	public class Bugzilla1 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Content = new Label
			{
				AutomationId = "IssuePageLabel",
				Text = "See if I'm here"
			};
		}

#if UITEST
		[Test]
		public void Issue1Test ()
		{
			RunningApp.Screenshot ("I am at Issue 1");
			RunningApp.WaitForElement (q => q.Marked ("IssuePageLabel"));
			RunningApp.Screenshot ("I see the Label");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1396, "Label HorizontalTextAlignment (Center or End) is not kept when navigating back to a page", PlatformAffected.Android)]
	public class Issue1396 : TestNavigationPage
	{
		protected override void Init()
		{
			var button = new Button { Text = "Go" };
			button.Clicked += Button_Clicked;

			var label = new Label
			{
				Text = "I should be centered",
				HorizontalTextAlignment = TextAlignment.Center
			};

			var layout = new StackLayout {
				Children = { button, label },
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill
			};

			var content = new ContentPage {
				Content = layout 
			};

			PushAsync(content);
		}

		private void Button_Clicked(object sender, System.EventArgs e)
		{
			PushAsync(new ContentPage { Content = new Label { Text = "Page 2" } });
		}
	}
}