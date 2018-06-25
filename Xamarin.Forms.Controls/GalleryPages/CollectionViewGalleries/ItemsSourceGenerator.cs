using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	public class TesItem
	{
		public string First { get; set; }
		public string Second { get; set; }
	}

	internal class ItemsSourceGenerator : ContentView
	{
		readonly CollectionView _cv;
		readonly Entry _entry;

		public ItemsSourceGenerator(CollectionView cv, int initialItems = 1000)
		{
			_cv = cv;

			var layout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Fill
			};

			var button = new Button { Text = "Update" };
			var label = new Label { Text = "Item count:", VerticalTextAlignment = TextAlignment.Center };
			_entry = new Entry { Keyboard = Keyboard.Numeric, Text = initialItems.ToString(), WidthRequest = 200 };

			layout.Children.Add(label);
			layout.Children.Add(_entry);
			layout.Children.Add(button);

			button.Clicked += GenerateItems;

			Content = layout;
		}



		public void GenerateItems()
		{
			if (int.TryParse(_entry.Text, out int count))
			{
				var items = new List<TesItem>();

				for (int n = 0; n < count; n++)
				{
					//items.Add($"{DateTime.Now.AddDays(n).ToLongDateString()}");

					items.Add(new TesItem(){First = "Herp", Second = "derp"});
				}

				_cv.ItemsSource = items;
			}
		}

		void GenerateItems(object sender, EventArgs e)
		{
			GenerateItems();
		}
	}
}