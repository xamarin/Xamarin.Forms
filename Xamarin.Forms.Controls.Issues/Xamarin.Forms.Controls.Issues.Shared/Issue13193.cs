using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
	[Category(UITestCategories.Accessibility)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13193,
		"Double prompt when voice over reads picker item on iOS",
		PlatformAffected.iOS)]
	public class Issue13193 : TestContentPage
	{
		public Issue13193()
		{
		}

		protected override void Init()
		{
			Title = "Issue 13193";

			var layout = new StackLayout
			{
				Padding = 12
			};

			var instructions = new Label
			{
				Text = "Turn VoiceOver on, select the picker, double-tap to edit, select the input view, and swipe up to navigate through the picker items.  If an item or a truncated portion of an item is read twice, the test failed"
			};

			var picker = new Picker
			{
				ItemsSource = new List<string>
				{
					"Bananas",
					"New York",
					"Grapes"
				}
			};

			layout.Children.Add(instructions);
			layout.Children.Add(picker);

			Content = layout;

		}
	}
}