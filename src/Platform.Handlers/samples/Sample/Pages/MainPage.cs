using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Services;
using Sample.ViewModel;
using Xamarin.Forms;
using Xamarin.Platform;
using Xamarin.Platform.Hosting;

namespace Sample.Pages
{
	public class MainPage : Xamarin.Forms.ContentPage, IPage
	{
		public MainPage(MainPageViewModel viewModel)
		{
			BindingContext = viewModel;
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

			var button = new Button() { Text = "A Button", Width = 200 };

			var xfButton = new Xamarin.Forms.Button
			{
				FontSize = 16,
				Padding = new Xamarin.Forms.Thickness(10)
			};
			xfButton.BindingContext = BindingContext;
			xfButton.SetBinding(Xamarin.Forms.Button.TextProperty, new Binding("Text"));

			var button2 = new Button()
			{
				Color = Color.Green,
				Text = "Hello I'm a button",
				BackgroundColor = Color.Purple,
				Margin = new Thickness(12)
			};

			horizontalStack.Add(xfButton);
			horizontalStack.Add(button);
			horizontalStack.Add(button2);
			horizontalStack.Add(new Label { Text = "And these buttons are in a HorizontalStackLayout" });

			verticalStack.Add(horizontalStack);
			verticalStack.Add(new Slider());

			return verticalStack;
		}
	}
}
