namespace Xamarin.Forms.Sandbox
{
	public class CodePage : ContentPage
	{
		Label _label;

		public CodePage()
		{
			Title = "Code Page";
			Content = new StackLayout
			{
				Padding = 20,
				Spacing = 20,
				Children =
				{
					(_label = new Label { Text = "Click a button..." }),
					new Button
					{
						Text = "Default",

						Command = new Command(() => _label.Text = "Clicked the default button."),
					},
					new Button
					{
						Text = "Material",

						Command = new Command(() => _label.Text = "Clicked the Material button."),

						Visual = VisualMarker.Material,
					},
					new Button
					{
						Text = "SkiaSharp",
						CornerRadius = 4,
						BackgroundColor = Color.DarkOrange,
						BorderColor = Color.Red,
						BorderWidth = 4,

						Command = new Command(() => _label.Text = "Clicked the SkiaSharp button."),

						Visual = SkiaSharpVisual.SkiaSharp.Instance,

						// TODO: implement the sizing logic for the custom renderers
						WidthRequest = 100,
						HeightRequest = 100
					},

					new Button
					{
						Text = "Go To XAML",
						Command = new Command(() => Navigation.PushAsync(new XamlPage()))
					}
				}
			};
		}
	}
}
