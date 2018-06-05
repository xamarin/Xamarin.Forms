using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	public class CollectionViewGallery : ContentPage
	{
		public CollectionViewGallery()
		{
			Content = new StackLayout
			{
				Children =
				{
					GalleryBuilder.NavButton("Default Text Galleries", () => new DefaultTextGallery(), Navigation),
				}
			};
		}
	}

	public class DefaultTextGallery : ContentPage
	{
		public DefaultTextGallery()
		{
			var desc = "No DataTemplates; just using the ToString() of the objects in the source.";

			var descriptionLabel = new Label { Text = desc, Margin = new Thickness(2,2,2,2)};

			Title = "Default Text Galleries";

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						// TODO hartez 2018-06-05 10:43 AM Need a gallery page which allows layout selection so we can demonstrate switching between them
						descriptionLabel,
						GalleryBuilder.NavButton("Vertical List (Code)", () => new TextCodeCollectionViewGallery(ListItemsLayout.VerticalList), Navigation),
						GalleryBuilder.NavButton("Horizontal List (Code)", () => new TextCodeCollectionViewGallery(ListItemsLayout.HorizontalList), Navigation),
						GalleryBuilder.NavButton("Vertical Grid (Code)", () => new TextCodeCollectionViewGridGallery(), Navigation),
					}
				}
			};
		}
	}

	internal class ItemsSourceGenerator : ContentView
	{
		readonly CollectionView _cv;
		readonly Entry _entry;

		public ItemsSourceGenerator(CollectionView cv)
		{
			_cv = cv;

			var layout = new StackLayout { Orientation = StackOrientation.Horizontal };

			var button = new Button { Text = "Update" };
			var label = new Label { Text = "Item count:", VerticalTextAlignment = TextAlignment.Center };
			_entry = new Entry { Keyboard = Keyboard.Numeric, Text = "1000" };

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
				var items = new List<string>();

				for (int n = 0; n < count; n++)
				{
					items.Add($"{DateTime.Now.AddDays(n).ToLongDateString()}");
				}

				_cv.ItemsSource = items;
			}
		}

		void GenerateItems(object sender, EventArgs e)
		{
			GenerateItems();
		}
	}

	internal class TextCodeCollectionViewGallery : ContentPage
	{
		public TextCodeCollectionViewGallery(IItemsLayout itemsLayout)
		{
			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star }
				}
			};

			var collectionView = new CollectionView {ItemsLayout = itemsLayout};

			var generator = new ItemsSourceGenerator(collectionView);

			layout.Children.Add(generator);
			layout.Children.Add(collectionView);
			Grid.SetRow(collectionView, 1);

			Content = layout;

			generator.GenerateItems();
		}
	}

	internal class TextCodeCollectionViewGridGallery : ContentPage
	{
		public TextCodeCollectionViewGridGallery()
		{
			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					//new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star }
				}
			};

			var itemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical);

			var collectionView = new CollectionView {ItemsLayout = itemsLayout};

			var generator = new ItemsSourceGenerator(collectionView);

			layout.Children.Add(generator);
			layout.Children.Add(collectionView);
			Grid.SetRow(collectionView, 1);

			Content = layout;

			generator.GenerateItems();
		}
	}

}
