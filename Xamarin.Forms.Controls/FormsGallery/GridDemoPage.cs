using System;
using Xamarin.Forms;

namespace FormsGallery
{
	class GridDemoPage : ContentPage
	{
		public GridDemoPage ()
		{
			Grid grid = new Grid {
				VerticalOptions = LayoutOptions.FillAndExpand,
				RowDefinitions = {
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = new GridLength (1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength (100, GridUnitType.Absolute) }
				},
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (100, GridUnitType.Absolute) }
				}
			};

			grid.Children.Add (new Label {
				Text = "Grid",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center
			}, 0, 3, 0, 1);

			grid.Children.Add (new Label {
				Text = "Autosized cell",
				TextColor = Color.White,
				BackgroundColor = Color.Blue
			}, 0, 1);

			grid.Children.Add (new BoxView {
				Color = Color.Silver,
				HeightRequest = 0
			}, 1, 1);

			grid.Children.Add (new BoxView {
				Color = Color.Teal
			}, 0, 2);

			grid.Children.Add (new Label {
				Text = "Leftover space",
				TextColor = Color.Purple,
				BackgroundColor = Color.Aqua,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
			}, 1, 2);

			grid.Children.Add (new Label {
				Text = "Span two rows (or more if you want)",
				TextColor = Color.Yellow,
				BackgroundColor = Color.Navy,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			}, 2, 3, 1, 3);

			grid.Children.Add (new Label {
				Text = "Span 2 columns",
				TextColor = Color.Blue,
				BackgroundColor = Color.Yellow,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			}, 0, 2, 3, 4);

			grid.Children.Add (new Label {
				Text = "Fixed 100x100",
				TextColor = Color.Aqua,
				BackgroundColor = Color.Red,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			}, 2, 3);

			// Build the page.
			this.Content = grid;
		}
	}
}
