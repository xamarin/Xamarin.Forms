using System.Collections.Generic;

namespace Xamarin.Forms.Controls.GalleryPages
{
	public class GifGallery : ContentPage
	{
		public GifGallery()
		{
			var scroll = new ScrollView();

			var layout = new StackLayout
			{
				Padding = new Thickness(12)
			};

			var sourceLabel = new Label
			{
				FontSize = 10,
				Text = "Source:"
			};

			var itemsSource = new List<string>
			{
				Device.RuntimePlatform == Device.UWP ? "Assets/GifOne.gif" : "GifOne.gif",
				Device.RuntimePlatform == Device.UWP ? "Assets/GifTwo.gif" : "GifTwo.gif",
				"https://devblogs.microsoft.com/wp-content/uploads/sites/44/2019/03/imagebutton-1.gif",
				"https://upload.wikimedia.org/wikipedia/commons/1/13/Rotating_earth_%28huge%29.gif" // (Huge gif file)
			};

			var sourcePicker = new Picker
			{
				ItemsSource = itemsSource,
				SelectedItem = itemsSource[0]
			};

			var isAnimationAutoPlayLabel = new Label
			{
				FontSize = 10,
				Text = "IsAnimationAutoPlay:"
			};

			var isAnimationAutoPlaySwitch = new Switch
			{
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};

			var IsAnimationPlayingLabel = new Label
			{
				FontSize = 10,
				Text = "IsAnimationPlaying:",
				VerticalOptions = LayoutOptions.Center
			};

			var isAnimationPlayingSwitch = new Switch
			{
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};

			var gifImage = new Image
			{
				IsAnimationAutoPlay = false,
				BackgroundColor = Color.LightGray,
				Source = itemsSource[0]
			};

			isAnimationPlayingSwitch.SetBinding(Switch.IsToggledProperty, nameof(gifImage.IsAnimationPlaying));

			var buttonStack = new StackLayout
			{
				Orientation = StackOrientation.Horizontal
			};

			var playButton = new Button
			{
				Text = "Play"
			};

			playButton.Clicked += (sender, e) =>
			{
				gifImage.IsAnimationPlaying = true;
			};

			var stopButton = new Button
			{
				Text = "Stop"
			};

			stopButton.Clicked += (sender, e) =>
			{
				gifImage.IsAnimationPlaying = false;
			};

			buttonStack.Children.Add(playButton);
			buttonStack.Children.Add(stopButton);

			layout.Children.Add(sourceLabel);
			layout.Children.Add(sourcePicker);
			layout.Children.Add(isAnimationAutoPlayLabel);
			layout.Children.Add(isAnimationAutoPlaySwitch);
			layout.Children.Add(IsAnimationPlayingLabel);
			layout.Children.Add(isAnimationPlayingSwitch);
			layout.Children.Add(gifImage);
			layout.Children.Add(buttonStack);

			sourcePicker.SelectedIndexChanged += (sender, e) =>
			{
				gifImage.Source = itemsSource[sourcePicker.SelectedIndex];
			};

			isAnimationAutoPlaySwitch.Toggled += (sender, e) =>
			{
				gifImage.IsAnimationAutoPlay = isAnimationAutoPlaySwitch.IsToggled;
			};

			scroll.Content = layout;

			Content = scroll;
		}
	}
}