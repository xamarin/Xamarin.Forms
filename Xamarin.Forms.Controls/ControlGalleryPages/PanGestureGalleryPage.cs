using System;

namespace Xamarin.Forms.Controls
{
	public class PanGestureGalleryPage : ContentPage
	{
		public PanGestureGalleryPage()
		{
			var box = new Image
			{
				BackgroundColor = Color.Gray,
				WidthRequest = 2000,
				HeightRequest = 2000,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};

			var label = new Label { Text = "Use two fingers to pinch. Use one finger to pan." };

			var panme = new PanContainer { Content = box };
			panme.PanCompleted += (s, e) => { label.Text = e.Message; };

			Content = new StackLayout { Children = { label, panme }, Padding = new Thickness(20) };
		}

		public class PanCompleteArgs : EventArgs
		{
			public PanCompleteArgs(string message)
			{
				Message = message;
			}

			public string Message { get; private set; }
		}

		public class PanContainer : ContentView
		{
			double _currentScale = 1;
			double _x, _y;

			public EventHandler<PanCompleteArgs> PanCompleted;

			public PanContainer()
			{
				GestureRecognizers.Add(GetPinch());
				GestureRecognizers.Add(GetPan());
			}

			PanGestureRecognizer GetPan()
			{
				var pan = new PanGestureRecognizer();
				pan.PanUpdated += (s, e) =>
				{
					switch (e.StatusType)
					{
						case GestureStatus.Running:
							Content.TranslationX = e.TotalX;
							Content.TranslationY = e.TotalY;
							break;

						case GestureStatus.Completed:
							_x = Content.TranslationX;
							_y = Content.TranslationY;

							PanCompleted?.Invoke(s, new PanCompleteArgs($"x: {_x}, y: {_y}"));
							break;
					}
				};
				return pan;
			}

			PinchGestureRecognizer GetPinch()
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
				return pinch;
			}
		}
	}
}