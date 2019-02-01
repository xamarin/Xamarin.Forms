using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.CarouselViewGalleries
{
	public partial class CarouselXamlGallery : ContentPage
	{
		public CarouselXamlGallery()
		{
			InitializeComponent();
			BindingContext = new CarouselViewModel();
		}
	}

	internal class CarouselViewModel : ViewModelBase2
	{
		int _count;
		int _position;
		ObservableCollection<CarouselItem> _items;

		public CarouselViewModel(int intialItems = 5)
		{
			var items = new List<CarouselItem>();
			for (int i = 0; i < intialItems; i++)
			{
				items.Add(new CarouselItem("cardBackground"));
			}

			MessagingCenter.Subscribe<ExampleTemplateCarousel>(this, "remove", (obj) => Items.Remove(obj.BindingContext as CarouselItem));

			Items = new ObservableCollection<CarouselItem>(items);
			Items.CollectionChanged += ItemsCollectionChanged;
			Count = Items.Count - 1;
		}

		void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Count = Items.Count - 1;
		}

		public int Count
		{
			get { return _count; }
			set { SetProperty(ref _count, value); }
		}

		public int Position
		{
			get { return _position; }
			set { SetProperty(ref _position, value); }
		}

		public ObservableCollection<CarouselItem> Items
		{
			get { return _items; }
			set { SetProperty(ref _items, value); }
		}
	}

	internal class CarouselItem
	{
		public CarouselItem(string image)
		{
			Image = image;
		}

		public string Image { get; set; }
	}
}
