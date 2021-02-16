using Sample.ViewModel;
using Xamarin.Forms;
using Xamarin.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Pages
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
			View = GetContentView();
		}

		public IView View { get; set; }

		public IView GetContentView()
		{
			var verticalStack = new VerticalStackLayout() { Spacing = 5, BackgroundColor = Color.AntiqueWhite };
			var horizontalStack = new HorizontalStackLayout() { Spacing = 2, BackgroundColor = Color.CornflowerBlue };

			var label = new Label { Text = "This will disappear in ~5 seconds", BackgroundColor = Color.Fuchsia };
			label.Margin = new Thickness(15, 10, 20, 15);

			verticalStack.Add(label);

			var button = new Button() { Text = _viewModel.Text, Width = 200 };

			var button2 = new Button()
			{
				Color = Color.Green,
				Text = "Hello I'm a button",
				BackgroundColor = Color.Purple,
				Margin = new Thickness(12)
			};

			horizontalStack.Add(button);
			horizontalStack.Add(button2);
			horizontalStack.Add(new Label { Text = "And these buttons are in a HorizontalStackLayout" });

			verticalStack.Add(horizontalStack);
			verticalStack.Add(new Slider());

			return verticalStack;
		}
	}
}
