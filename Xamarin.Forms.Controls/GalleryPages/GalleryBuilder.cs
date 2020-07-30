﻿using System;

namespace Xamarin.Forms.Controls.GalleryPages
{
	public static class GalleryBuilder
	{
		public static Button NavButton(string galleryName, Func<Page> gallery, INavigation nav)
		{
			var automationId = System.Text.RegularExpressions.Regex.Replace(galleryName, " |\\(|\\)", string.Empty);
			var button = new Button { Text = $"{galleryName}", AutomationId = automationId, FontSize = 10, HeightRequest = Device.RuntimePlatform == Device.Android ? 40 : 30 };
			button.Clicked += (sender, args) => { nav.PushAsync(gallery()); };
			return button;
		}
	}
}