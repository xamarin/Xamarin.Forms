namespace Xamarin.Forms
{
	/// <summary>
	/// BooleanPopup is a convenience class for quickly creating popups which require a 
	/// binary (e.g. yes/no, accept/reject, tastes great/less filling) response from a user.
	/// It takes whatever content the creator passes in and adds buttons for an affirmative
	/// and negative response to the bottom. 
	/// The creator can specify the text for the affirmative/negative buttons.
	/// </summary>
	public class BooleanPopup : Popup<bool>
	{
		public BooleanPopup(View content, bool isLightDismissEnabled = true, View anchor = null, Size size = new Size(),
			string affirmativeText = "OK", string negativeText = null)
		{
			IsLightDismissEnabled = isLightDismissEnabled;
			Anchor = anchor;
			Size = size;
			var view = new ContentView();

			// Set up the grid in which to display the user's content and our affirmative/negative buttons
			var layout = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			layout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
			layout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

			layout.Children.Add(content);
			Grid.SetRow(content, 0);
			Grid.SetColumnSpan(content, 2);

			// Create the affirmative button and add it to the layout
			var affirmativeButton = new Button { Text = affirmativeText, HorizontalOptions = LayoutOptions.Center };
			affirmativeButton.Clicked += (sender, args) => Dismiss(true);
			layout.Children.Add(affirmativeButton);
			Grid.SetRow(affirmativeButton, 1);


			if (!string.IsNullOrEmpty(negativeText))
			{
				// If there's going to be a negative button, create it and add it to the bottom row, second column
				var negativeButton = new Button { Text = negativeText, HorizontalOptions = LayoutOptions.Center };
				negativeButton.Clicked += (sender, args) => Dismiss(false);
				layout.Children.Add(negativeButton);
				Grid.SetRow(negativeButton, 1);
				Grid.SetColumn(negativeButton, 1);
			}
			else
			{
				// If there's no negative button, make the affirmative button span the whole row
				Grid.SetColumnSpan(affirmativeButton, 2);
			}

			view.Content = layout;

			// Set this popup's view to the layout we just created
			View = view;
		}

		protected override bool OnLightDismissed()
		{
			return false;
		}
	}
}
