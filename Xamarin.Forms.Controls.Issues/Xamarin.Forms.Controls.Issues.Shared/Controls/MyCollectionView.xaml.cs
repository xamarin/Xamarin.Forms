using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls.Issues
{
	public partial class MyCollectionView : CollectionView
	{
		public ObservableCollection<string> Data { get; private set; }

		public MyCollectionView(ObservableCollection<string> data)
		{
#if APP
			InitializeComponent();
#endif
			BindingContext = this;

			// Moving this above InitializeComponent fixes the problem.
			Data = data;
		}

		void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
			{
				return;
			}

			SelectedItem = null;

			if (e.CurrentSelection.FirstOrDefault() is String data)
			{
				Console.WriteLine($"Selected: {data}");
			}
		}

		void DeleteItem_Invoked(System.Object sender, System.EventArgs e)
		{
			if (sender is SwipeItem swipeItem && swipeItem.BindingContext is String dataItem)
			{
				Data.Remove(dataItem);
			}
		}
	}
}