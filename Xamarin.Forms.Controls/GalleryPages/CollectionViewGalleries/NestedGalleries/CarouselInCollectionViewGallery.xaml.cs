using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.NestedGalleries
{
	public partial class CarouselInCollectionViewGallery : ContentPage
	{
		public CarouselInCollectionViewGallery()
		{
			InitializeComponent();
			BindingContext = new ViewModel();
		}

		public class ViewModel
		{
			public List<Item> Items { get; }

			public ViewModel()
			{
				Items = Enumerable.Range(0, 30).Select(x => new Item()).ToList();
			}
		}

		public class Item
		{
			public IList<string> Images { get; set; } = new[]
			{
				"cover1.jpg",
				"oasis.jpg",
				"photo.jpg",
				"Vegetables.jpg",
				"Fruits.jpg",
				"FlowerBuds.jpg",
				"Legumes.jpg"
			};
		}
	}
}
