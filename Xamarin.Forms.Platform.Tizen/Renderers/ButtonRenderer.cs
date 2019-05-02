using System;
using Specific = Xamarin.Forms.PlatformConfiguration.TizenSpecific.VisualElement;

namespace Xamarin.Forms.Platform.Tizen
{
	public class ButtonRenderer : ViewRenderer<Button, Native.Button>
	{
		public ButtonRenderer()
		{
			RegisterPropertyHandler(Button.TextProperty, UpdateText);
			RegisterPropertyHandler(Button.FontFamilyProperty, UpdateFontFamily);
			RegisterPropertyHandler(Button.FontSizeProperty, UpdateFontSize);
			RegisterPropertyHandler(Button.FontAttributesProperty, UpdateFontAttributes);
			RegisterPropertyHandler(Button.TextColorProperty, UpdateTextColor);
			RegisterPropertyHandler(Button.ImageSourceProperty, UpdateBitmap);
			RegisterPropertyHandler(Button.BorderColorProperty, UpdateBorder);
			RegisterPropertyHandler(Button.CornerRadiusProperty, UpdateBorder);
			RegisterPropertyHandler(Button.BorderWidthProperty, UpdateBorder);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			if (Control == null)
			{
				if (Device.Idiom == TargetIdiom.Watch)
					SetNativeControl(new Native.Watch.WatchButton(Forms.NativeParent));
				else
					SetNativeControl(new Native.Button(Forms.NativeParent));

				Control.Clicked += OnButtonClicked;
				Control.Pressed += OnButtonPressed;
				Control.Released += OnButtonReleased;
			}
			base.OnElementChanged(e);
		}

		protected override Size MinimumSize()
		{
			return Control.Measure(Control.MinimumWidth, Control.MinimumHeight).ToDP();
		}

		protected override void UpdateThemeStyle()
		{
			var style = Specific.GetStyle(Element);
			if (!string.IsNullOrEmpty(style))
			{
				Control.UpdateStyle(style);
				((IVisualElementController)Element).NativeSizeChanged();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Control != null)
				{
					Control.Clicked -= OnButtonClicked;
					Control.Pressed -= OnButtonPressed;
					Control.Released -= OnButtonReleased;
				}
			}
			base.Dispose(disposing);
		}

		void OnButtonClicked(object sender, EventArgs e)
		{
			(Element as IButtonController)?.SendClicked();
		}

		void OnButtonPressed(object sender, EventArgs e)
		{
			(Element as IButtonController)?.SendPressed();
		}

		void OnButtonReleased(object sender, EventArgs e)
		{
			(Element as IButtonController)?.SendReleased();
		}

		void UpdateText()
		{
			Control.Text = Element.Text ?? "";
		}

		void UpdateFontSize()
		{
			Control.FontSize = Element.FontSize;
		}

		void UpdateFontAttributes()
		{
			Control.FontAttributes = Element.FontAttributes;
		}

		void UpdateFontFamily()
		{
			Control.FontFamily = Element.FontFamily;
		}

		void UpdateTextColor()
		{
			Control.TextColor = Element.TextColor.ToNative();
		}

		void UpdateBitmap()
		{
			if (!Element.ImageSource.IsNullOrEmpty())
			{
				Control.Image = new Native.Image(Control);
				_ = Control.Image.LoadFromImageSourceAsync(Element.ImageSource);
			}
			else
			{
				Control.Image = null;
			}
		}

		void UpdateBorder()
		{
			/* The simpler way is to create some specialized theme for button in
			 * tizen-theme
			 */
			// TODO: implement border handling
		}
	}
}
