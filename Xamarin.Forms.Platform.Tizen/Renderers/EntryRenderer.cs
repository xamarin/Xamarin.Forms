using System;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;

using EColor = ElmSharp.Color;
using Specific = Xamarin.Forms.PlatformConfiguration.TizenSpecific.Entry;

namespace Xamarin.Forms.Platform.Tizen
{
	public class EntryRenderer : ViewRenderer<Entry, Native.Entry>, IDisposable
	{
		static readonly EColor s_defaultTextColor = EColor.Black;

		static readonly EColor s_defaultPlaceholderColor = EColor.Gray;

		public EntryRenderer()
		{
			RegisterPropertyHandler(Entry.IsPasswordProperty, UpdateIsPassword);
			RegisterPropertyHandler(Entry.TextProperty, UpdateText);
			RegisterPropertyHandler(Entry.TextColorProperty, UpdateTextColor);
			RegisterPropertyHandler(Entry.FontSizeProperty, UpdateFontSize);
			RegisterPropertyHandler(Entry.FontFamilyProperty, UpdateFontFamily);
			RegisterPropertyHandler(Entry.FontAttributesProperty, UpdateFontAttributes);
			RegisterPropertyHandler(Entry.HorizontalTextAlignmentProperty, UpdateHorizontalTextAlignment);
			RegisterPropertyHandler(Entry.KeyboardProperty, UpdateKeyboard);
			RegisterPropertyHandler(Entry.PlaceholderProperty, UpdatePlaceholder);
			RegisterPropertyHandler(Entry.PlaceholderColorProperty, UpdatePlaceholderColor);
			RegisterPropertyHandler(Specific.FontWeightProperty, UpdateFontWeight);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			if (Control == null)
			{
				var entry = new Native.Entry(Forms.Context.MainWindow)
				{
					IsSingleLine = true,
					PropagateEvents = false,
				};
				SetNativeControl(entry);
			}

			if (e.OldElement != null)
			{
				Control.TextChanged -= EntryChangedHandler;
				Control.Activated -= EntryCompletedHandler;
			}

			if (e.NewElement != null)
			{
				Control.TextChanged += EntryChangedHandler;
				Control.Activated += EntryCompletedHandler;
			}

			base.OnElementChanged(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (null != Control)
			{
				Control.TextChanged -= EntryChangedHandler;
				Control.Activated -= EntryCompletedHandler;
			}

			base.Dispose(disposing);
		}

		void EntryChangedHandler(object sender, EventArgs e)
		{
			Element.Text = Control.Text;
		}

		void EntryCompletedHandler(object sender, EventArgs e)
		{
			//TODO Consider if any other object should overtake focus
			Control.SetFocus(false);

			((IEntryController)Element).SendCompleted();
		}

		void UpdateIsPassword()
		{
			Control.IsPassword = Element.IsPassword;
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

		void UpdateHorizontalTextAlignment()
		{
			Control.HorizontalTextAlignment = Element.HorizontalTextAlignment.ToNative();
		}

		void UpdateKeyboard()
		{
			Control.Keyboard = Element.Keyboard.ToNative();
		}

		void UpdatePlaceholder()
		{
			Control.Placeholder = Element.Placeholder;
		}

		void UpdatePlaceholderColor()
		{
			Control.PlaceholderColor = Element.PlaceholderColor.IsDefault ? s_defaultPlaceholderColor : Element.PlaceholderColor.ToNative();
		}

		void UpdateFontWeight()
		{
			Control.FontWeight = Specific.GetFontWeight(Element);
		}
	}
}
