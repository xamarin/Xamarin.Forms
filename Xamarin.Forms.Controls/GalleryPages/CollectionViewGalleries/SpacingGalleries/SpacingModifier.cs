using System;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.SpacingGalleries
{
	internal class SpacingModifier : ContentView
	{
		protected readonly CollectionView _cv;
		protected readonly Entry Entry;

		public SpacingModifier(CollectionView cv, string buttonText)
		{
			_cv = cv;

			var layout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Fill
			};

			var button = new Button { Text = buttonText, AutomationId = $"btn{buttonText}" };
			var label = new Label { Text = LabelText, VerticalTextAlignment = TextAlignment.Center };

			Entry = new Entry { Keyboard = Keyboard.Numeric, Text = InitialEntryText, WidthRequest = 100, AutomationId = $"entry{buttonText}" };

			layout.Children.Add(label);
			layout.Children.Add(Entry);
			layout.Children.Add(button);

			button.Clicked += ButtonOnClicked;

			Content = layout;
		}

		void ButtonOnClicked(object sender, EventArgs e)
		{
			OnButtonClicked();
		}

		protected virtual string LabelText => "Spacing:";

		protected virtual string InitialEntryText => "0";

		protected virtual void OnButtonClicked()
		{
			if (!ParseIndexes(out int[] indexes))
			{
				return;
			}

			if (_cv.ItemsLayout is ListItemsLayout listItemsLayout)
			{
				listItemsLayout.ItemSpacing = indexes[0];
			}
		}

		protected virtual bool ParseIndexes(out int[] indexes)
		{
			if (!int.TryParse(Entry.Text, out int index))
			{
				indexes = new int[0];
				return false;
			}

			indexes = new[] { index };
			return true;
		}
	}
}
