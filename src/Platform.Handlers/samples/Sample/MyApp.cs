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
			var button2 = new Button() { 
				Color = Color.Green, 
				Text = "Hello I'm a button", BackgroundColor = Color.Purple };

			fl.Children.Add(button);
			fl.Children.Add(button2);

			layout.Children.Add(fl);

			return layout;
		}
	}
}