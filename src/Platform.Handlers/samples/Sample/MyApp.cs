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
			var layout = new Xamarin.Forms.StackLayout();

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

			var label = new Label { Text = "I am a label" };

			layout.Children.Add(label);


var b = new Xamarin.Forms.Button();
b.Navigation.PushAsync(new ContentPage());

			var alternateLayout = new AlternateStackLayout();
			var button3 = new Button() { Text = "In a 3rd party stack layout" };
			alternateLayout.Children.Add(button3);
			layout.Children.Add(alternateLayout);

			return layout;
		}
	}
}