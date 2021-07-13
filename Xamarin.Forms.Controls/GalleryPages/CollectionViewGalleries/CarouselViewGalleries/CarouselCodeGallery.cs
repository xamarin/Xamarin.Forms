using Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.SpacingGalleries;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.CarouselViewGalleries
{
	[Preserve(AllMembers = true)]
	internal class CarouselCodeGallery : ContentPage
	{
		readonly Label _scrollInfoLabel = new Label();
		readonly ItemsLayoutOrientation _orientation;
		ItemsSourceGenerator _generator;
		PositionControl _positionControl;
		CarouselView _carouselView;
		Slider _padiSlider;
		public CarouselCodeGallery(ItemsLayoutOrientation orientation)
		{
			On<iOS>().SetLargeTitleDisplay(LargeTitleDisplayMode.Never);

			_scrollInfoLabel.MaxLines = 1;
			_scrollInfoLabel.LineBreakMode = LineBreakMode.TailTruncation;
			_orientation = orientation;

			Title = $"CarouselView (Code, {orientation})";

			var nItems = 5;
			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star }
				}
			};
			var itemsLayout =
			new LinearItemsLayout(orientation)
			{
				SnapPointsType = SnapPointsType.MandatorySingle,
				SnapPointsAlignment = SnapPointsAlignment.Center
			};

			var itemTemplate = ExampleTemplates.CarouselTemplate();

			_carouselView = new CarouselView
			{
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate,
				Margin = new Thickness(0, 10, 0, 10),
				BackgroundColor = Color.Red,
				AutomationId = "TheCarouselView",
				//Loop = false
			};

			if (orientation == ItemsLayoutOrientation.Horizontal)
				_carouselView.PeekAreaInsets = new Thickness(30, 0, 30, 0);
			else
				_carouselView.PeekAreaInsets = new Thickness(0, 30, 0, 30);

			StackLayout stacklayoutInfo = GetReadOnlyInfo(_carouselView);

			_generator = new ItemsSourceGenerator(_carouselView, initialItems: nItems, itemsSourceType: ItemsSourceType.ObservableCollection);

			_positionControl = new PositionControl(_carouselView, nItems);

			var spacingModifier = new SpacingModifier(_carouselView.ItemsLayout, "Update Spacing");

			var stckPeek = new StackLayout { Orientation = StackOrientation.Horizontal };
			stckPeek.Children.Add(new Label { Text = "Peek" });
			_padiSlider = new Slider
			{
				Maximum = 100,
				Minimum = 0,
				Value = 30,
				WidthRequest = 100,
				BackgroundColor = Color.Pink
			};

			stckPeek.Children.Add(_padiSlider);

			var content = new Grid();
			content.Children.Add(_carouselView);

#if DEBUG
			// Uncomment this line to add a helper to visualize the center of each element.
			//content.Children.Add(CreateDebuggerLines());
#endif
			layout.Children.Add(_generator);
			layout.Children.Add(_positionControl);
			layout.Children.Add(stacklayoutInfo);
			layout.Children.Add(stckPeek);
			layout.Children.Add(spacingModifier);
			layout.Children.Add(_scrollInfoLabel);
			layout.Children.Add(content);

			Grid.SetRow(_positionControl, 1);
			Grid.SetRow(stacklayoutInfo, 2);
			Grid.SetRow(stckPeek, 3);
			Grid.SetRow(spacingModifier, 4);
			Grid.SetRow(_scrollInfoLabel, 5);
			Grid.SetRow(content, 6);

			Content = layout;

			_generator.GenerateItems();
			_positionControl.UpdatePosition(1);
		}

		private void Padi_ValueChanged(object sender, ValueChangedEventArgs e)
		{
			var peek = _padiSlider.Value;

			if (_orientation == ItemsLayoutOrientation.Horizontal)
				_carouselView.PeekAreaInsets = new Thickness(peek, 0, peek, 0);
			else
				_carouselView.PeekAreaInsets = new Thickness(0, peek, 0, peek);
		}

		private void Generator_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			_positionControl.UpdatePositionCount(_generator.Count);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_carouselView.Scrolled += CarouselViewScrolled;

			_padiSlider.ValueChanged += Padi_ValueChanged;

			_generator.CollectionChanged += Generator_CollectionChanged;

			_generator.SubscribeEvents();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_carouselView.Scrolled -= CarouselViewScrolled;

			_padiSlider.ValueChanged -= Padi_ValueChanged;

			_generator.CollectionChanged -= Generator_CollectionChanged;

			_generator.UnsubscribeEvents();
		}

		void CarouselViewScrolled(object sender, ItemsViewScrolledEventArgs e)
		{
			_scrollInfoLabel.Text = $"First item: {e.FirstVisibleItemIndex}, Last item: {e.LastVisibleItemIndex}";

			double delta;
			double offset;

			if (_orientation == ItemsLayoutOrientation.Horizontal)
			{
				delta = e.HorizontalDelta;
				offset = e.HorizontalOffset;
			}
			else
			{
				delta = e.VerticalDelta;
				offset = e.VerticalOffset;
			}

			_scrollInfoLabel.Text += $", Delta: {delta}, Offset: {offset}";
		}

		static StackLayout GetReadOnlyInfo(CarouselView carouselView)
		{
			var stacklayoutInfo = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				BindingContext = carouselView
			};
			var labelDragging = new Label { Text = nameof(carouselView.IsDragging) };
			var switchDragging = new Switch();

			switchDragging.SetBinding(Switch.IsToggledProperty, nameof(carouselView.IsDragging), BindingMode.OneWay);
			stacklayoutInfo.Children.Add(labelDragging);
			stacklayoutInfo.Children.Add(switchDragging);

			return new StackLayout { Children = { stacklayoutInfo } };
		}
#if DEBUG
		Grid CreateDebuggerLines()
		{
			var grid = new Grid
			{
				InputTransparent = true,
				Margin = new Thickness(0, 10, 0, 10)
			};

			var horizontalLine = new Grid
			{
				HeightRequest = 1,
				BackgroundColor = Color.Red,
				VerticalOptions = LayoutOptions.Center
			};

			grid.Children.Add(horizontalLine);

			var verticalLine = new Grid
			{
				WidthRequest = 1,
				BackgroundColor = Color.Red,
				HorizontalOptions = LayoutOptions.Center
			};

			grid.Children.Add(verticalLine);

			return grid;
		}
#endif
	}
}