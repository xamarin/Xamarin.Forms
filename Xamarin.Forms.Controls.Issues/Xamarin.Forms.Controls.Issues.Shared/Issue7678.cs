using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.CarouselView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7678, "[Android] CarouselView binded to a new ObservableCollection filled with Items does not render content", PlatformAffected.Android)]
	public class Issue7678Droid : TestContentPage
	{
		public Issue7678Droid()
		{
			Title = "Issue 7678";
			BindingContext = new Issue7678DroidViewModel();
		}

		protected override void Init()
		{
			var layout = new StackLayout();

			var instructions = new Label
			{
				Margin = new Thickness(6),
				Text = "The CarouselView below must show items. If you do not see any item, this test has failed."
			};

			var itemsLayout =
				new LinearItemsLayout(ItemsLayoutOrientation.Horizontal)
				{
					SnapPointsType = SnapPointsType.MandatorySingle,
					SnapPointsAlignment = SnapPointsAlignment.Center
				};

			var carouselView = new CarouselView
			{
				ItemsLayout = itemsLayout,
				ItemTemplate = GetCarouselTemplate()
			};

			carouselView.SetBinding(ItemsView.ItemsSourceProperty, "Items");

			layout.Children.Add(instructions);
			layout.Children.Add(carouselView);

			Content = layout;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await ((Issue7678DroidViewModel)BindingContext).LoadItemsAsync();
		}

		internal DataTemplate GetCarouselTemplate()
		{
			return new DataTemplate(() =>
			{
				var grid = new Grid();

				var info = new Label
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Margin = new Thickness(6)
				};

				info.SetBinding(Label.TextProperty, new Binding("Name"));

				grid.Children.Add(info);

				var frame = new Frame
				{
					Content = grid,
					HasShadow = false
				};

				frame.SetBinding(BackgroundColorProperty, new Binding("Color"));

				return frame;
			});
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue7678DroidViewModel : BindableObject
	{
		ObservableCollection<CarouselData> _items;
  
		public ObservableCollection<CarouselData> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		public async Task LoadItemsAsync()
		{
			Items = new ObservableCollection<CarouselData>();

			var random = new Random();
			var items = new List<CarouselData>();

			for (int n = 0; n < 5; n++)
			{
				items.Add(new CarouselData
				{
					Color = Color.FromRgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)),
					Name = $"{n + 1}"
				});
			}

			await Task.Delay(500);

			_items = new ObservableCollection<CarouselData>(items);
			OnPropertyChanged(nameof(Items));
		}
	}
}
