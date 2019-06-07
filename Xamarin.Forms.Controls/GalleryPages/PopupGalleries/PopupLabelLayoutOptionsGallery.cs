namespace Xamarin.Forms.Controls
{
	public class PopupLabelLayoutOptionsGallery : ContentPage
	{
		public PopupLabelLayoutOptionsGallery()
		{
			Content = new StackLayout
			{
				Children =
				{
					CreateLabelButton("Label - Centered", LayoutOptions.CenterAndExpand, LayoutOptions.CenterAndExpand),
					CreateLabelButton("Label - Top Centered", LayoutOptions.CenterAndExpand, LayoutOptions.StartAndExpand),
					CreateLabelButton("Label - Bottom Centered", LayoutOptions.CenterAndExpand, LayoutOptions.EndAndExpand),
					CreateLabelButton("Label - Right Centered", LayoutOptions.EndAndExpand, LayoutOptions.CenterAndExpand),
					CreateLabelButton("Label - Left Centered", LayoutOptions.StartAndExpand, LayoutOptions.CenterAndExpand),
					CreateLabelButton("Label - Top Left", LayoutOptions.StartAndExpand, LayoutOptions.StartAndExpand),
					CreateLabelButton("Label - Top Right", LayoutOptions.EndAndExpand, LayoutOptions.StartAndExpand),
					CreateLabelButton("Label - Bottom Left", LayoutOptions.StartAndExpand, LayoutOptions.EndAndExpand),
					CreateLabelButton("Label - Bottom Right", LayoutOptions.EndAndExpand, LayoutOptions.EndAndExpand),
				}
			};
		}

		private Button CreateLabelButton(string label, LayoutOptions horizontalAlignment, LayoutOptions verticalAlignment)
		{
			var button = new Button
			{
				Text = label,
			};
			button.Clicked += async (s, e) =>
			{
				var popup = new Popup(BuildLabel(horizontalAlignment, verticalAlignment))
				{
					Size = new Size(700, 700)
				};
				var result = await Navigation.ShowPopup(popup);
			};

			return button;
		}

		private View BuildLabel(LayoutOptions horizontalAlignment, LayoutOptions verticalAlignment)
		{
			return new Label
			{
				Text = "This is just a Label inside of a Popup. This is a basic popup which takes a View, and is only light-dismissable.",
				LineBreakMode = LineBreakMode.WordWrap,
				WidthRequest = 100,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalOptions = horizontalAlignment,
				VerticalOptions = verticalAlignment
			};
		}
	}
}
