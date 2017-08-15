using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 1234, "Platform Renderer Demo", PlatformAffected.Default)]
	public class PlatformRendererDemo : TestContentPage
	{
		protected override void Init()
		{
			var layout = new StackLayout();

			var pr_label = new PR_Label
			{
				AutomationId = "PR_Label",
				Text = "PR_Label"
			};
			layout.Children.Add(pr_label);

			var label = new Label
			{
				AutomationId = "Label",
				Text = "Label"
			};
			layout.Children.Add(label);

			Content = layout;
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
}