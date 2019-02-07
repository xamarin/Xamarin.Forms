namespace Xamarin.Forms.Controls
{
	public class MaterialProgressBarGallery : ContentPage
	{
		public MaterialProgressBarGallery()
		{
			Visual = VisualMarker.Material;

			var progressBar = new ProgressBar();

			Slider[] ColorSlider =
			{
				new Slider(0, 1, 0.5),
				new Slider(0, 1, 0.5),
				new Slider(0, 1, 0.5),
				new Slider(0, 1, 0.5)
			};
			Slider[] BackColorSlider =
			{
				new Slider(0, 1, 0.5),
				new Slider(0, 1, 0.5),
				new Slider(0, 1, 0.5),
				new Slider(0, 1, 0.5)
			};

			var actions = new Grid()
			{
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = 10 },
					new ColumnDefinition { Width = GridLength.Star },
					new ColumnDefinition { Width = 30 },
					new ColumnDefinition { Width = 10 },
					new ColumnDefinition { Width = GridLength.Star },
					new ColumnDefinition { Width = 30 },
				}
			};

			actions.AddChild(new Label { Text = "Primary color" }, 0, 0, 3);
			for (int i = 0; i < ColorSlider.Length; i++)
			{
				var valueLabel = new Label();
				ColorSlider[i].ValueChanged += (_, e) =>
				{
					progressBar.ProgressColor = Color.FromRgba(ColorSlider[0].Value, ColorSlider[1].Value, ColorSlider[2].Value, ColorSlider[3].Value);
					valueLabel.Text = ((int)(e.NewValue * 255)).ToString();
				};
				actions.AddChild(new Label { Text = GetColorLetter(i) }, 0, i + 1);
				actions.AddChild(ColorSlider[i], 1, i + 1);
				actions.AddChild(valueLabel, 2, i + 1);
			}

			actions.AddChild(new Label { Text = "Background color" }, 3, 0, 3);
			for (int i = 0; i < BackColorSlider.Length; i++)
			{
				var valueLabel = new Label();
				BackColorSlider[i].ValueChanged += (_, e) =>
				{
					progressBar.BackgroundColor = Color.FromRgba(BackColorSlider[0].Value, BackColorSlider[1].Value, BackColorSlider[2].Value, BackColorSlider[3].Value);
					valueLabel.Text = ((int)(e.NewValue * 255)).ToString();
				};
				actions.AddChild(new Label { Text = GetColorLetter(i) }, 3, i + 1);
				actions.AddChild(BackColorSlider[i], 4, i + 1);
				actions.AddChild(valueLabel, 5, i + 1);
			}

			var valueSlider = new Slider(0, 1, progressBar.Progress);
			valueSlider.ValueChanged += (_, e) => progressBar.Progress = e.NewValue;
			actions.AddChild(new Label { Text = "Value" }, 0, 5, 6);
			actions.AddChild(valueSlider, 0, 6, 6);

			var heightSlider = new Slider(0, 100, 4);
			heightSlider.ValueChanged += (_, e) => progressBar.HeightRequest = e.NewValue;
			actions.AddChild(new Label { Text = "HeightRequest" }, 0, 7, 6);
			actions.AddChild(heightSlider, 0, 8, 6);

			Content = new StackLayout
			{
				Children =
				{
					actions,
					progressBar
				}
			};
		}

		string GetColorLetter(int index)
		{
			switch (index)
			{
				case 0:
					return "R";
				case 1:
					return "G";
				case 2:
					return "B";
				default:
					return "A";
			}
		}
	}
}
