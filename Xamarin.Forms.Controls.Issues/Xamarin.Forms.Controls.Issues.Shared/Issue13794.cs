﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13794, "[Bug] CollectionView iOS draws both EmptyView and Items/ItemTemplate", PlatformAffected.iOS)]
	public class Issue13794 : TestContentPage
	{
		ObservableCollection<string> _data;
		MyCollectionView _myCollectionView;

		public Issue13794()
		{
			var carouselView = new CarouselView();

            BindingContext = this;

            // This is gross, but the idea is the page contains the data and passes a refernece to the view.
            // Data is loaded from the page (eg via web request, loaded from file)
            _data = new ObservableCollection<string>();

            _myCollectionView = new MyCollectionView(_data);

            // In my code I was replacing the Footer already defined in the xaml.
            // This didn't seem to contribute to the issue.
            //_myCollectionView.Footer = new BoxView();

            // Add collection view to a CarouselView. This was the only way I could replicate the issue.
            // In my actual app I have 3 different user feeds, and user can change between them.
            carouselView.ItemsSource = new List<View>()
            {
                _myCollectionView,
            };

            // This makes more sense when there is 3 differnet things to select.
            carouselView.ItemTemplate = new MyDataTemplateSelector()
            {
                MyCollectionViewDataTemplate = new DataTemplate(() => _myCollectionView),
            };

            // I was trying to set _myCollectionView as root Content but it doesn't appear to
            // cause the problem. Its related to the CarouselView
            //Content = _myCollectionView;


            // Add data
            var rand = new Random();
            _data.Add(rand.Next().ToString());
            _data.Add(rand.Next().ToString());
            _data.Add(rand.Next().ToString());



            // Does not have the issue if the data is loaded with a delay.
            /*
            Task.Run(async () =>
            {
                await Task.Delay(1000);

                var rand = new Random();
                _data.Add(rand.Next().ToString());
                _data.Add(rand.Next().ToString());
                _data.Add(rand.Next().ToString());
            });
            */

            Content = carouselView;
		}

		protected override void Init()
		{
			
		}
	}

	public class MyDataTemplateSelector : DataTemplateSelector
	{
		public DataTemplate MyCollectionViewDataTemplate { get; set; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item is MyCollectionView)
			{
				return MyCollectionViewDataTemplate;
			}

			return null;
		}
	}
}

