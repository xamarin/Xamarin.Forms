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
		public CarouselViewModel(int intialItems = 5)
		{
			var items = new List<CarouselItem>();
			for (int i = 0; i < intialItems; i++)
			{
				items.Add(new CarouselItem());
			}

			Items = new ObservableCollection<CarouselItem>(items);
			//Items.CollectionChanged += ItemsCollectionChanged;
			Count = Items.Count;
		}

		void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
		}


		int _count;
		ObservableCollection<CarouselItem> _items;

		public int Count
		{
			get { return _count; }
			set { SetProperty(ref _count, value); }
		}

		public ObservableCollection<CarouselItem> Items
		{
			get { return _items; }
			set { SetProperty(ref _items, value); }
		}

	}

	internal class CarouselItem
	{

	}
}
