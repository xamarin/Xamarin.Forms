﻿using System;
using Windows.UI.Xaml;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.UWP
{
	internal class FormsPresenter : Windows.UI.Xaml.Controls.ContentPresenter
	{
		public FormsPresenter()
		{
			Loaded += FormsPresenter_Loaded;
			Unloaded += FormsPresenter_Unloaded;
			SizeChanged += (s, e) =>
			{
				if (ActualWidth > 0 && ActualHeight > 0 && DataContext != null)
				{
					var page = (Page)DataContext;
					((Page)page.RealParent).ContainerArea = new Rectangle(0, 0, ActualWidth, ActualHeight);
				}
			};
		}

		void FormsPresenter_Loaded(object sender, RoutedEventArgs e)
			=> (DataContext as Page)?.SendAppearing();

		void FormsPresenter_Unloaded(object sender, RoutedEventArgs e)
			=> (DataContext as Page)?.SendDisappearing();
	}
}
