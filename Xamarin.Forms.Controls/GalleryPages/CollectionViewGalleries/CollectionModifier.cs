using System;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal abstract class CollectionModifier : ContentView
	{
		protected readonly CollectionView _cv;
		protected readonly Entry Entry;

		protected CollectionModifier(CollectionView cv, string buttonText)
		{
			_cv = cv;

			var layout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Fill
			};

			var button = new Button { Text = buttonText };
			var label = new Label { Text = "Index:", VerticalTextAlignment = TextAlignment.Center };

			Entry = new Entry { Keyboard = Keyboard.Numeric, Text = "0", WidthRequest = 200 };

			layout.Children.Add(label);
			layout.Children.Add(Entry);
			layout.Children.Add(button);

			button.Clicked += ButtonOnClicked;

			Content = layout;
		}

		private void ButtonOnClicked(object sender, EventArgs e)
		{
			OnButtonClicked();
		}

		protected virtual void OnButtonClicked()
		{
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