﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Xamarin.Forms.Platform.UWP
{
	partial class ShellPageWrapper
	{
		public ShellPageWrapper()
		{
			InitializeComponent();
		}

		public Page Page { get; set; }
		protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			LoadPage();
		}

		protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
			Root.Content = null;
		}

		public void LoadPage()
		{
			if (Page != null)
			{
				var container = Page.GetOrCreateRenderer().ContainerElement;
				Root.Content = container;
				container.Loaded -= OnPageLoaded;
				container.Loaded += OnPageLoaded;
			}
		}

		private void OnPageLoaded(object sender, RoutedEventArgs e)
		{
			var frameworkElement = sender as FrameworkElement;
			Page.Layout(new Rectangle(0, 0, frameworkElement.ActualWidth, frameworkElement.ActualHeight));
		}
	}
}
