using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Editor)]
#endif
	[Issue(IssueTracker.Github, 12716,
		"[Bug] [UWP] Editor > Unfocused not triggered when pressing back button",
		PlatformAffected.UWP)]
	public class Issue12716 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Issue 12716";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Tap the Button to navigate."
			};

			var navigateButton = new Button
			{
				Text = "Navigate"
			};

			layout.Children.Add(instructions);
			layout.Children.Add(navigateButton);

			Content = layout;

			navigateButton.Clicked += OnNavigateButtonClicked;
		}

		void OnNavigateButtonClicked(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Issue12716SecondPage());
		}
	}

	public class Issue12716SecondPage : ContentPage
	{
		public Issue12716SecondPage()
		{
			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Set focus in the Editor.Press the back button in top left corner of the app. If a dialog appears, the test has passed."
			};

			var editor = new Editor();

			layout.Children.Add(instructions);
			layout.Children.Add(editor);

			Content = layout;

			editor.Unfocused += OnEditorUnfocused;
		}

		void OnEditorUnfocused(object sender, FocusEventArgs e)
		{
			DisplayAlert("Issue 12716", "Editor unfocused", "Ok");
		}
	}
}