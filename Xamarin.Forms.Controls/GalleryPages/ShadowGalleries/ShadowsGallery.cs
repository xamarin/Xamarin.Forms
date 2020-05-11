namespace Xamarin.Forms.Controls.GalleryPages.ShadowGalleries
{
	public class ShadowsGallery : ContentPage
	{
		public ShadowsGallery()
		{
			Title = "Shadows Galleries";


			var button = new Button
			{
				Text = "Enable Shadows",
				AutomationId = "EnableShadows"
			};

			button.Clicked += ButtonClicked;

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						button,
						GalleryBuilder.NavButton("Shadows Explorer", () =>
							new ShadowsExplorerGallery(), Navigation),
						GalleryBuilder.NavButton("Shadow Layouts Explorer", () =>
							new ShadowLayoutGallery(), Navigation)
					}
				}
			};
		}


		void ButtonClicked(object sender, System.EventArgs e)
		{
			var button = sender as Button;

			button.Text = "Shadows Enabled!";
			button.TextColor = Color.Black;
			button.IsEnabled = false;

			Device.SetFlags(new[] { ExperimentalFlags.ShadowExperimental });
		}
	}
}