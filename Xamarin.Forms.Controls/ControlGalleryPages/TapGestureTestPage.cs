using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.Controls
{
	public class TapGestureTestPage : ContentPage
	{
		readonly Label outputLabel;

		public TapGestureTestPage()
		{
			var grid = new Grid
			{
				ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } },
				RowDefinitions = new RowDefinitionCollection { new RowDefinition {Height = GridLength.Auto},new RowDefinition {Height = GridLength.Auto}, new RowDefinition {Height = GridLength.Star} }
			};

			var picker = new Picker { ItemsSource = new List<int> { 1, 2 }, Title = "Number of taps" };
			grid.AddChild(picker, 0, 0);

			outputLabel = new Label { Text = "Nothing tapped yet!", LineBreakMode = LineBreakMode.WordWrap, HorizontalTextAlignment = TextAlignment.Center };
			grid.AddChild(outputLabel, 0, 1, 2);

			var box = new BoxView { BackgroundColor = Color.Gray };
			grid.AddChild(box, 0, 2, 2);

			var button = new Button { Text = "Set", Command = new Command(() => UpdateGesture(picker, box)) };
			grid.AddChild(button, 1, 0);
			
			Content = grid;

			picker.SelectedItem = 1;
			button.Command.Execute(null);
		}

		void UpdateGesture(Picker settings, BoxView box)
		{
			if (settings.SelectedItem is int count)
			{
				var gestureRecognizer = new TapGestureRecognizer { NumberOfTapsRequired = count };
				gestureRecognizer.Tap += OnTap;

				foreach (var recognizer in box.GestureRecognizers.Where(r => r is TapGestureRecognizer).Cast<TapGestureRecognizer>())
					recognizer.Tap -= OnTap;

				box.GestureRecognizers.Clear();
				box.GestureRecognizers.Add(gestureRecognizer);
			}
		}

		void OnTap(object sender, TapEventArgs args)
		{
			outputLabel.Text = $"Tapped at {args.Position}";
		}
	}
}