using System.Collections;
using Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.SpacingGalleries;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.CarouselViewGalleries
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
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star }
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
				Position = 2,
			//	NumberOfSideItems = 1,
				Margin = new Thickness(0,10,0,40),
				PeekAreaInsets = new Thickness(30,0,30,0),
				BackgroundColor = Color.LightGray,
				AutomationId = "TheCarouselView"
			};

			layout.Children.Add(carouselView);

			StackLayout stacklayoutInfo = GetReadOnlyInfo(carouselView);

			var generator = new ItemsSourceGenerator(carouselView, initialItems: nItems, itemsSourceType: ItemsSourceType.ObservableCollection);

			layout.Children.Add(generator);

			var positionControl = new PositionControl(carouselView, nItems);
			layout.Children.Add(positionControl);

			var spacingModifier = new SpacingModifier(carouselView, "Update Spacing");

			layout.Children.Add(spacingModifier);

			layout.Children.Add(stacklayoutInfo);

			var stckPeek = new StackLayout { Orientation = StackOrientation.Horizontal };
			stckPeek.Children.Add(new Label { Text = "Peek" });
			var padi = new Slider
			{
				Maximum = 100,
				Minimum = 0,
				Value = 30
			};

			padi.ValueChanged += (s, e) => {
				var peek = padi.Value;
				carouselView.PeekAreaInsets = new Thickness(peek, 0, peek, 0);
			};

			stckPeek.Children.Add(padi);
			stacklayoutInfo.Children.Add(stckPeek);
			
			Grid.SetRow(positionControl, 1);
			Grid.SetRow(stacklayoutInfo, 2);
			Grid.SetRow(spacingModifier, 3);
			Grid.SetRow(carouselView, 4);

			Content = layout;
			generator.CollectionChanged += (sender, e) => {
				positionControl.UpdatePositionCount(generator.Count);
			};

			generator.GenerateItems();
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

			switchDragging.SetBinding(Switch.IsToggledProperty, nameof(carouselView.IsDragging));
			stacklayoutInfo.Children.Add(labelDragging);
			stacklayoutInfo.Children.Add(switchDragging);

			return new StackLayout { Children = { stacklayoutInfo } };
		}
	}
}