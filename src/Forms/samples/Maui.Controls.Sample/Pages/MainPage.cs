using Maui.Controls.Sample.ViewModel;
using Xamarin.Forms;
using Xamarin.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Controls.Sample.Pages
{
	public class MainPage : ContentPage
	{
		MainPageViewModel _viewModel;
		public MainPage() : this(App.Current.Services.GetService<MainPageViewModel>())
		{

		}

		public MainPage(MainPageViewModel viewModel)
		{
			Content = GetContentView();
			BindingContext = _viewModel = viewModel;
		}

		public IView GetContentView()
		{
			var verticalStack = new Controls.VerticalStackLayout() { Spacing = 5, BackgroundColor = Color.AntiqueWhite };

			var button = new Button() { Text = _viewModel.Text };

			verticalStack.Add(button);
			verticalStack.Add(new Slider());

			return verticalStack;
		}
	}
}
