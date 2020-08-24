using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Label)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11829,
		"[Bug] TextDecoration Strikethrough not working on iOS together with LineHeight",
		PlatformAffected.All)]
	public class Issue11829 : TestContentPage
	{
		public Issue11829()
		{

		}

		protected override void Init()
		{
			Title = "Issue 11829";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If the text below is underline, the test has passed."
			};

			var label = new Label
			{
				TextDecorations = TextDecorations.Underline,
				LineHeight = 2,
				Text = "Underline using LineHeight"
			};

			layout.Children.Add(instructions);
			layout.Children.Add(label);

			Content = layout;
		}
	}
}
