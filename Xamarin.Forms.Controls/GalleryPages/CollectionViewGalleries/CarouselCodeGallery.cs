using System.Collections;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class CarouselCodeGallery : ContentPage
	{
		public CarouselCodeGallery(ItemsLayoutOrientation orientation)
		{
			Title = $"CarouselView (Code, {orientation})";

			var nItems = 5;
			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star },
					new RowDefinition { Height = 50 }
				}
			};

			var itemsLayout =
				new ListItemsLayout(orientation)
				{
					SnapPointsType = SnapPointsType.MandatorySingle,
					SnapPointsAlignment = SnapPointsAlignment.Center
				};

			var itemTemplate = ExampleTemplates.CarouselTemplate();

			var carouselView = new CarouselView
			{
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate,
				Position = 2
			};

			var indicatorsView = new IndicatorsView
			{
				//BackgroundColor = Color.Red,
				//IndicatorsTintColor = Color.Blue,
				//SelectedIndicatorTintColor = Color.Yellow,
				//IndicatorsShape = IndicatorsShape.Square,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,

			};

			IndicatorsView.SetItemsSourceBy(indicatorsView, carouselView);

			layout.Children.Add(indicatorsView);

			var generator = new ItemsSourceGenerator(carouselView, initialItems: nItems);

			layout.Children.Add(generator);

			var positionControl = new PositionControl(carouselView, nItems);
			layout.Children.Add(positionControl);

			layout.Children.Add(carouselView);

			Grid.SetRow(positionControl, 1);
			Grid.SetRow(carouselView, 3);
			Grid.SetRow(indicatorsView, 4);

			Content = layout;

			generator.GenerateItems();
		}

		internal class PositionControl : ContentView
		{
			public PositionControl(CarouselView carousel, int itemsCount)
			{
				var animateLabel = new Label { Text = "Animate: ", VerticalTextAlignment = TextAlignment.Center };
				var animateSwitch = new Switch { BindingContext = carousel };
				animateSwitch.SetBinding(Switch.IsToggledProperty, nameof(carousel.AnimateTransition), BindingMode.TwoWay);

				var swypeLabel = new Label { Text = "Swype: ", VerticalTextAlignment = TextAlignment.Center };
				var swypeSwitch = new Switch { BindingContext = carousel };
				swypeSwitch.SetBinding(Switch.IsToggledProperty, nameof(carousel.IsSwipeEnabled), BindingMode.TwoWay);

				var slider = new Slider
				{
					BackgroundColor = Color.Pink,
					ThumbColor = Color.Red,
					WidthRequest = 100,
					BindingContext = carousel,
					Maximum = itemsCount,
				};
				slider.SetBinding(Slider.ValueProperty, nameof(carousel.Position));

				var indexLabel = new Label { Text = "Go To Position: ", VerticalTextAlignment = TextAlignment.Center };
				var label = new Label { WidthRequest = 20, BackgroundColor = Color.LightCyan };
				label.SetBinding(Label.TextProperty, nameof(carousel.Position));
				label.BindingContext = carousel;
				var indexButton = new Button { Text = "Go" };

				var layout = new Grid
				{
					RowDefinitions = new RowDefinitionCollection {
								new RowDefinition { Height = GridLength.Auto },
								new RowDefinition { Height = GridLength.Auto },
								new RowDefinition { Height = GridLength.Auto },
					},
					ColumnDefinitions = new ColumnDefinitionCollection
					{
								new ColumnDefinition(),
								new ColumnDefinition(),
								new ColumnDefinition(),
					}
				};

				layout.Children.Add(indexLabel);

				layout.Children.Add(slider);
				Grid.SetColumn(slider, 1);

				layout.Children.Add(label);
				Grid.SetColumn(label, 2);

				var stacklayout = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Children = { animateLabel, animateSwitch, swypeLabel, swypeSwitch }
				};

				layout.Children.Add(stacklayout);
				Grid.SetRow(stacklayout, 2);
				Grid.SetColumnSpan(stacklayout, 3);

				Content = layout;
			}
		}
	}
}