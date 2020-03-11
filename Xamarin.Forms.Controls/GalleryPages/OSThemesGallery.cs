using System;
namespace Xamarin.Forms.Controls
{
	public class OSThemesGallery : ContentPage
	{
		public OSThemesGallery()
		{
			var currentThemeLabel = new Label
			{
				Text = Application.Current.RequestedTheme.ToString()
			};

			var onThemeLabel = new Label
			{
				Text = "This text is green or red depending on Light (or default) or Dark",
				TextColor = new OnAppTheme<Color> { Light = Color.Green, Dark = Color.Red }
			};

			var stackLayout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Children = { currentThemeLabel, onThemeLabel }
			};

			Content = stackLayout;
		}
	}
}