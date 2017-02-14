using System;
using EColor = ElmSharp.Color;

namespace Xamarin.Forms.Platform.Tizen
{
	public class EditorRenderer : ViewRenderer<Editor, Native.Entry>
	{
		static readonly EColor s_defaultTextColor = EColor.Black;

		public EditorRenderer()
		{
			RegisterPropertyHandler(Editor.TextProperty, UpdateText);
			RegisterPropertyHandler(Editor.TextColorProperty, UpdateTextColor);
			RegisterPropertyHandler(Editor.FontSizeProperty, UpdateFontSize);
			RegisterPropertyHandler(Editor.FontFamilyProperty, UpdateFontFamily);
			RegisterPropertyHandler(Editor.FontAttributesProperty, UpdateFontAttributes);
			RegisterPropertyHandler(Editor.KeyboardProperty, UpdateKeyboard);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			if (Control == null)
			{
				var entry = new Native.Entry(Forms.Context.MainWindow)
				{
					IsSingleLine = false,
					PropagateEvents = false,
				};
				SetNativeControl(entry);
			}

			if (e.OldElement != null)
			{
				Control.TextChanged -= TextChanged;
				Control.Unfocused -= Completed;
			}

			if (e.NewElement != null)
			{
				Control.TextChanged += TextChanged;
				Control.Unfocused += Completed;
			}

			base.OnElementChanged(e);
		}

		void TextChanged(object sender, EventArgs e)
		{
			Element.Text = ((Native.Entry)sender).Text;
		}

		void Completed(object sender, EventArgs e)
		{
			Element.SendCompleted();
		}

		void UpdateText()
		{
			Control.Text = Element.Text;
		}

		void UpdateTextColor()
		{
			Control.TextColor = Element.TextColor.IsDefault ? s_defaultTextColor : Element.TextColor.ToNative();
		}

		void UpdateFontSize()
		{
			Control.FontSize = Element.FontSize;
		}

		void UpdateFontFamily()
		{
			Control.FontFamily = Element.FontFamily;
		}

		void UpdateFontAttributes()
		{
			Control.FontAttributes = Element.FontAttributes;
		}

		void UpdateKeyboard()
		{
			Control.Keyboard = Element.Keyboard.ToNative();
		}
	}
}

