using Maui.Controls.Sample.ViewModel;
using Xamarin.Forms;
using Xamarin.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Controls.Sample.Pages
{

	public class MainPage : ContentPage, IPage
	{
		MainPageViewModel _viewModel;
		public MainPage() : this(App.Current.Services.GetService<MainPageViewModel>())
		{

		}
		public MainPage(MainPageViewModel viewModel)
		{
			BindingContext = _viewModel = viewModel;

			var verticalStack = new VerticalStackLayout() { Spacing = 5, BackgroundColor = Color.AntiqueWhite };
			var horizontalStack = new HorizontalStackLayout() { Spacing = 2, BackgroundColor = Color.CornflowerBlue };

			var button = new Button() { Text = _viewModel.Text };
			var button2 = new Button()
			{
				TextColor = Color.Green,
				Text = "Hello I'm a button",
				BackgroundColor = Color.Purple,
				Margin = new Thickness(12)
			};

			horizontalStack.Add(button);
			horizontalStack.Add(button2);

		//	horizontalStack.Add(new Label { Text = "And these buttons are in a HorizontalStackLayout" });

			verticalStack.Add(horizontalStack);
			verticalStack.Add(new Slider());

			View = verticalStack;
		}

		public IView View { get; set; }
	}
}
