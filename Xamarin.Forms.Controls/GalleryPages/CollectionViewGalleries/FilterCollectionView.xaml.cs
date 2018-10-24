using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FilterCollectionView : ContentPage
	{
		readonly List<CollectionViewGalleryTestItem> _source;
		readonly ObservableCollection<CollectionViewGalleryTestItem> _items;

		public FilterCollectionView ()
		{
			InitializeComponent ();

			_source = new List<CollectionViewGalleryTestItem>();
			var count = 0;
			string[] _images = 
			{
				"cover1.jpg", 
				"oasis.jpg",
				"photo.jpg",
				"Vegetables.jpg",
				"Fruits.jpg",
				"FlowerBuds.jpg",
				"Legumes.jpg"
			};

			for (int n = 0; n < count; n++)
			{
				_source.Add(new CollectionViewGalleryTestItem(DateTime.Now.AddDays(n),
					$"{_images[n % _images.Length]}, {n}", _images[n % _images.Length], n));
			}

			CollectionView.ItemTemplate = ExampleTemplates.PhotoTemplate();
			_items = new ObservableCollection<CollectionViewGalleryTestItem>(_source);
			CollectionView.ItemsSource = _items;

			SearchBar.TextChanged += SearchBarOnTextChanged;
		}

		void SearchBarOnTextChanged(object sender, TextChangedEventArgs e)
		{
			FilterItems(e.NewTextValue);
		}

		void FilterItems(string filter)
		{
			var filteredItems = _source.Where(item => item.Caption.ToLower().Contains(filter.ToLower())).ToList();

			foreach (CollectionViewGalleryTestItem collectionViewGalleryTestItem in _source)
			{
				if (!filteredItems.Contains(collectionViewGalleryTestItem))
				{
					_items.Remove(collectionViewGalleryTestItem);
				}
				else
				{
					if (!_items.Contains(collectionViewGalleryTestItem))
					{
						_items.Add(collectionViewGalleryTestItem);
					}
				}
			}
		}
	}
}