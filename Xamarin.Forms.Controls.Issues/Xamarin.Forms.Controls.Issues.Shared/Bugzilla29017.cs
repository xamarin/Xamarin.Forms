﻿using System;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Maps;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(Core.UITests.UITestCategories.Bugzilla)]
#endif
	[Issue(IssueTracker.Bugzilla, 29017, "Pin clicked does not work on iOS maps")]
	public class Issue29017 : TestContentPage // or TestFlyoutPage, etc ...
	{
		Label _lbl;

		protected override void Init()
		{
			var map = new Map
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			_lbl = new Label
			{
				Text = "Not Clicked"
			};

			Content = new StackLayout
			{
				Children = {
					new Button {
						Text = "Add pins",
						Command = new Command (() => {
							foreach (var pin in map.Pins) {
#pragma warning disable CS0618
								pin.Clicked -= PinClicked;
#pragma warning restore CS0618
							}

							map.Pins.Clear ();

							for (int i = 0; i < 100; i++) {
								var rnd = new Random ();
								var lat = rnd.NextDouble () / 10;
								var lng = rnd.NextDouble () / 10;

								if (i % 2 == 0) {
									lat = -lat;
									lng = -lng;
								}

								var pin = new Pin {
									Address = "address",
									Label = "label",
									Type = PinType.Place,
									Position = new Position (map.VisibleRegion.Center.Latitude + lat, map.VisibleRegion.Center.Longitude + lng)
								};
#pragma warning disable CS0618
								pin.Clicked += PinClicked;
#pragma warning restore CS0618
								map.Pins.Add (pin);
							}
						})
					},
					_lbl,
					map
				}
			};
		}

		void PinClicked(object sender, EventArgs e)
		{
			_lbl.Text = "Click " + DateTime.Now.ToLocalTime();
		}
	}
}
