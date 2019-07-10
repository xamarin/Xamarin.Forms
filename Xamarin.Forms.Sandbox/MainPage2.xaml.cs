using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Forms.Sandbox
{
	public partial class MainPage2 : ContentPage
	{
		public MainPage2()
		{
			InitializeComponent();

			_collView.ItemsSource = new ObservableCollection<string>();
				//Enumerable.Range(0, 2).Select(i => i.ToString()));
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			var item = (string)((View)sender).BindingContext;
			(_collView.ItemsSource as ObservableCollection<string>).Remove(item);
		}

		private void Button_Clicked_1(object sender, EventArgs e)
		{
			_collView.ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem;
			//(_collView.ItemsSource as ObservableCollection<string>).Add("AAAA");
		}
	}
}