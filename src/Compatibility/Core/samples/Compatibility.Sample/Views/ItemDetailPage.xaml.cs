using Compatibility.Sample.ViewModels;
using System.ComponentModel;
using Microsoft.Maui.Controls;

namespace Compatibility.Sample.Views
{
	public partial class ItemDetailPage : ContentPage
	{
		public ItemDetailPage()
		{
			InitializeComponent();
			BindingContext = new ItemDetailViewModel();
		}
	}
}