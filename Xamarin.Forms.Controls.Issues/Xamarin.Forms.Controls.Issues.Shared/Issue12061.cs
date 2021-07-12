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
	[Category(UITestCategories.BoxView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12061,
		"[Bug] [Android] BoxView color is cleared when updating corner radius",
		PlatformAffected.Android)]
	public class Issue12061 : TestContentPage
	{
		public Issue12061()
		{
		}

		protected override void Init()
		{
			Title = "Issue 12061";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor  = Color.White,
				Text = "Tap the Button. The BoxView must be updated with ConerRadius but without losing the Color. If the BoxView Color is kept, the test has passed."
			};

			var boxView = new BoxView
			{
				CornerRadius = 0,
				Color = Color.Red,
				HeightRequest = 60
			};

			var button = new Button
			{
				Text = "Update CornerRadius"
			};

			layout.Children.Add(instructions);
			layout.Children.Add(boxView);
			layout.Children.Add(button);

			Content = layout;

			button.Clicked += (sender, args) =>
			{
				boxView.CornerRadius = 12;
			};
		}
	}
}
