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

			var button = new Button() { Text = "A Button" };
			var button2 = new Button() { Text = "A Second Button" };

			layout.Children.Add(button);
			layout.Children.Add(button2);
			
			return layout;
		}

	
	}
}