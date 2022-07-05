using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 15066, "StackLayout's layout inside ScrollView is not updated properly when adding children",
		PlatformAffected.Android)]
	public class Issue15066 : TestContentPage
	{
		protected override void Init()
		{
			var instructions = new Label
			{
				BackgroundColor = Color.AntiqueWhite,
				Padding = 3,
				Text = "Tap the 'Add' button until the added items are past the bottom of the screen." +
					" Then scroll down - if there is any blue visible at the bottom, this test has failed."
			};

			var scrollView = new ScrollView() { BackgroundColor = Color.DarkBlue };

			var layout = new StackLayout() { BackgroundColor = Color.DarkGreen };

			var button = new Button() { Text = "Add" };

			button.Clicked += (sender, args) =>
			{
				layout.Children.Add(new StackLayout() { BackgroundColor = Color.Gray, HeightRequest = 40.0 });
			};

			layout.Children.Add(instructions);
			layout.Children.Add(button);

			for (int n = 0; n < 8; n++)
			{
				layout.Children.Add(new StackLayout() { BackgroundColor = Color.Gray, HeightRequest = 40.0 });
			}

			scrollView.Content = layout;

			Content = scrollView;
		}
	}
}
