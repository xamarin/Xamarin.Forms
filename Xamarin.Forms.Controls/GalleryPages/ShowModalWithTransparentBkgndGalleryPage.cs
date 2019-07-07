using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages
{
	class ShowModalWithTransparentBkgndGalleryPage : ContentPage
	{
		public ShowModalWithTransparentBkgndGalleryPage()
		{
			BackgroundColor = Color.LightPink;

			var btn = new Button()
			{
				Text = "Show page",
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Center
			};

			btn.Clicked += ShowModalBtnClicked;

			Content = btn;
		}

		void ShowModalBtnClicked(object sender, System.EventArgs e)
		{
			Navigation.PushModalAsync(new PageWithTransparentBkgnd());
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
		}
	}
}
