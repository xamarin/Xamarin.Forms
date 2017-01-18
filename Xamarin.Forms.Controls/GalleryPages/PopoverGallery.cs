using System;
using System.Linq;

namespace Xamarin.Forms.Controls
{
	public class PopoverGallery : ContentPage
	{
		const string Placeholder =
			"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras ante dolor, maximus non dignissim non, pellentesque id felis. Etiam accumsan leo et eleifend efficitur. Donec rutrum euismod auctor. Integer metus ante, blandit eget nisl eget, egestas imperdiet libero. Sed lectus purus, placerat quis pretium nec, ullamcorper a orci. Duis eget varius purus, et mollis metus. Sed sed mi vitae justo placerat venenatis ut sit amet sem. Etiam nec neque sit amet tellus mollis faucibus. Aliquam nec urna at leo imperdiet consectetur. Quisque turpis diam, feugiat eu maximus vel, elementum mattis sem.";

		const string ResultTitle = "Popup Result";
		const string DismissText = "Cool, thanks!";

		public PopoverGallery ()
		{
			var layout = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			var top = new StackLayout { Children = { PopoverWithLabel(), PopoverWithLayout(), TermsOfServicePopup() } };
			Grid.SetRow(top, 0);

			// Putting one of the buttons on the bottom so if we're on the iPad we can see the little popover arrows working
			var bottom = new StackLayout { Children = { PopoverWithDatePicker() } };
			Grid.SetRow(bottom, 1);

			layout.Children.Add(top);
			layout.Children.Add(bottom);

			Content = layout;
		}

		Button CreateButton(string text, string automationId, View content)
		{
			var button = new Button { Text = text, AutomationId = automationId };
			button.Clicked += async (sender, e) =>
			{
				var popover = new Popup(content, text, anchor: button);

				var result = await Navigation.ShowPopup(popover);

				await DisplayAlert(ResultTitle, result.ToString(), DismissText);
			};

			return button;
		}

		Button PopoverWithLabel()
		{
			return CreateButton("Popup with Label", "testPopupWithLabel",
				new Label
				{
					Text = "This is just a Label inside of a Popup. This is a basic popup which takes a View, and is only light-dismissable.",
					LineBreakMode = LineBreakMode.WordWrap,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					HorizontalOptions = LayoutOptions.Fill,
					VerticalOptions = LayoutOptions.Fill
				});
		}

		Button PopoverWithLayout()
		{
			var content = new StackLayout
			{
				Margin = new Thickness(20),
				Children =
				{
					new Image { Source = "coffee.png", Margin = new Thickness(5)},
					new Label { LineBreakMode = LineBreakMode.WordWrap, Margin = new Thickness(5), Text = "This is a popup with a Layout as its content." },
					new Label { LineBreakMode = LineBreakMode.WordWrap, Text = Placeholder }
				}
			};

			return CreateButton("Popup with Layout", "testPopupWithLayout", content);
		}


		Button PopoverWithDatePicker()
		{
			var button = new Button { Text = "Popup with DatePicker" };

			button.Clicked += async (sender, e) =>
			{
				// Create a DateChooserPopup which uses DateChooserPopupControl as its content and is anchored to this button
				var popup = new DateChooserPopup(new DateChooserPopupControl(), "Select Date", button);

				// Show the popup and await the DateTime? result
				DateTime? result = await Navigation.ShowPopup(popup);

				// Display the user's selection
				await DisplayAlert(ResultTitle, result?.ToString(), DismissText);
			};

			return button;
		}

		class DateChooserPopup : Popup<DateTime?>
		{
			public DateChooserPopup(IPopupView<DateTime?> popupView, string title = null, View anchor = null) 
				: base(popupView, title, false, anchor) 
			{
				// Note that this popup is not light-dismissable
			}

			protected override DateTime? OnLightDismissed()
			{
				return null;
			}
		}

		class DateChooserPopupControl : ContentView, IPopupView<DateTime?>
		{
			public View View => this;
			Action<DateTime?> _dismiss;

			public void SetDismissDelegate(Action<DateTime?> dismissDelegate)
			{
				// Keep track of the dismiss delegate so we can tell the popup what the user selects
				_dismiss = dismissDelegate;
			}

			public DateChooserPopupControl()
			{
				// Build the UI for our popup
				var datePicker = new DatePicker { HorizontalOptions = LayoutOptions.Center };
				var button = new Button { Text = "OK", HorizontalOptions = LayoutOptions.Center };

				Content = new StackLayout
				{
					Margin = new Thickness(40),
					Children =
					{
						new Label
						{
							Text = "Choose a Date",
							HorizontalOptions = LayoutOptions.Center,
							HorizontalTextAlignment = TextAlignment.Center
						},
						datePicker,
						button
					}
				};

				// When the user clicks OK, we report the result back to the popup
				button.Clicked += (sender, args) => { _dismiss?.Invoke(datePicker.Date); };
			}
		}

		Button TermsOfServicePopup()
		{
			var button = new Button { Text = "Terms of Service Popup" };

			// Define the content which goes into the popup (in this case, a scrollview with some really long text to read)
			var tos = new Label
			{
				LineBreakMode = LineBreakMode.WordWrap,
				Text = "This example demonstrates a convenience method for creating a popup which returns a binary result (e.g. yes/no, accept/reject, ok/cancel).\n\n" 
				+  string.Concat(Enumerable.Repeat(Placeholder + "\n", 10))
			};

			var scrollView = new ScrollView { Content = tos, Margin = new Thickness(20) };
			
			// Create the popup, specifying the text for the buttons
			var tosPopup = new BinaryResultPopup(scrollView, "Terms of Service", anchor: button, affirmativeText: "Accept", negativeText: "Reject", size: new Size(400, 500));

			button.Clicked += async (sender, args) =>
			{
				// Reset the popup in case we've already gotten a result from it
				tosPopup.Reset();

				// Display the popup and await the result
				var result = await Navigation.ShowPopup(tosPopup);

				// Show the result that we just got back from the popup
				await DisplayAlert(ResultTitle, result.ToString(), DismissText);
			};

			return button;
		}
	}
}