using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

// Map requires:
//      - Xamarin.Forms.Maps and Xamarin.Forms.Maps.* assemblies, 
//      - a platform-specific call to Xamarin.FormsMaps.Init() in:
//              - iOS: AppDelegate.cs
//              - Android: MainActivity.cs
//              - WP: MainPage.xaml.cs 
//      - platform-specific permission:
//              - iOS: None
//              - Android: API key!
//              - WP: WMAppManifest.xaml, Capabilities, ID_CAP_MAP

namespace FormsGallery
{
	class MapDemoPage : ContentPage
	{
		public MapDemoPage()
		{
			Label header = new Label
			{
				Text = "Map",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center
			};

			View view;

			if (Device.RuntimePlatform == Device.Android)
			{
				view = new Label
				{
					Text = "Android applications require API key " +
						   "to use the Google Map service.",
					FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
					VerticalOptions = LayoutOptions.CenterAndExpand
				};
			}
			else
			{
				Map map = new Map();
				view = map;

				// Let's visit Xamarin HQ in San Francisco!
				Position position = new Position(37.79762, -122.40181);
				map.MoveToRegion(new MapSpan(position, 0.01, 0.01));
				map.Pins.Add(new Pin
				{
					Label = "Xamarin",
					Position = position
				});
			}

			// Build the page.
			this.Content = new StackLayout
			{
				Children =
				{
					header,
					view
				}
			};
		}
	}
}
