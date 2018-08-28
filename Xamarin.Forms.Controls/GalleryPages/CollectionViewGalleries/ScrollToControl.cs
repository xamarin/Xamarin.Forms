using System;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class ScrollToControl : ContentView
	{
		readonly Entry _entry;
		readonly CollectionView _cv;

		public ScrollToControl(CollectionView cv)
		{
			_cv = cv;
			var label = new Label { Text = "Scroll To Index: ", VerticalTextAlignment = TextAlignment.Center};
			_entry = new Entry { Keyboard = Keyboard.Numeric, Text = "0", WidthRequest = 200 };
			var button = new Button { Text = "Go" };

			button.Clicked += ScrollTo;

			var layout = new StackLayout() { Orientation = StackOrientation.Horizontal };
			layout.Children.Add(label);
			layout.Children.Add(_entry);
			layout.Children.Add(button);

			Content = layout;
		}

		void ScrollTo()
		{
			if (int.TryParse(_entry.Text, out int count))
			{
				_cv.ScrollTo(count);
			}
		}

		void ScrollTo(object sender, EventArgs e)
		{
			ScrollTo();
		}
	}
}