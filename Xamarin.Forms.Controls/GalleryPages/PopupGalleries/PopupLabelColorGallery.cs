namespace Xamarin.Forms.Controls
{
	public class PopupLabelColorGallery : ContentPage
	{
		public PopupLabelColorGallery()
		{
			Content = new StackLayout
			{
				Children =
				{
					CreatePopupWithBackground(),
					CreatePopupWithBackgroundAndSize()
				}
			};
		}

		private View CreatePopupWithBackgroundAndSize()
		{
			var button = new Button
			{
				Text = "Popup with Background and Size"
			};
			button.Clicked += async (s, e) =>
			{
				View label = new Label
				{
					Text = "This is just a Label inside of a Popup. This is a basic popup which takes a View, and is only light-dismissable.",
					LineBreakMode = LineBreakMode.WordWrap,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					VerticalOptions = LayoutOptions.CenterAndExpand
				};

				var popup = new Popup(label)
				{
					Size = new Size(600, 600),
					Color = Color.Green
				};

				var result = await Navigation.ShowPopup(popup);
			};

			return button;
		}

		private Button CreatePopupWithBackground()
		{
			var button = new Button
			{
				Text = "Popup with Green Background"
			};
			button.Clicked += async (s, e) =>
			{
				View label = new Label
				{
					Text = "This is just a Label inside of a Popup. This is a basic popup which takes a View, and is only light-dismissable.",
					LineBreakMode = LineBreakMode.WordWrap,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center
				};

				var popup = new Popup(label)
				{
					Color = Color.Green
				};
				var result = await Navigation.ShowPopup(popup);
			};

			return button;
		}
	}
}
