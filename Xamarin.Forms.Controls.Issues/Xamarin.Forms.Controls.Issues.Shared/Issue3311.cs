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
	[Issue(IssueTracker.Github, 3311, "Issue Description", PlatformAffected.Default)]
	public class Issue3311 : TestContentPage 
	{
		protected override void Init()
		{

			var formattedString = new FormattedString();
			formattedString.Spans.Add(new Span { Text = "RTL \nformatted \ntext" });

			Content = new StackLayout()
			{
				Margin = 20,

				Children =
				{
					new Label()
					{
						AutomationId = "Issue3311Label",
						Text = "Issue 3311: RTL is not working for Label.FormattedText on iOS. This test passes if all proceeding labels are properly right-aligned.",
						HorizontalTextAlignment = TextAlignment.Center,
						FontSize = 20
					},
					new Label()
					{
						AutomationId = "Issue3311NormalTextLabel",
						FlowDirection = FlowDirection.RightToLeft,
						Text = "RTL normal text"
					},
					new Label()
					{
						AutomationId = "Issue3311FormattedTextLabel",
						FlowDirection = FlowDirection.RightToLeft,
						BackgroundColor = Color.Red,
						FormattedText = formattedString
					},
					new Label()
					{
						AutomationId = "Issue3311FormattedTextWithLineHeightLabel",
						FlowDirection = FlowDirection.RightToLeft,
						LineHeight = 3,
						BackgroundColor = Color.Red,
						FormattedText = formattedString
					}

				}
			};
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
}