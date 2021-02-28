using Compatibility.Sample.Models;
using Compatibility.Sample.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Compatibility.Sample.Views
{
	public partial class NewItemPage : ContentPage
	{
		public Item Item { get; set; }

		public NewItemPage()
		{
			InitializeComponent();
			BindingContext = new NewItemViewModel();
		}
	}
}