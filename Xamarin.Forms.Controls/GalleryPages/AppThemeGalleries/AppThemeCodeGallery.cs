namespace Xamarin.Forms.Controls.GalleryPages.AppThemeGalleries
{
	public class AppThemeCodeGallery : ContentPage
	{
		public AppThemeCodeGallery()
		{
			var currentThemeLabel = new Label
			{
				Text = Application.Current.RequestedTheme.ToString()
			};

			Application.Current.RequestedThemeChanged += (s, a) =>
			{
				currentThemeLabel.Text = Application.Current.RequestedTheme.ToString();
			};

			var onThemeLabel = new Label
			{
				Text = "TextColor through SetBinding"
			};

			var onThemeLabel1 = new Label
			{
				Text = "TextColor through SetAppTheme"
			};

			var onThemeLabel2 = new Label
			{
				Text = "TextColor through SetAppThemeColor"
			};

			onThemeLabel.SetBinding(Label.TextColorProperty, new AppThemeColor() { Light = Color.Green, Dark = Color.Red } );

			onThemeLabel1.SetAppTheme(Label.TextColorProperty, Color.Green, Color.Red);

			onThemeLabel2.SetAppThemeColor(Label.TextColorProperty, Color.Green, Color.Red);

			var stackLayout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Children = { currentThemeLabel, onThemeLabel , onThemeLabel1, onThemeLabel2 }
			};

			Content = stackLayout;
		}
	}
}