using System;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls
{
	[Preserve (AllMembers=true)]
	[Issue (IssueTracker.None, 0, "Default colors toggle test", PlatformAffected.All)]
	public class DefaultColorToggleTest : TabbedPage
	{
		public DefaultColorToggleTest()
		{
			Title = "Test Color Toggle Page";
			
			Children.Add(TimePickerPage());
			Children.Add(ButtonPage());
			Children.Add(LabelPage());
			Children.Add(EntryPage());
			Children.Add(PasswordPage());
			Children.Add(SearchBarPage());
		}

		static ContentPage PasswordPage()
		{
			var passwordColorDefaultToggle = new Entry() { IsPassword = true, Text = "Default Password Color" };
			var passwordColorInit = new Entry() { IsPassword = true, Text = "Should Be Red", TextColor = Color.Red };
			var passwordToggleButton = new Button() { Text = "Toggle Password Box (Default)" };
			passwordToggleButton.Clicked += (sender, args) => {
				if (passwordColorDefaultToggle.TextColor.IsDefault)
				{
					passwordColorDefaultToggle.TextColor = Color.Red;
					passwordToggleButton.Text = "Toggle Password Box (Red)";
				}
				else
				{
					passwordColorDefaultToggle.TextColor = Color.Default;
					passwordToggleButton.Text = "Toggle Password Box (Default)";
				}
			};

			return new ContentPage {
				Title = "Password",
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Fill,
					HorizontalOptions = LayoutOptions.Fill,
					Children =
					{
						passwordColorDefaultToggle,
						passwordToggleButton,
						passwordColorInit
					}
				}
			};
		}

		static ContentPage SearchBarPage()
		{
			var searchbarTextColorDefaultToggle = new Entry() { Text = "Default SearchBar Text Color" };
			var searchbarTextColorToggleButton = new Button() { Text = "Toggle SearchBar Color" };
			searchbarTextColorToggleButton.Clicked += (sender, args) => {
				if (searchbarTextColorDefaultToggle.TextColor.IsDefault)
				{
					searchbarTextColorDefaultToggle.TextColor = Color.Fuchsia;
					searchbarTextColorDefaultToggle.Text = "Should Be Fuchsia";
				}
				else
				{
					searchbarTextColorDefaultToggle.TextColor = Color.Default;
					searchbarTextColorDefaultToggle.Text = "Default SearchBar Text Color";
				}
			};

			var searchbarPlaceholderColorDefaultToggle = new Entry() { Placeholder = "Default Placeholder Color" };
			var searchbarPlaceholderToggleButton = new Button() { Text = "Toggle Placeholder Color" };
			searchbarPlaceholderToggleButton.Clicked += (sender, args) => {
				if (searchbarPlaceholderColorDefaultToggle.PlaceholderColor.IsDefault)
				{
					searchbarPlaceholderColorDefaultToggle.PlaceholderColor = Color.Lime;
					searchbarPlaceholderColorDefaultToggle.Placeholder = "Should Be Lime";
				}
				else
				{
					searchbarPlaceholderColorDefaultToggle.PlaceholderColor = Color.Default;
					searchbarPlaceholderColorDefaultToggle.Placeholder = "Default Placeholder Color";
				}
			};

			return new ContentPage {
				Title = "SearchBar",
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Fill,
					HorizontalOptions = LayoutOptions.Fill,
					Children =
					{
						searchbarTextColorDefaultToggle,
						searchbarTextColorToggleButton,
						searchbarPlaceholderColorDefaultToggle,
						searchbarPlaceholderToggleButton
					}
				}
			};
		}

		static ContentPage ButtonPage()
		{
			const string defaultLabel = "Default Color (Click To Toggle)";

			var buttonColorDefaultToggle = new Button {
				Text = defaultLabel
			};

			var buttonColorInitted = new Button {
				Text = "This Should Always Be Red",
				TextColor = Color.Red
			};

			buttonColorDefaultToggle.Clicked += (s, e) => {
				if (buttonColorDefaultToggle.TextColor == Color.Default)
				{
					buttonColorDefaultToggle.TextColor = Color.Red;
					buttonColorDefaultToggle.Text = "Custom Color (Click To Toggle)";
				}
				else
				{
					buttonColorDefaultToggle.TextColor = Color.Default;
					buttonColorDefaultToggle.Text = defaultLabel;
				}

			};

			return new ContentPage {
				Title = "Button",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Fill,
					HorizontalOptions = LayoutOptions.Fill,
					Children =
					{
						buttonColorDefaultToggle,
						buttonColorInitted
					}
				}
			}; 
		}

		static ContentPage LabelPage()
		{
			const string defaultText = "Default Label Color (Tap To Toggle)";

			var labelColorDefaultToggle = new Label {
				Text = defaultText
			};

			var labelColorInitted = new Label {
				Text = "Should Always Be Blue",
				TextColor = Color.Blue
			};

			labelColorDefaultToggle.GestureRecognizers.Add(new TapGestureRecognizer {
				Command = new Command(o => {
					if (labelColorDefaultToggle.TextColor == Color.Default)
					{
						labelColorDefaultToggle.TextColor = Color.Green;
						labelColorDefaultToggle.Text = "Custom Label Color (Tap To Toggle)";
					}
					else
					{
						labelColorDefaultToggle.TextColor = Color.Default;
						labelColorDefaultToggle.Text = defaultText;
					}
				})
			});

			return new ContentPage {
				Title = "Label",
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Fill,
					HorizontalOptions = LayoutOptions.Fill,
					Children =
					{
						labelColorDefaultToggle,
						labelColorInitted
					}
				}
			};
		}

		static ContentPage EntryPage()
		{
			var entryTextColorInit = new Entry { Text = "Should Always Be Red", TextColor = Color.Red };
			var entryPlaceholderColorInit = new Entry { Placeholder = "Should Always Be Lime", PlaceholderColor = Color.Lime };

			const string defaultEntryColor = "Default Entry Text Color";
			var entryTextColorDefaultToggle = new Entry { Text = defaultEntryColor };

			var entryToggleButton = new Button { Text = "Toggle Entry Color" };
			entryToggleButton.Clicked += (sender, args) =>
			{
				if (entryTextColorDefaultToggle.TextColor.IsDefault)
				{
					entryTextColorDefaultToggle.TextColor = Color.Fuchsia;
					entryTextColorDefaultToggle.Text = "Should Be Fuchsia";
				}
				else
				{
					entryTextColorDefaultToggle.TextColor = Color.Default;
					entryTextColorDefaultToggle.Text = defaultEntryColor;
				}
			};

			const string defaultPlaceholderColorText = "Default Placeholder Color";
			var entryPlaceholderColorDefaultToggle = new Entry { Placeholder = defaultPlaceholderColorText };

			var entryPlaceholderToggleButton = new Button { Text = "Toggle Placeholder Color" };
			entryPlaceholderToggleButton.Clicked += (sender, args) =>
			{
				if (entryPlaceholderColorDefaultToggle.PlaceholderColor.IsDefault)
				{
					entryPlaceholderColorDefaultToggle.PlaceholderColor = Color.Lime;
					entryPlaceholderColorDefaultToggle.Placeholder = "Should Be Lime";
				}
				else
				{
					entryPlaceholderColorDefaultToggle.PlaceholderColor = Color.Default;
					entryPlaceholderColorDefaultToggle.Placeholder = defaultPlaceholderColorText;
				}
			};

			return new ContentPage
			{
				Title = "Entry",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Fill,
					HorizontalOptions = LayoutOptions.Fill,
					Children =
					{
						entryTextColorDefaultToggle,
						entryToggleButton,
						entryTextColorInit,
						entryPlaceholderColorDefaultToggle,
						entryPlaceholderToggleButton,
						entryPlaceholderColorInit
					}
				}
			};
		}

		static ContentPage TimePickerPage()
		{
			var timePickerInit = new TimePicker { Time = new TimeSpan(11, 34, 00), TextColor = Color.Red };
			var timePickerColorDefaultToggle = new TimePicker { Time = new TimeSpan(11, 34, 00) };

			var defaultText = "Should have default color text";
			var label = new Label() {Text = defaultText };

			var toggleButton = new Button { Text = "Toggle TimePicker Text Color" };
			toggleButton.Clicked += (sender, args) => {
				if (timePickerColorDefaultToggle.TextColor.IsDefault)
				{
					timePickerColorDefaultToggle.TextColor = Color.Fuchsia;
					label.Text = "Should have fuchsia text";
				}
				else
				{
					timePickerColorDefaultToggle.TextColor = Color.Default;
					label.Text = defaultText;
				}
			};

			const string defaultPlaceholderColorText = "Default Placeholder Color";
			var entryPlaceholderColorDefaultToggle = new Entry { Placeholder = defaultPlaceholderColorText };

			var entryPlaceholderToggleButton = new Button { Text = "Toggle Placeholder Color" };
			entryPlaceholderToggleButton.Clicked += (sender, args) => {
				if (entryPlaceholderColorDefaultToggle.PlaceholderColor.IsDefault)
				{
					entryPlaceholderColorDefaultToggle.PlaceholderColor = Color.Lime;
					entryPlaceholderColorDefaultToggle.Placeholder = "Should Be Lime";
				}
				else
				{
					entryPlaceholderColorDefaultToggle.PlaceholderColor = Color.Default;
					entryPlaceholderColorDefaultToggle.Placeholder = defaultPlaceholderColorText;
				}
			};

			return new ContentPage {
				Title = "TimePicker",
				Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, Device.OnPlatform(00, 0, 0)),
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Fill,
					HorizontalOptions = LayoutOptions.Fill,
					Children =
					{
						label,
						timePickerColorDefaultToggle,
						toggleButton,
						timePickerInit
					}
				}
			};
		}
	}
}
