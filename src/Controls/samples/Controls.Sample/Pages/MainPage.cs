using Maui.Controls.Sample.ViewModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Controls.Sample.Pages
{

	public class MainPage : ContentPage, IPage
	{
		const string longLoremIpsum =
			"Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
			"Quisque ut dolor metus. Duis vel iaculis mauris, sit amet finibus mi. " +
			"Etiam congue ornare risus, in facilisis libero tempor eget. " +
			"Phasellus mattis mollis libero ut semper. In sit amet sapien odio. " +
			"Sed interdum ullamcorper dui eu rutrum. Vestibulum non sagittis justo. " +
			"Cras rutrum scelerisque elit, et porta est lobortis ac. " +
			"Pellentesque eu ornare tortor. Sed bibendum a nisl at laoreet.";

		const string loremIpsum =
			"Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

		MainPageViewModel _viewModel;
		public MainPage() : this(App.Current.Services.GetService<MainPageViewModel>())
		{

		}
		public MainPage(MainPageViewModel viewModel)
		{
			BindingContext = _viewModel = viewModel;

			var verticalStack = new VerticalStackLayout() { Spacing = 5, BackgroundColor = Color.AntiqueWhite };
			var horizontalStack = new HorizontalStackLayout() { Spacing = 2, BackgroundColor = Color.CornflowerBlue };

			var label = new Label { Text = "This will disappear in ~5 seconds", BackgroundColor = Color.Fuchsia };
			label.Margin = new Thickness(15, 10, 20, 15);

			verticalStack.Add(label);
			verticalStack.Add(new Label { Text = loremIpsum });
			verticalStack.Add(new Label { Text = loremIpsum, MaxLines = 2 });
			verticalStack.Add(new Label { Text = loremIpsum, LineBreakMode = LineBreakMode.TailTruncation });
			verticalStack.Add(new Label { Text = loremIpsum, MaxLines = 2, LineBreakMode = LineBreakMode.TailTruncation });

			var button = new Button() { Text = _viewModel.Text, WidthRequest = 200 };
			var button2 = new Button()
			{
				TextColor = Color.Green,
				Text = "Hello I'm a button",
				BackgroundColor = Color.Purple,
				Margin = new Thickness(12)
			};

			horizontalStack.Add(button);
			horizontalStack.Add(button2);
			horizontalStack.Add(new Label { Text = "And these buttons are in a HorizontalStackLayout" });

			verticalStack.Add(horizontalStack);
			verticalStack.Add(new Slider());
			verticalStack.Add(new Switch());
			verticalStack.Add(new Switch() { OnColor = Color.Green });
			verticalStack.Add(new Switch() { ThumbColor = Color.Yellow });
			verticalStack.Add(new Switch() { OnColor = Color.Green, ThumbColor = Color.Yellow });

			Content = verticalStack;
		}

		public IView View { get => (IView)Content; set => Content = (View)value; }
	}
}
