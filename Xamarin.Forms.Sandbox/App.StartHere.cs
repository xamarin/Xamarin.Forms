using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Sandbox
{
	public partial class App 
	{
		// This code is called from the App Constructor so just initialize the main page of the application here
		void InitializeMainPage()
		{
			Label label;

			MainPage = CreateContentPage(new StackLayout
			{
				Padding = 20,
				Spacing = 20,
				Children =
				{
					(label = new Label { Text = "Click a button..." }),
					new Button
					{
						Text = "Default",

						Command = new Command(() => label.Text = "Clicked the default button."),
					},
					new Button
					{
						Text = "Material",

						Command = new Command(() => label.Text = "Clicked the Material button."),

						Visual = VisualMarker.Material,
					},
					new Button
					{
						Text = "SkiaSharp",
						CornerRadius = 4,
						BackgroundColor = Color.DarkOrange,
						BorderColor = Color.Red,
						BorderWidth = 4,

						Command = new Command(() => label.Text = "Clicked the SkiaSharp button."),

						Visual = SkiaSharpVisual.SkiaSharp.Instance,
						WidthRequest = 100,
						HeightRequest = 100
					}
				}
			});
		}
	}
}
