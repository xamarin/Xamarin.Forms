using System;

namespace Xamarin.Forms.Controls
{
	public class PinchToZoomContainer : ContentView
	{
		double _currentScale = 1;

		public PinchToZoomContainer()
		{
		}

		public bool AlwaysZoomCenter { get; set; }

		public void AddPinch()
		{
			var pinch = new PinchGestureRecognizer();

			double xOffset = 0;
			double yOffset = 0;
			double startScale = 1;

			pinch.PinchUpdated += (sender, e) =>
			{
				if (e.Status == GestureStatus.Started)
				{
					startScale = Content.Scale;
					Content.AnchorX = Content.AnchorY = 0;
				}
				if (e.Status == GestureStatus.Running)
				{
					_currentScale += (e.Scale - 1) * startScale;
					_currentScale = Math.Max(1, _currentScale);

					double renderedX = Content.X + xOffset;
					double deltaX = renderedX / Width;
					double deltaWidth = Width / (Content.Width * startScale);
					double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

					double renderedY = Content.Y + yOffset;
					double deltaY = renderedY / Height;
					double deltaHeight = Height / (Content.Height * startScale);
					double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

					double targetX = xOffset - originX * Content.Width * (_currentScale - startScale);
					double targetY = yOffset - originY * Content.Height * (_currentScale - startScale);

					Content.TranslationX = targetX.Clamp(-Content.Width * (_currentScale - 1), 0);
					Content.TranslationY = targetY.Clamp(-Content.Height * (_currentScale - 1), 0);

					Content.Scale = _currentScale;
				}
				if (e.Status == GestureStatus.Completed)
				{
					xOffset = Content.TranslationX;
					yOffset = Content.TranslationY;
				}
			};

			GestureRecognizers.Add(pinch);
		}
	}

	public class PinchGestureTestPage : ContentPage
	{
		double _currentScale = 1;

		public PinchGestureTestPage()
		{
			var stack = new StackLayout
			{
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Center
			};
			var textBoxScale = new Label
			{
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Center
			};
			var textBox = new Label { VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.Center };
			var textBoxPoint = new Label
			{
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Center
			};
			stack.Children.Add(textBox);
			stack.Children.Add(textBoxScale);
			stack.Children.Add(textBoxPoint);

			var box = new Image
			{
				Source = "crimson.jpg",
				BackgroundColor = Color.Red,
				WidthRequest = 200,
				HeightRequest = 200,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};

			var zoomContainer = new PinchToZoomContainer();
			zoomContainer.Content = box;

			var btn = new Button { Text = "add pinch gesture", Command = new Command(() => zoomContainer.AddPinch()) };
			var btnRemove = new Button
			{
				Text = "remove pinch gesture",
				Command = new Command(() => zoomContainer.GestureRecognizers.Clear())
			};

			Content = new StackLayout
			{
				Children = { btn, btnRemove, new Grid { Children = { zoomContainer }, Padding = new Thickness(20) } }
			};
		}
	}
}