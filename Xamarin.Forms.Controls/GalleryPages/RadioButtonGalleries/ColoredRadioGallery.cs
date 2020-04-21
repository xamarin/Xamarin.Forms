using System;
namespace Xamarin.Forms.Controls.GalleryPages.RadioButtonGalleries
{
	public class ColoredRadioGallery : ContentPage
	{
		readonly Random _rand = new Random();
		readonly RadioButton _radioButton = new RadioButton
		{
			Text = "I can change color",
			RadioColor = Color.Red
		};

		readonly RadioButton _radioButtonDisabled = new RadioButton
		{
			Text = "I'm disabled",
			IsEnabled = false
		};

		public ColoredRadioGallery()
		{
			var stackLayout = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};

			var button = new Button
			{
				Text = "Change color"
			};

			button.Clicked += Button_Clicked;

			stackLayout.Children.Add(button);
			stackLayout.Children.Add(_radioButton);
			stackLayout.Children.Add(_radioButtonDisabled);

			Content = stackLayout;
		}

		void Button_Clicked(object sender, EventArgs e)
		{
			var c = Color.FromRgb(_rand.Next(256), _rand.Next(256), _rand.Next(256));
			_radioButton.RadioColor = c;
			_radioButtonDisabled.RadioColor = c;
		}
	}
}