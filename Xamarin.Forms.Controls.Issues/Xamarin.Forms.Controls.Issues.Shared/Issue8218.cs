using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8218, "[Bug] -UWP - Device.Idiom is not updated when flipping tablet mode state", PlatformAffected.UWP)]
	class Issue8218 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Issue 8218";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				Text = "Switch to the tablet mode and tap the button. If the Idiom value is Tablet, the test has passed."
			};

			var idiomButton = new Button
			{
				HorizontalOptions = LayoutOptions.Center,
				Text = "Idiom",
				Margin = new Thickness(0, 24)
			};

			layout.Children.Add(instructions);
			layout.Children.Add(idiomButton);

			Content = layout;

			idiomButton.Clicked += (sender, args) =>
			{
				DisplayAlert("Issue 8218", Device.Idiom.ToString(), "Ok");
			};
		}
	}
}