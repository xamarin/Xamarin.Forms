using System;

namespace Xamarin.Forms.Controls.GalleryPages.VisualStateManagerGalleries
{
	public class VisualStateManagerGallery : ContentPage
	{
		static Button GalleryNav(string galleryName, Func<ContentPage> gallery, INavigation nav)
		{
			var button = new Button { Text = $"{galleryName}" };
			button.Clicked += (sender, args) => { nav.PushAsync(gallery()); };
			return button;
		}

		public VisualStateManagerGallery()
		{
			Content = new StackLayout
			{
				Children =
				{
					GalleryNav("Disabled States Gallery", () => new DisabledStatesGallery(), Navigation),
					GalleryNav("Entry Focus States", () => new EntryFocusStatesGallery(), Navigation),
					GalleryNav("OnPlatform Example", () => new OnPlatformExample(), Navigation),
					GalleryNav("OnIdiom Example", () => new OnIdiomExample(), Navigation),
				}
			};
		}
	}

	public class DisabledStatesGallery : ContentPage
	{
		static Button GalleryNav(string control, Func<ContentPage> gallery, INavigation nav)
		{
			var button = new Button { Text = $"{control} Disabled States" };
			button.Clicked += (sender, args) => { nav.PushAsync(gallery()); };
			return button;
		}

		public DisabledStatesGallery()
		{
			var desc = "Some of the XF controls have legacy behaviors such that when IsEnabled is set to `false`, they " 
						+ "will override the colors set by the user with the default native colors for the 'disabled' " 
						+ "state. For backward compatibility, this remains the default behavior for those controls. " 
						+ "\n\nUsing the VSM with these controls overrides that behavior; it is also possible to override " 
						+ "that behavior with the `IsLegacyColorModeEnabled` platform specific, which returns the " 
						+ "controls to their old (pre-2.0) behavior (i.e., colors set on the control remain even when " 
						+ "the control is 'disabled'). \n\nThe galleries below demonstrate each behavior.";

			var descriptionLabel = new Label { Text = desc, Margin = new Thickness(2,2,2,2)};

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children =
					{
						descriptionLabel,
						GalleryNav("Entry", () => new EntryDisabledStatesGallery(), Navigation),
						GalleryNav("Button", () => new ButtonDisabledStatesGallery(), Navigation),
						GalleryNav("Picker", () => new PickerDisabledStatesGallery(), Navigation),
						GalleryNav("TimePicker", () => new TimePickerDisabledStatesGallery(), Navigation),
						GalleryNav("DatePicker", () => new DatePickerDisabledStatesGallery(), Navigation)
					}
				}
			};
		}
	}
}
