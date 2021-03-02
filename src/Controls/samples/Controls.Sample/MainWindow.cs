using Maui.Controls.Sample.Pages;
using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Controls.Sample
{
	public class MainWindow : Window
	{
		public MainWindow() : this(Application.Current.Services.GetService<MainPage>())
		{
		}

		public MainWindow(MainPage page)
		{
			Content = page;
		}
	}
}