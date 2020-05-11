using System;
using System.Linq;

namespace Xamarin.Forms.Controls.GalleryPages.ShadowGalleries
{
	public partial class ShadowsExplorerGallery : ContentPage
	{
		const uint AnimationSpeed = 200;

		Color _color;
		double _x;
		double _y;
		float _radius;
		float _opacity;
		Layout _layout;

		public ShadowsExplorerGallery()
		{
			InitializeComponent();
			InitializeShadow();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			ShadowColorPicker.ColorSelected += ShadowColorPickerColorSelected;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			ShadowColorPicker.ColorSelected -= ShadowColorPickerColorSelected;
		}

		void InitializeShadow()
		{
			_color = GetColorFromString(ColorEntry.Text);

			ColorEntry.BackgroundColor = _color;
			ColorFrame.BackgroundColor = _color;

			_x = Convert.ToInt32(XEntry.Text);
			_y = Convert.ToInt32(YEntry.Text);
			_radius = (float)RadiusSlider.Value;
			_opacity = (float)OpacitySlider.Value;

			UpdateShadow();
		}

		void OnColorChanged(object sender, TextChangedEventArgs e)
		{
			var color = GetColorFromString(e.NewTextValue);

			if (color != Color.Default)
			{
				ColorEntry.BackgroundColor = color;
				ColorFrame.BackgroundColor = color;

				_color = color;
				UpdateShadow();
			}
		}

		void ShadowColorPickerColorSelected(object sender, ColorSource e)
		{
			ShadowColorPicker.FadeTo(0, 0, Easing.SinInOut);
			ShadowColorPicker.TranslateTo(0, 1000, 0, Easing.SinInOut);

			var selectedColor = ShadowColorPicker.SelectedColorSource;

			if (selectedColor == null)
				return;

			if (!(_layout.Children.FirstOrDefault() is Entry entry))
				return;

			var red = (int)(selectedColor.Color.R * 255);
			var green = (int)(selectedColor.Color.G * 255);
			var blue = (int)(selectedColor.Color.B * 255);

			entry.Text = $"#{red:X2}{green:X2}{blue:X2}";
		}

		void OnColorPickerTapped(object sender, EventArgs e)
		{
			ShadowColorPicker.FadeTo(1, AnimationSpeed, Easing.SinInOut);
			ShadowColorPicker.TranslateTo(0, 0, AnimationSpeed, Easing.SinInOut);

			if (((Frame)sender).Parent is Layout<View> layout)
				_layout = layout;
		}

		void OnXEntryChanged(object sender, TextChangedEventArgs e)
		{
			int.TryParse(e.NewTextValue, out int result);
			_x = result;
			UpdateShadow();
		}

		void OnYEntryChanged(object sender, TextChangedEventArgs e)
		{
			int.TryParse(e.NewTextValue, out int result);
			_y = result;
			UpdateShadow();
		}

		void OnRadiusChanged(object sender, ValueChangedEventArgs e)
		{
			_radius = (float)e.NewValue;
			UpdateShadow();
		}

		void OnOpacityChanged(object sender, ValueChangedEventArgs e)
		{
			_opacity = (float)e.NewValue;
			UpdateShadow();
		}

		void UpdateShadow()
		{
			var shadow = new DropShadow
			{
				Color = _color,
				Offset = new Point(_x, _y),
				Radius = _radius,
				Opacity = _opacity
			};

			ExplorerView.Shadow = shadow;
		}

		Color GetColorFromString(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Color.Default;

			try
			{
				return Color.FromHex(value[0].Equals('#') ? value : $"#{value}");
			}
			catch (Exception)
			{
				return Color.Default;
			}
		}
	}
}