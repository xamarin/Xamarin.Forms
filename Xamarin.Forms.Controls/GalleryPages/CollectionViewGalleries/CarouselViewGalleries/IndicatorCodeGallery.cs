using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.CarouselViewGalleries
{
	[Preserve(AllMembers = true)]
	public class IndicatorCodeGallery : ContentPage
	{
		CarouselView _carouselView;
		public IndicatorCodeGallery()
		{
			Title = "IndicatorView Gallery";

			On<iOS>().SetLargeTitleDisplay(LargeTitleDisplayMode.Never);

			var nItems = 10;

			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto }
				}
			};

			var itemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Horizontal)
			{
				SnapPointsType = SnapPointsType.MandatorySingle,
				SnapPointsAlignment = SnapPointsAlignment.Center
			};

			var itemTemplate = ExampleTemplates.CarouselTemplate();

			_carouselView = new CarouselView
			{
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate,
				BackgroundColor = Color.LightGray,
				AutomationId = "TheCarouselView"
			};

			layout.Children.Add(_carouselView);
			var generator = new ItemsSourceGenerator(_carouselView, nItems, ItemsSourceType.ObservableCollection);

			layout.Children.Add(generator);

			generator.GenerateItems();

			var indicatorView = new IndicatorView
			{
				HorizontalOptions = LayoutOptions.Center,
				Margin = new Thickness(12, 6, 12, 12),
				IndicatorColor = Color.Gray,
				SelectedIndicatorColor = Color.Black,
				IndicatorsShape = IndicatorShape.Square,
				AutomationId = "TheIndicatorView",
				Count = 5,
				Position = 2
			};

			_carouselView.IndicatorView = indicatorView;

			layout.Children.Add(indicatorView);

			var stckColors = new StackLayout { Orientation = StackOrientation.Horizontal };
			stckColors.Children.Add(new Label { VerticalOptions = LayoutOptions.Center, Text = "IndicatorColor" });

			var colors = new List<string>
   			{
				"Black",
				"Blue",
				"Red"
			};

			var colorsPicker = new Picker
			{
				ItemsSource = colors,
				WidthRequest = 150
			};
			colorsPicker.SelectedIndex = 0;

			colorsPicker.SelectedIndexChanged += (s, e) =>
			{
				var selectedIndex = colorsPicker.SelectedIndex;

				switch (selectedIndex)
				{
					case 0:
						indicatorView.IndicatorColor = Color.Black;
						break;
					case 1:
						indicatorView.IndicatorColor = Color.Blue;
						break;
					case 2:
						indicatorView.IndicatorColor = Color.Red;
						break;
				}
			};

			stckColors.Children.Add(colorsPicker);

			layout.Children.Add(stckColors);

			var stckTemplate = new StackLayout { Orientation = StackOrientation.Horizontal };
			stckTemplate.Children.Add(new Label { VerticalOptions = LayoutOptions.Center, Text = "IndicatorTemplate" });

			var templates = new List<string>
			{
				"Circle",
				"Square",
				"Template"
			};

			var templatePicker = new Picker
			{
				ItemsSource = templates,
				WidthRequest = 150,
				TextColor = Color.Black
			};

			templatePicker.SelectedIndexChanged += (s, e) =>
			{
				var selectedIndex = templatePicker.SelectedIndex;

				switch (selectedIndex)
				{
					case 0:
						indicatorView.IndicatorTemplate = null;
						indicatorView.IndicatorsShape = IndicatorShape.Circle;
						break;
					case 1:
						indicatorView.IndicatorTemplate = null;
						indicatorView.IndicatorsShape = IndicatorShape.Square;
						break;
					case 2:
						indicatorView.IndicatorTemplate = ExampleTemplates.IndicatorTemplate();
						break;
				}
			};

			templatePicker.SelectedIndex = 0;

			stckTemplate.Children.Add(templatePicker);

			layout.Children.Add(stckTemplate);

			var stckSize = new StackLayout { Orientation = StackOrientation.Horizontal };
			stckSize.Children.Add(new Label { VerticalOptions = LayoutOptions.Center, Text = "Indicator Size" });

			//indicatorView.IndicatorSize = 25;

			var sizeSlider = new Slider
			{
				WidthRequest = 150,
				Value = indicatorView.IndicatorSize,
				Maximum = 50,
			};

			sizeSlider.ValueChanged += (s, e) =>
			{
				var indicatorSize = sizeSlider.Value;
				indicatorView.IndicatorSize = indicatorSize;
			};

			stckSize.Children.Add(sizeSlider);

			layout.Children.Add(stckSize);

			Grid.SetRow(generator, 0);
			Grid.SetRow(stckColors, 1);
			Grid.SetRow(stckTemplate, 2);
			Grid.SetRow(stckSize, 3);
			Grid.SetRow(_carouselView, 4);
			Grid.SetRow(indicatorView, 6);

			var btn = new Button
			{
				Text = "Remove",
				Command = new Command(() =>
				{
					var items = (_carouselView.ItemsSource as ObservableCollection<CollectionViewGalleryTestItem>);
					items.Remove(items[0]);
				})
			};
			_carouselView.PropertyChanged += CarouselView_PropertyChanged;
			layout.Children.Add(btn);
			Grid.SetRow(btn, 5);
			Content = layout;
		}

		void CarouselView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName ==  nameof(CarouselView.Position))
				System.Diagnostics.Debug.WriteLine($"Position {_carouselView.Position}");
		}
	}
}