using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Controls
{
	internal class MediaElementDemoPage : ContentPage
	{
		MediaElement element;
		public MediaElementDemoPage()
		{
			element = new MediaElement();
			element.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill,true);
			element.VerticalOptions = new LayoutOptions(LayoutAlignment.Fill,true);
			element.AutoPlay = false;
			element.AreTransportControlsEnabled = false;

			var label = new Label();
			label.SetBinding(Label.TextProperty, new Binding("CurrentState", BindingMode.OneWay, null, null, null, element));

			var playButton = new Button();
			playButton.Text = "\u25b6\uFE0F";
			playButton.FontSize = 48;
			playButton.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
			playButton.Clicked += PlayButton_Clicked;

			var pauseButton = new Button();
			pauseButton.Text = "\u23f8\uFE0F";
			pauseButton.FontSize = 48;
			pauseButton.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
			pauseButton.Clicked += PauseButton_Clicked;

			var stopButton = new Button();
			stopButton.Text = "\u23f9\uFE0F";
			stopButton.FontSize = 48;
			stopButton.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
			stopButton.Clicked += StopButton_Clicked;

			var mediaControlStack = new StackLayout();
			mediaControlStack.Orientation = StackOrientation.Horizontal;
			mediaControlStack.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, false);
			mediaControlStack.Children.Add(playButton);
			mediaControlStack.Children.Add(pauseButton);
			mediaControlStack.Children.Add(stopButton);

			var stack = new StackLayout();
			stack.Padding = new Thickness(10);
			stack.Spacing = 10;
			stack.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, false);
			stack.VerticalOptions = new LayoutOptions(LayoutAlignment.Fill, false);
			stack.Children.Add(element);
			stack.Children.Add(label);
			stack.Children.Add(mediaControlStack);
			Content = stack;	
		}

		void PlayButton_Clicked(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(element.CanSeek);
			element.Play();
			System.Diagnostics.Debug.WriteLine(element.CanSeek);
		}

		void PauseButton_Clicked(object sender, EventArgs e)
		{
			element.Pause();
			System.Diagnostics.Debug.WriteLine(element.CanSeek);
		}

		void StopButton_Clicked(object sender, EventArgs e)
		{
			element.Stop();
			System.Diagnostics.Debug.WriteLine(element.CanSeek);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			element.Source = new Uri("https://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4");
		}
	}
}
