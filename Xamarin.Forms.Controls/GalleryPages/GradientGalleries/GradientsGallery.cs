namespace Xamarin.Forms.Controls.GalleryPages.GradientGalleries
{
	public class GradientsGallery : ContentPage
	{
		public GradientsGallery()
		{
			var descriptionLabel =
				new Label { Text = "Gradients Galleries", Margin = new Thickness(2, 2, 2, 2) };

			Title = "Gradients Galleries";

			var navigationBarButton = new Button
			{
				FontSize = 10,
				HeightRequest = Device.RuntimePlatform == Device.Android ? 40 : 30,
				Text = "Gradient NavigationPage Gallery"
			};

			navigationBarButton.Clicked += (sender, args) =>
			{
				Navigation.PushAsync(new GradientNavigationPageGallery());
			};

			var tabsButton = new Button
			{
				FontSize = 10,
				HeightRequest = Device.RuntimePlatform == Device.Android ? 40 : 30,
				Text = "Gradient Tabs Gallery"
			};

			tabsButton.Clicked += (sender, args) =>
			{
				Navigation.PushAsync(new GradientTabsGallery());
			};

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryBuilder.NavButton("Gradient Views", () =>
							new GradientViewsGallery(), Navigation),
						GalleryBuilder.NavButton("LinearGradientBrush Explorer", () =>
							new LinearGradientExplorerGallery(), Navigation),
						GalleryBuilder.NavButton("RadialGradient Explorer", () =>
							new RadialGradientExplorerGallery(), Navigation),
						navigationBarButton,
						tabsButton,
						GalleryBuilder.NavButton("CSS Gradients Explorer", () =>
							new CssGradientsGallery(), Navigation)
					}
				}
			};
		}
	}
}