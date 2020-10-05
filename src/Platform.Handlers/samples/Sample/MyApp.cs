using System;
using Xamarin.Forms;
using Xamarin.Platform;
using Xamarin.Platform.Core;

namespace Sample
{
	public class MyApp : IApp
	{
		public MyApp()
		{
			Platform.Init();
		}

		public IView CreateView()
		{
			return CreateModernVersion();
		}

		private IView CreateModernVersion()
		{
			var verticalStack = new Xamarin.Platform.VerticalStackLayout() { Spacing = 5 };
			var horizontalStack = new Xamarin.Platform.HorizontalStackLayout() { Spacing = 2 };

			var label = new Label { Text = "This top part is a Xamarin.Platform.VerticalStackLayout" };

			verticalStack.Add(label);

			var button = new Button() { Text = "A Button" };
			var button2 = new Button()
			{
				Color = Color.Green,
				Text = "Hello I'm a button",
				BackgroundColor = Color.Purple
			};

			horizontalStack.Add(button);
			horizontalStack.Add(button2);
			horizontalStack.Add(new Label { Text = "And these buttons are in a HorizontalStackLayout" });

			verticalStack.Add(horizontalStack);

			verticalStack.Add(CreateFormsVersion());

			return verticalStack;
		}

		IView CreateFormsVersion()
		{
			var layout = new Xamarin.Forms.StackLayout();

			var label = new Label { Text = "This part of is a Forms StackLayout" };

			layout.Children.Add(label);

			var fl = new FlexLayout();

			var button = new Button() { Text = "A Button" };
			var button2 = new Button()
			{
				Color = Color.Green,
				Text = "Hello I'm a button",
				BackgroundColor = Color.Purple
			};

			fl.Children.Add(button);
			fl.Children.Add(button2);

			layout.Children.Add(fl);

			return layout;
		}
	}
}