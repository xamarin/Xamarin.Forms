using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.TwoPaneViewGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OpenCompactWindow : ContentPage
	{
		public OpenCompactWindow()
		{
			InitializeComponent();
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			ContentPage page = new ContentPage() { BackgroundColor = Color.Purple };
			var button = new Button()
			{
				Text = "Close this Window"
			};

			var layout = new StackLayout()
			{
				Children =
				{
					button
				},
				BackgroundColor = Color.Blue,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill
			};


			layout.BatchCommitted += Layout_BatchCommitted;
			page.Content = layout;

			var args = await DualScreen.DualScreenHelper.OpenCompactMode(page);

			button.Command = new Command(async () =>
			{
				await args.Close();
			});
		}

		private void Layout_BatchCommitted(object sender, Internals.EventArg<VisualElement> e)
		{

		}
	}
}