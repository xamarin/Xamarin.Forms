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
	[Category(UITestCategories.Switch)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12150, "[Bug] Switch control not respecting Material Design on Android",
		PlatformAffected.Android)]
	public class Issue12150 : TestContentPage
	{
		public Issue12150()
		{
		}

		protected override void Init()
		{
			Title = "Issue 12150";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If each Switch has a shadow on the Thumb, the test has passed."
			};

			var defaultRadioButton = new Switch
			{
				HorizontalOptions = LayoutOptions.Center
			};

			var thumbColorRadioButton = new Switch
			{
				HorizontalOptions = LayoutOptions.Center,
				ThumbColor = Color.White
			};

			var onColorRadioButton = new Switch
			{
				HorizontalOptions = LayoutOptions.Center,
				OnColor = Color.Red,
				ThumbColor = Color.White
			};

			layout.Children.Add(instructions);
			layout.Children.Add(defaultRadioButton);
			layout.Children.Add(thumbColorRadioButton);
			layout.Children.Add(onColorRadioButton);

			Content = layout;
		}
	}
}
