using Maui.Controls.Sample.ViewModel;
using Xamarin.Forms;
using Xamarin.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Controls.Sample.Pages
{

	public class MainPage : Xamarin.Forms.ContentPage, IPage
	{
		MainPageViewModel _viewModel;
		public MainPage() : this(App.Current.Services.GetService<MainPageViewModel>())
		{

		}
		public MainPage(MainPageViewModel viewModel)
		{
			BindingContext = _viewModel = viewModel;
			View = new Button { Text = _viewModel.Text };
		}

		public IView View { get; set; }
	}
}
