using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1026, "Label cropping", PlatformAffected.iOS | PlatformAffected.WinPhone, NavigationBehavior.PushModalAsync)]
	public class Issue1026 : ContentPage
	{
		public Issue1026()
		{
			var instructions = new Label
			{
				Text = "The label at the bottom of the form should read 'by subscribing," +
				" you accept the general conditions.'; if the label is truncated, this test has failed."
			};

			var scrollView =
				new ScrollView
				{
					BackgroundColor = Color.FromHex("#dae1eb"),
					Content =
					new StackLayout
					{
						Padding = new Thickness(0, 18),
						Spacing = 10,
						Orientation = StackOrientation.Vertical,
						Children = {
						new Button {
							BackgroundColor = Color.FromHex ("#006599"),
							TextColor = Color.White,
							Text = "Subscribe with LinkedIn",
							WidthRequest = 262,
							HorizontalOptions = LayoutOptions.Center,
							CornerRadius = 0,
						},
						new Entry {
							Placeholder = "Professional email",
							WidthRequest = 262,
							HorizontalOptions = LayoutOptions.Center,
							Keyboard = Keyboard.Email,
						},
						new Entry {
							Placeholder = "Firstname",
							WidthRequest = 262,
							HorizontalOptions = LayoutOptions.Center,
						},
						new Entry {
							Placeholder = "Lastname",
							WidthRequest = 262,
							HorizontalOptions = LayoutOptions.Center,
						},
						new Entry {
							Placeholder = "Company",
							WidthRequest = 262,
							HorizontalOptions = LayoutOptions.Center,
						},
						new Entry {
							Placeholder = "Password",
							WidthRequest = 262,
							IsPassword = true,
							HorizontalOptions = LayoutOptions.Center,
						},
						new Entry {
							Placeholder = "Confirm password",
							WidthRequest = 262,
							IsPassword = true,
							HorizontalOptions = LayoutOptions.Center,
						},
						new Button {
							BackgroundColor = Color.FromHex ("#05addc"),
							TextColor = Color.White,
							Text = "Create an account",
							WidthRequest = 262,
							HorizontalOptions = LayoutOptions.Center,
							CornerRadius = 0,
						},
						new Label {
							Text = "by subscribing, you accept the general conditions.",
							TextColor = Color.White,
							HorizontalTextAlignment = TextAlignment.Center,
							FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
							WidthRequest = 262,
							HorizontalOptions = LayoutOptions.Center,
						},
					},
					},

				};

			Content = new StackLayout { Children = { instructions, scrollView } };
		}
	}
}
