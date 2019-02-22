using System;

namespace Xamarin.Forms.Controls
{
	public class MaterialActivityIndicatorGallery : ContentPage
	{
		public MaterialActivityIndicatorGallery()
		{
			Visual = VisualMarker.Material;

			var activityIndicator = new ActivityIndicator()
			{
				IsRunning = false,
				HeightRequest = 50
			};

			var isRunSwitch = new Switch { IsToggled = activityIndicator.IsRunning };
			isRunSwitch.Toggled += (_, e) => activityIndicator.IsRunning = e.Value;

			var primaryPicker = new ColorPicker { Title = "Primary Color", Color = activityIndicator.Color };
			primaryPicker.ColorPicked += (_, e) =>
			{
				activityIndicator.Color = e.Color;
			};
			var backgroundPicker = new ColorPicker { Title = "Background Color", Color = activityIndicator.BackgroundColor };
			backgroundPicker.ColorPicked += (_, e) => activityIndicator.BackgroundColor = e.Color;
			var heightPicker = MaterialProgressBarGallery.CreateValuePicker("Height", value => activityIndicator.HeightRequest = value);

			Content = new StackLayout
			{
				Padding = 10,
				Spacing = 10,
				Children =
				{
					new ScrollView
					{
						Margin = new Thickness(-10, 0),
						Content = new StackLayout
						{
							Padding = 10,
							Spacing = 10,
							Children =
							{
								isRunSwitch,
								primaryPicker,
								backgroundPicker,
								heightPicker,
							}
						}
					},

					new BoxView
					{
						HeightRequest = 1,
						Margin = new Thickness(-10, 0),
						Color = Color.Black
					},

					new StackLayout
					{
						Children =
						{
							activityIndicator
						},
						BackgroundColor = Color.Blue
					}
				}
			};
		}
	}
}
