namespace Xamarin.Forms.Controls.GalleryPages.VisualStateManagerGalleries
{
	public class VisualStateManagerGallery : ContentPage
	{
		public VisualStateManagerGallery()
		{
			var entryDisabledStatesButton = new Button { Text = "Entry Disabled States" };
			entryDisabledStatesButton.Clicked += (sender, args) => { Navigation.PushAsync (new EntryDisabledStatesGallery()); };

			var buttonDisabledStatesButton = new Button { Text = "Button Disabled States" };
			buttonDisabledStatesButton.Clicked += (sender, args) => { Navigation.PushAsync(new ButtonDisabledStatesGallery()); };


			var entryFocusStatesButton = new Button { Text = "Entry Focus States" };
			entryFocusStatesButton.Clicked += (sender, args) => { Navigation.PushAsync(new EntryFocusStatesGallery()); };

			var onPlatformExampleButton = new Button { Text = "OnPlatform Example" };
			onPlatformExampleButton.Clicked += (sender, args) => { Navigation.PushAsync(new OnPlatformExample()); };

			var onIdiomExampleButton = new Button { Text = "OnIdiom Example" };
			onIdiomExampleButton.Clicked += (sender, args) => { Navigation.PushAsync(new OnIdiomExample()); };

			Content = new StackLayout
			{
				Children =
				{
					entryDisabledStatesButton,
					buttonDisabledStatesButton,
					entryFocusStatesButton, 
					onPlatformExampleButton, 
					onIdiomExampleButton
				}
			};
		}
	}
}
