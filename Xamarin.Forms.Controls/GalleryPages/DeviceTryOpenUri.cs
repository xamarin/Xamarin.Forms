using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Controls.GalleryPages
{
	class DeviceTryOpenUriPage : ContentPage
	{
		public DeviceTryOpenUriPage()
		{
			var instructions = new Label
			{
				Text = "This calls Device.TryOpenUri with the given uri." +
				"\nDifferent than OpenUri, TryOpenUri never throws an exception if the call failed." +
				"\nOn Android for example, calling OpenUri with an invalid uri throws an exception."
			};

			Title = "Issue 6221";

			var entry = new Entry() { Text = "someProtocol:Something" };

			var label = new Label();

			var btn = new Button() { Text = "Call Device.TryOpenUri" };
			btn.Clicked += (s, e) =>
			{
				label.Text = "Result: " + Device.TryOpenUri(new Uri(entry.Text, UriKind.Absolute)).ToString();
			};

			var layout = new StackLayout
			{
				Spacing = 10,
				Children =  {
					instructions,
					entry,
					label,
					btn,
				}
			};

			Padding = 15;
			Content = layout;
		}
	}
}
