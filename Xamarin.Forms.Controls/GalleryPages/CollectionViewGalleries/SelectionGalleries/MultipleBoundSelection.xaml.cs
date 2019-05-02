using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.SelectionGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MultipleBoundSelection : ContentPage
	{
		BoundSelectionModel _vm;

		public MultipleBoundSelection()
		{
			InitializeComponent();
			_vm = new BoundSelectionModel();
			BindingContext = _vm;
		}

		private void ResetClicked(object sender, EventArgs e)
		{
			_vm.SelectedItems.Clear();
			_vm.SelectedItems.Add(_vm.Items[1]);
			_vm.SelectedItems.Add(_vm.Items[2]);

			//_vm.SelectedItems = new ObservableCollection<CollectionViewGalleryTestItem>
			//{
			//	_vm.Items[1],
			//	_vm.Items[2]
			//};
		}
	}
}