using Sample.Controls;
using Sample.Pages;
using Xamarin.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace Sample
{
	public class MainWindow : Window
	{
		public MainWindow() : this(App.Current.Services.GetService<MainPage>())
		{
		}

		public MainWindow(MainPage page)
		{
			Page = page;
		}
	}
}
