using System;
using EColor = ElmSharp.Color;
using ESize = ElmSharp.Size;
using Specific = Xamarin.Forms.PlatformConfiguration.TizenSpecific.VisualElement;

namespace Xamarin.Forms.Platform.Tizen
{
	public class ButtonRenderer : ViewRenderer<Button, Native.Button>
	{
		public ButtonRenderer()
		{
			RegisterPropertyHandler(Button.TextProperty, UpdateText);
			RegisterPropertyHandler(Button.FontFamilyProperty, UpdateText);
			RegisterPropertyHandler(Button.FontSizeProperty, UpdateText);
			RegisterPropertyHandler(Button.FontAttributesProperty, UpdateText);
			RegisterPropertyHandler(Button.TextColorProperty, UpdateTextColor);
			RegisterPropertyHandler(Button.ImageProperty, UpdateBitmap);
			RegisterPropertyHandler(Button.BorderColorProperty, UpdateBorder);
			RegisterPropertyHandler(Button.BorderRadiusProperty, UpdateBorder);
			RegisterPropertyHandler(Button.BorderWidthProperty, UpdateBorder);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			if (Control == null)
			{
				var button = new Native.Button(Forms.Context.MainWindow)
				{
					PropagateEvents = false,
				};
				SetNativeControl(button);
			}

			if (e.OldElement != null)
			{
				Control.Clicked -= ButtonClickedHandler;
			}

			if (e.NewElement != null)
			{
				Control.Clicked += ButtonClickedHandler;
			}
			base.OnElementChanged(e);
		}

		protected override Size MinimumSize()
		{
			return new ESize(Control.MinimumWidth, Control.MinimumHeight).ToDP();
		}

		void ButtonClickedHandler(object sender, EventArgs e)
		{
			IButtonController btn = Element as IButtonController;
			if (btn != null)
			{
				btn.SendClicked();
			}
		}

		void UpdateText()
		{
			Control.Text = Element.Text ?? "";
			Control.FontSize = Element.FontSize;
			Control.FontAttributes = Element.FontAttributes;
			Control.FontFamily = Element.FontFamily;
		}

		void UpdateTextColor()
		{
			Control.TextColor = Element.TextColor.ToNative();
		}

		void UpdateBitmap()
		{
			if (!string.IsNullOrEmpty(Element.Image))
			{
				Control.Image = new Native.Image(Control);
				var task = Control.Image.LoadFromImageSourceAsync(Element.Image);
			}
			else
			{
				Control.Image = null;
			}
		}

		protected override void UpdateThemeStyle()
		{
			Control.UpdateStyle(Specific.GetStyle(Element));
			((IVisualElementController)Element).NativeSizeChanged();
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
