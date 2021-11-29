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
	[Issue(IssueTracker.Github, 12380, "[Bug] Android: Entry Clear button becomes invisible when changing Entry.BackgroundColor to dark color",
		PlatformAffected.Android | PlatformAffected.iOS)]
	public class Issue12380 : TestContentPage
	{
		public Issue12380()
		{
		}

		protected override void Init()
		{
			Title = "Issue 12380";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "The ClearButton must be visible in all cases."
			};

			var defaultLabel = new Label
			{
				Text = "Default ClearButton"
			};

			var defaultEntry = new Entry
			{
				ClearButtonVisibility = ClearButtonVisibility.WhileEditing
			};

			var whiteClearButtonLabel = new Label
			{
				Text = "White ClearButton"
			};

			var whiteClearButtonEntry = new Entry
			{
				BackgroundColor = Color.Black,
				ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
				TextColor = Color.White
			};

			var yellowClearButtonLabel = new Label
			{
				Text = "Yellow ClearButton"
			};

			var yellowClearButtonEntry = new Entry
			{
				BackgroundColor = Color.Black,
				ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
				TextColor = Color.Yellow
			};

			layout.Children.Add(instructions);
			layout.Children.Add(defaultLabel);
			layout.Children.Add(defaultEntry);
			layout.Children.Add(whiteClearButtonLabel);
			layout.Children.Add(whiteClearButtonEntry);
			layout.Children.Add(yellowClearButtonLabel);
			layout.Children.Add(yellowClearButtonEntry);

			Content = layout;
		}
	}
}