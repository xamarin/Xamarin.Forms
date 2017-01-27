﻿namespace Xamarin.Forms.Controls
{
	public class BackgroundImageGallery
		: ContentPage
	{
		public BackgroundImageGallery()
		{
			var contentIntabs = new Button { Text = "Tabbed children" };
			contentIntabs.Clicked += async (sender, args) =>
			{
				await Navigation.PushModalAsync(new TabbedPage
				{
					ItemTemplate = new DataTemplate(() =>
					{
						var page = new ContentPage();
						page.SetBinding(BackgroundImageProperty, ".");
						return page;
					}),
					ItemsSource = new[]
					{
						"oasis.jpg",
						"crimson.jpg"
					}
					/*ItemsSource = new[] {
					"http://www.zemwallpaper.com/wp-content/uploads/2014/05/Desktop-background-download-21.jpg",
					"http://www.zemwallpaper.com/wp-content/uploads/2014/05/Desktop-background-download-31.jpg",
					"http://www.zemwallpaper.com/wp-content/uploads/2014/05/Desktop-background-download-61.jpg",
					"http://www.zemwallpaper.com/wp-content/uploads/2014/05/Desktop-background-download-71.jpg"
				};*/
				});
			};

			var contentInCarosel = new Button { Text = "Carousel children" };
			contentInCarosel.Clicked += async (sender, args) =>
			{
				await Navigation.PushModalAsync(new CarouselPage
				{
					ItemTemplate = new DataTemplate(() =>
					{
						var page = new ContentPage();
						page.SetBinding(BackgroundImageProperty, ".");
						return page;
					}),
					ItemsSource = new[]
					{
						"oasis.jpg",
						"crimson.jpg"
					}
					/*ItemsSource = new[] {
					"http://www.zemwallpaper.com/wp-content/uploads/2014/05/Desktop-background-download-21.jpg",
					"http://www.zemwallpaper.com/wp-content/uploads/2014/05/Desktop-background-download-31.jpg",
					"http://www.zemwallpaper.com/wp-content/uploads/2014/05/Desktop-background-download-61.jpg",
					"http://www.zemwallpaper.com/wp-content/uploads/2014/05/Desktop-background-download-71.jpg"
				};*/
				});
			};

			var navigation = new Button { Text = "NavigationPage" };
			navigation.Clicked += async (sender, args) =>
			{
				await Navigation.PushModalAsync(new NavigationPage(new ContentPage
					{
						Content = new Label
						{
							Text = "Text",
							FontSize = 42
						}
					})
					{
						BackgroundImage = "oasis.jpg"
					}
				);
			};

			var carousel = new Button { Text = "CarouselPage" };
			carousel.Clicked += async (sender, args) =>
			{
				await Navigation.PushAsync(new CarouselPage
				{
					BackgroundImage = "crimson.jpg",
					ItemsSource = new[] { "test1", "test2" }
				});
			};

			var tabbed = new Button { Text = "TabbedPage" };
			tabbed.Clicked += async (sender, args) =>
			{
				await Navigation.PushAsync(new TabbedPage
				{
					BackgroundImage = "crimson.jpg",
					ItemsSource = new[] { "test1", "test2" }
				});
			};

			var master = new Button { Text = "MasterDetail" };
			master.Clicked += async (sender, args) =>
			{
				await Navigation.PushModalAsync(new MasterDetailPage
				{
					Master = new ContentPage { Title = "Master" },
					Detail = new ContentPage(),
					BackgroundImage = "crimson.jpg",
				});
			};

			Content = new StackLayout
			{
				Children =
				{
					navigation,
					carousel,
					tabbed,
					master,
					contentIntabs,
					contentInCarosel
				}
			};
		}
	}
}