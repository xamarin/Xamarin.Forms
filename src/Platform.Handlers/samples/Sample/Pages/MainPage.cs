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
	public class MainPage : Xamarin.Forms.ContentPage, IStartup
	{
		public MainPage(MainPageViewModel viewModel)
		{
			BindingContext = viewModel;
		}

		public IView GetContentView()
		{
			var verticalStack = new VerticalStackLayout()
			{
				Spacing = 5,
				BackgroundColor = Xamarin.Forms.Color.AntiqueWhite
			};

			var frame = new HorizontalStackLayout
			{
				BackgroundColor = Xamarin.Forms.Color.FromHex("#2196F3"),
			};

			frame.Add(new Label
			{
				Text = "Welcome to MAUI!",
				FontSize = 12,
			});

			verticalStack.Add(frame);

			verticalStack.Add(new Label
			{
				Text = "Start developing now",
				FontSize = 12,
				//Padding = new Thickness(30, 10, 30, 10),
			});

			verticalStack.Add(new Label
			{
				Text = "Make changes to your XAML file and save to see your UI update in the running app with XAML Hot Reload. Give it a try!",
				FontSize = 16,
				//Padding = new Thickness(30, 0, 30, 0),
			});

			var horizontalStack = new HorizontalStackLayout();

			horizontalStack.Add( new Label
			{
				FontSize = 16,
				Text = "Learn more at  "
				//Padding = new Thickness(30, 24, 30, 24),
			});
			horizontalStack.Add(new Label
			{
				FontSize = 16,
				Text = "https://aka.ms/xamarin-maui  ",
				FontAttributes = Xamarin.Forms.FontAttributes.Bold
			});

			verticalStack.Add(horizontalStack);

			var button = new Xamarin.Forms.Button
			{
				//FontSize = 16,
				//Padding = new Xamarin.Forms.Thickness(10)
			};
			button.BindingContext = BindingContext;
			button.SetBinding(Xamarin.Forms.Button.TextProperty, new Binding("Text"));

			verticalStack.Add(button);

			return verticalStack;
		}
	}
}
