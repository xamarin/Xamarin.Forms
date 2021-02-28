using System;
using System.Windows.Input;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Controls;

namespace Compatibility.Sample.ViewModels
{
	public class AboutViewModel : BaseViewModel
	{
		public AboutViewModel()
		{
			Title = "About";
			OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
		}

		public ICommand OpenWebCommand { get; }
	}
}