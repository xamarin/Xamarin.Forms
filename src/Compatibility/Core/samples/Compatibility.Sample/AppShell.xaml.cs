using Compatibility.Sample.ViewModels;
using Compatibility.Sample.Views;
using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Compatibility.Sample
{
	public partial class AppShell : Microsoft.Maui.Controls.Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
			Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
		}

		private async void OnMenuItemClicked(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}
	}
}
