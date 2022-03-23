using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.CarouselViewGalleries
{
	public partial class ReplaceSourceGallery : ContentPage
	{
		public ReplaceSourceGallery()
        {
            InitializeComponent();
            BindingContext = new ViewModel();
        }

        public void BindingContextButton_Clicked(object sender, EventArgs eventArgs)
        {
            BindingContext = new ViewModel();
        }

        public void ItemsSourceButton_Clicked(object sender, EventArgs eventArgs)
        {
            ((ViewModel)BindingContext).Images = new[]
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

        public class ViewModel
        {
            public IList<string> Images { get; set; } = new []
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
