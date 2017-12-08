using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.VisualStateManagerGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PickerDisabledStatesGallery : ContentPage
	{
		public PickerDisabledStatesGallery ()
		{
			InitializeComponent ();
		}

		void Button0_OnClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			ToggleIsEnabled(Picker0, button);
		}

		void Button1_OnClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			ToggleIsEnabled(Picker1, button);
		}

		void Button2_OnClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			ToggleIsEnabled(Picker2, button);
		}

		void Button3_OnClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			ToggleIsEnabled(Picker3, button);
		}

		void ToggleIsEnabled(Picker picker, Button toggleButton)
		{
			picker.IsEnabled = !picker.IsEnabled;

			if (toggleButton != null)
			{
				toggleButton.Text = $"Toggle IsEnabled (Currently {picker.IsEnabled})";
			}
		}
	}
}