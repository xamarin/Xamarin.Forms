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

			var top = new StackLayout { Children = { PopoverWithLabel(), PopoverWithLayout(), PopoverWithCloseButtonLayout(), PopoverWithCloseButtonLayoutNoSize(), TermsOfServicePopup(), BindablePopup() } };
			Grid.SetRow(top, 0);

			// Putting one of the buttons on the bottom so if we're on the iPad we can see the little popover arrows working
			var bottom = new StackLayout { Children = { PopoverWithDatePicker() } };
			Grid.SetRow(bottom, 1);

			layout.Children.Add(top);
			layout.Children.Add(bottom);

			Content = layout;
		}

		Button CreateButton(string text, string automationId, View content, Color color, bool isAnchored = false, Size size = default(Size), Color border = default(Color))
		{
			var button = new Button { Text = text, AutomationId = automationId };
			button.Clicked += async (sender, e) =>
			{
				var popover = isAnchored ?
					new Popup(content) { Anchor = button, Size = size, Color = color, BorderColor = border } :
					new Popup(content) { Size = size, Color = color, BorderColor = border };

				var result = await Navigation.ShowPopup(popover);

				await DisplayAlert(ResultTitle, "Popup Dismissed", DismissText);
			};

			return button;
		}

		Button PopoverWithLabel()
		{
			var button = new Button
			{
				Text = "Label Popovers"
			};
			button.Clicked += async (s, e) => await Navigation.PushAsync(new PopupLabelGallery());
			return button;
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

			return CreateButton("Popup with Layout", "testPopupWithLayout", content, Color.White, size: new Size(800,800));
		}

		Button PopoverWithCloseButtonLayout()
		{
			var content = new Grid
			{
				Margin = new Thickness(20),
				Padding = new Thickness(20),
				Children =
				{
					new Label { LineBreakMode = LineBreakMode.WordWrap, Text = Placeholder, BackgroundColor = Color.White },
					new Frame
					{
						HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true),
						VerticalOptions = new LayoutOptions(LayoutAlignment.Start, true),
						Margin = new Thickness(0, -30, 0, 0),
						BackgroundColor = Color.Orange
					}
				}
			};

			return CreateButton("Popup with Close Button Layout", "testPopupWithCloseButtonLayout", content, Color.Transparent, size: new Size(500,500), border: Color.Transparent);
		}

		Button PopoverWithCloseButtonLayoutNoSize()
		{
			var content = new Grid
			{
				Margin = new Thickness(20),
				Padding = new Thickness(20),
				Children =
				{
					new Label { LineBreakMode = LineBreakMode.WordWrap, Text = Placeholder, BackgroundColor = Color.White },
					new Frame
					{
						HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true),
						VerticalOptions = new LayoutOptions(LayoutAlignment.Start, true),
						Margin = new Thickness(0, -30, 0, 0),
						BackgroundColor = Color.Orange
					}
				}
			};

			return CreateButton("Popup with Close Button Layout No Size", "testPopupWithCloseButtonLayout", content, Color.Transparent, border: Color.Transparent);
		}


		Button PopoverWithDatePicker()
		{
			var button = new Button { Text = "Popup with DatePicker" };

			button.Clicked += async (sender, e) =>
			{
				// Create a DateChooserPopup which uses DateChooserPopupControl as its content and is anchored to this button
				var popup = new DateChooserPopup(new DateChooserPopupControl(), button);

				// Show the popup and await the DateTime? result
				DateTime? result = await Navigation.ShowPopup(popup);

				// Display the user's selection
				await DisplayAlert(ResultTitle, result?.ToString(), DismissText);
			};

			return button;
		}

		class DateChooserPopup : Popup<DateTime?>
		{
			public DateChooserPopup(View popupView, View anchor = null, Size size = default(Size)) 
			{
				View = popupView;
				Anchor = anchor;
				Size = size;
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

		// TODO - This does not work in UWP for some reason, it needs more investigation
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

			var scrollView = new ScrollView { Content = tos, Margin = new Thickness(20), HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true) };
			
			
			

			button.Clicked += async (sender, args) =>
			{
				// Create the popup, specifying the text for the buttons
				var tosPopup = new BooleanPopup(scrollView, anchor: button, affirmativeText: "Accept", negativeText: "Reject", size: new Size(800, 400));
				
				// Reset the popup in case we've already gotten a result from it
				tosPopup.Reset();

				// Display the popup and await the result
				var result = await Navigation.ShowPopup(tosPopup);

				// Show the result that we just got back from the popup
				await DisplayAlert(ResultTitle, result.ToString(), DismissText);
			};

			return button;
		}

		Button BindablePopup()
		{
			var button = new Button { Text = "Bindable Properties Popup" };

			button.Clicked += async (sender, args) =>
			{
				// Create the popup, specifying the text for the buttons
				var bindablePopup = new BindablePopup();

				// Reset the popup in case we've already gotten a result from it
				bindablePopup.Reset();

				// Display the popup and await the result
				await Navigation.ShowPopup(bindablePopup);
			};

			return button;
		}
	}
}