﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Controls
{
	internal class MediaElementDemoPage : ContentPage
	{
		MediaElement element;
		Label consoleLabel;

		public MediaElementDemoPage()
		{
			element = new MediaElement();
			element.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill,true);
			element.VerticalOptions = new LayoutOptions(LayoutAlignment.Fill,true);
			element.AutoPlay = false;
			element.AreTransportControlsEnabled = true;
			element.BackgroundColor = Color.Red;
			element.MediaEnded += Element_MediaEnded;
			element.MediaFailed += Element_MediaFailed;
			element.MediaOpened += Element_MediaOpened;
			consoleLabel = new Label();

			var infoStack = new StackLayout { Orientation = StackOrientation.Horizontal };

			var stateLabel = new Label();
			stateLabel.SetBinding(Label.TextProperty, new Binding("CurrentState", BindingMode.OneWay, null, null, null, element));
			var heightLabel = new Label();
			heightLabel.SetBinding(Label.TextProperty, new Binding("VideoHeight", BindingMode.OneWay, null, null, null, element));
			var widthLabel = new Label();
			widthLabel.SetBinding(Label.TextProperty, new Binding("VideoWidth", BindingMode.OneWay, null, null, null, element));
			var durationLabel = new Label();
			durationLabel.SetBinding(Label.TextProperty, new Binding("Duration", BindingMode.OneWay, null, null, "{0:g}", element));

			infoStack.Children.Add(stateLabel);
			infoStack.Children.Add(heightLabel);
			infoStack.Children.Add(widthLabel);
			infoStack.Children.Add(durationLabel);

			var positionLabel = new Label();
			positionLabel.TextColor = Color.Black;
			positionLabel.SetBinding(Label.TextProperty, new Binding("Position", BindingMode.OneWay, null, null, "{0:g}", element));

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
			stack.Children.Add(infoStack);
			stack.Children.Add(positionLabel);
			stack.Children.Add(mediaControlStack);
			stack.Children.Add(consoleLabel);
			Content = stack;	
		}

		private void Element_MediaOpened(object sender, EventArgs e)
		{
			consoleLabel.Text += "Media opened\r\n";
		}

		private void Element_MediaFailed(object sender, EventArgs e)
		{
			consoleLabel.Text += "Media failed\r\n";
		}

		private void Element_MediaEnded(object sender, EventArgs e)
		{
			consoleLabel.Text += "Media ended\r\n";
		}

		void PlayButton_Clicked(object sender, EventArgs e)
		{
			element.Play();
		}

		void PauseButton_Clicked(object sender, EventArgs e)
		{
			element.Pause();
		}

		void StopButton_Clicked(object sender, EventArgs e)
		{
			element.Stop();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			element.Source = new Uri("https://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4");
		}
	}
}
