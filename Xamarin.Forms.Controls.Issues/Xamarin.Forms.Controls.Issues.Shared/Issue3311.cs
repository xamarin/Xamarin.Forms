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
	[Issue(IssueTracker.Github, 3311, "RTL is not working for iOS Label with FormattedText", PlatformAffected.Default)]
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
						Text = "This test passes if all proceeding labels are properly right-aligned",
						HorizontalTextAlignment = TextAlignment.Center,
						FontSize = 20
					},
					new Label()
					{
						AutomationId = "Issue3311NormalTextLabel",
						FlowDirection = FlowDirection.RightToLeft,
						BackgroundColor = Color.Red,
						Text = "RTL normal text"
					},
					new Label()
					{
						AutomationId = "Issue3311FormattedTextLabel",
						FlowDirection = FlowDirection.RightToLeft,
						BackgroundColor = Color.Yellow,
						FormattedText = formattedString
					},
					new Label()
					{
						AutomationId = "Issue3311FormattedTextWithLineHeightLabel",
						FlowDirection = FlowDirection.RightToLeft,
						BackgroundColor = Color.Red,
						LineHeight = 3,
						FormattedText = formattedString
					}

				}
			};
		}
}