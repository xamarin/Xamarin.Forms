using System;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class ItemAdder : ContentView
	{
		readonly CollectionView _cv;
		readonly Entry _entry;

		public ItemAdder(CollectionView cv)
		{
			_cv = cv;

			var layout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Fill
			};

			var button = new Button { Text = "Insert" };
			var label = new Label { Text = "Index:", VerticalTextAlignment = TextAlignment.Center };

			_entry = new Entry { Keyboard = Keyboard.Numeric, Text = "0", WidthRequest = 200 };

			layout.Children.Add(label);
			layout.Children.Add(_entry);
			layout.Children.Add(button);

			button.Clicked += InsertItem;

			Content = layout;
		}

		void InsertItem(object sender, EventArgs e)
		{
			if (!int.TryParse(_entry.Text, out int index))
			{
				return;
			}

			if (!(_cv.ItemsSource is ObservableCollection<TestItem> observableCollection))
			{
				return;
			}

			if (index > -1 && index < observableCollection.Count)
			{
				var item = new TestItem(){ Image = "oasis.jpg", Date = $"{DateTime.Now.ToLongDateString()}", Caption = "Inserted"};
				observableCollection.Insert(index, item);
			}
		}
	}
}