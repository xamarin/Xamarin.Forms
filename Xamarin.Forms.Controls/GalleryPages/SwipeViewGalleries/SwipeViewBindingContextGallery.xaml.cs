﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.SwipeViewGalleries
{
	[Preserve(AllMembers = true)]
	public partial class SwipeViewBindingContextGallery : ContentPage
	{
		public SwipeViewBindingContextGallery()
		{
			InitializeComponent();
		}
	}

	[Preserve(AllMembers = true)]
	public class SwipeViewBindingContextGalleryModel : BindableObject
	{
		public string Title { get; set; }
		public string SubTitle { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class SwipeViewBindingContextGalleryViewModel : BindableObject
	{
		ObservableCollection<SwipeViewBindingContextGalleryModel> _items;
		SwipeViewBindingContextGalleryModel _tappedItem;

		public SwipeViewBindingContextGalleryViewModel()
		{
			Items = new ObservableCollection<SwipeViewBindingContextGalleryModel>();
			LoadItems();
		}

		public ObservableCollection<SwipeViewBindingContextGalleryModel> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		public SwipeViewBindingContextGalleryModel TappedItem
		{
			get { return _tappedItem; }
			set
			{
				_tappedItem = value;
				OnPropertyChanged();
			}
		}

		public ICommand SwipeItemTapCommand => new Command<object>(SwipeItemTap);

		void LoadItems()
		{
			for (int i = 0; i < 100; i++)
			{
				Items.Add(new SwipeViewBindingContextGalleryModel
				{
					Title = $"Lorem ipsum {i + 1}",
					SubTitle = "Lorem ipsum dolor sit amet"
				});
			}
		}

		void SwipeItemTap(object parameter)
		{
			TappedItem = parameter as SwipeViewBindingContextGalleryModel;
		}
	}
}