using System;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatusBarGallery : ContentPage
	{
		public StatusBarGallery()
		{
			InitializeComponent();
		}

		void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
		{
			StatusBarColor = Color.FromRgb(Convert.ToInt32(RedSlider.Value), Convert.ToInt32(GreenSlider.Value), Convert.ToInt32(BlueSlider.Value));
		}

		void Switch_OnToggled(object sender, ToggledEventArgs e)
		{
			StatusBarStyle = e.Value ? StatusBarStyle.DarkContent : StatusBarStyle.LightContent;
		}

		void NavigationPage_Navigate(object sender, EventArgs e)
		{
			var page = new NavigationPage(new StatusBarGallery());
			page.StatusBarColor = Color.DarkBlue;
			page.StatusBarStyle = StatusBarStyle.DarkContent;
			Application.Current.MainPage = page;
		}

		void NavigationPage_Pages_Navigate(object sender, EventArgs e)
		{
			var page = new NavigationPage(new StatusBarGallery()
			{
				StatusBarColor = Color.Red,
				StatusBarStyle = StatusBarStyle.LightContent
			});
			Application.Current.MainPage = page;
			page.PushAsync(new StatusBarGallery()
			{
				StatusBarColor = Color.Cyan, 
				StatusBarStyle = StatusBarStyle.DarkContent
			});
		}

		void ContentPage_Navigate(object sender, EventArgs e)
		{
			var page = new StatusBarGallery()
			{
				StatusBarColor = Color.DarkTurquoise,
				StatusBarStyle = StatusBarStyle.DarkContent
			};
			Application.Current.MainPage = page;
		}

		void TabbedPage_Navigate(object sender, EventArgs e)
		{
			var page = new TabbedPage();
			page.Children.Add(new StatusBarGallery());
			page.Children.Add(new StatusBarGallery());
			page.StatusBarColor = Color.DarkCyan;
			page.StatusBarStyle = StatusBarStyle.DarkContent;
			Application.Current.MainPage = page;
		}

		void TabbedPage_Pages_Navigate(object sender, EventArgs e)
		{
			var page = new TabbedPage();
			page.Children.Add(new StatusBarGallery()
			{
				StatusBarColor = Color.Red,
				StatusBarStyle = StatusBarStyle.LightContent
			});
			page.Children.Add(new StatusBarGallery()
			{
				StatusBarColor = Color.Cyan,
				StatusBarStyle = StatusBarStyle.DarkContent
			});
			Application.Current.MainPage = page;
		}

		void CarouselPage_Navigate(object sender, EventArgs e)
		{
			var page = new CarouselPage();
			page.Children.Add(new StatusBarGallery());
			page.Children.Add(new StatusBarGallery());
			page.StatusBarColor = Color.DarkMagenta;
			page.StatusBarStyle = StatusBarStyle.DarkContent;
			Application.Current.MainPage = page;
		}

		void CarouselPage_Pages_Navigate(object sender, EventArgs e)
		{
			var page = new CarouselPage();
			page.Children.Add(new StatusBarGallery()
			{
				StatusBarColor = Color.Red,
				StatusBarStyle = StatusBarStyle.LightContent
			});
			page.Children.Add(new StatusBarGallery()
			{
				StatusBarColor = Color.Cyan,
				StatusBarStyle = StatusBarStyle.DarkContent
			});
			Application.Current.MainPage = page;
		}

		void Shell_Navigate(object sender, EventArgs e)
		{
			var shell = new Shell();
			shell.Items.Add(new TabBar()
			{
				Items = { new Tab
				{
					Items = { new ShellContent()
					{
						Content = new StatusBarGallery()
					}}
				}}
			});
			shell.StatusBarColor = Color.DarkGoldenrod;
			shell.StatusBarStyle = StatusBarStyle.DarkContent;

			Application.Current.MainPage = shell;
		}


	}
}