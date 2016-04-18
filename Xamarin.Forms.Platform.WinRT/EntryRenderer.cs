﻿using System.ComponentModel;
using Windows.System;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public class EntryRenderer : ViewRenderer<Entry, FormsTextBox>
	{
		Brush _backgroundColorFocusedDefaultBrush;

		bool _fontApplied;
		Brush _placeholderDefaultBrush;

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var textBox = new FormsTextBox { Style = Windows.UI.Xaml.Application.Current.Resources["FormsTextBoxStyle"] as Windows.UI.Xaml.Style };

					textBox.TextChanged += OnNativeTextChanged;
					textBox.KeyUp += TextBoxOnKeyUp;
					SetNativeControl(textBox);
				}

				UpdateIsPassword();
				UpdateText();
				UpdatePlaceholder();
				UpdateTextColor();
				UpdateFont();
				UpdateInputScope();
				UpdateAlignment();
				UpdatePlaceholderColor();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Entry.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
				UpdateIsPassword();
			else if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
				UpdatePlaceholder();
			else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
				UpdateInputScope();
			else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
				UpdateAlignment();
			else if (e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholderColor();
		}

		protected override void UpdateBackgroundColor()
		{
			base.UpdateBackgroundColor();

			if (Control == null)
			{
				return;
			}

			// By default some platforms have alternate default background colors when focused
			Color backgroundColor = Element.BackgroundColor;
			if (backgroundColor.IsDefault)
			{
				if (_backgroundColorFocusedDefaultBrush == null)
				{
					return;
				}

				Control.BackgroundFocusBrush = _backgroundColorFocusedDefaultBrush;
				return;
			}

			if (_backgroundColorFocusedDefaultBrush == null)
			{
				_backgroundColorFocusedDefaultBrush = Control.BackgroundFocusBrush;
			}

			Control.BackgroundFocusBrush = backgroundColor.ToBrush();
		}

		void OnNativeTextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs args)
		{
			Element?.SetValueCore(Entry.TextProperty, Control.Text);
		}

		void TextBoxOnKeyUp(object sender, KeyRoutedEventArgs args)
		{
			if (args.Key != VirtualKey.Enter)
				return;

			Element?.SendCompleted();
		}

		void UpdateAlignment()
		{
			Control.TextAlignment = Element.HorizontalTextAlignment.ToNativeTextAlignment();
		}

		void UpdateFont()
		{
			if (Control == null)
				return;

			Entry entry = Element;

			if (entry == null)
				return;

			bool entryIsDefault = entry.FontFamily == null && entry.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Entry), true) && entry.FontAttributes == FontAttributes.None;

			if (entryIsDefault && !_fontApplied)
				return;

			if (entryIsDefault)
			{
				// ReSharper disable AccessToStaticMemberViaDerivedType
				// Resharper wants to simplify 'FormsTextBox' to 'Control', but then it'll conflict with the property 'Control'
				Control.ClearValue(FormsTextBox.FontStyleProperty);
				Control.ClearValue(FormsTextBox.FontSizeProperty);
				Control.ClearValue(FormsTextBox.FontFamilyProperty);
				Control.ClearValue(FormsTextBox.FontWeightProperty);
				Control.ClearValue(FormsTextBox.FontStretchProperty);
				// ReSharper restore AccessToStaticMemberViaDerivedType
			}
			else
			{
				Control.ApplyFont(entry);
			}

			_fontApplied = true;
		}

		void UpdateInputScope()
		{
			var custom = Element.Keyboard as CustomKeyboard;
			if (custom != null)
			{
				Control.IsTextPredictionEnabled = (custom.Flags & KeyboardFlags.Suggestions) != 0;
				Control.IsSpellCheckEnabled = (custom.Flags & KeyboardFlags.Spellcheck) != 0;
			}

			Control.InputScope = Element.Keyboard.ToInputScope();
		}

		void UpdateIsPassword()
		{
			Control.IsPassword = Element.IsPassword;
		}

		void UpdatePlaceholder()
		{
			Control.PlaceholderText = Element.Placeholder ?? "";
		}

		void UpdatePlaceholderColor()
		{
			Color placeholderColor = Element.PlaceholderColor;

			if (placeholderColor.IsDefault)
			{
				if (_placeholderDefaultBrush == null)
				{
					return;
				}

				// Use the cached default brush
				Control.PlaceholderForegroundBrush = _placeholderDefaultBrush;
				return;
			}

			if (_placeholderDefaultBrush == null)
			{
				// Cache the default brush in case we need to set the color back to default
				_placeholderDefaultBrush = Control.PlaceholderForegroundBrush;
			}

			Control.PlaceholderForegroundBrush = placeholderColor.ToBrush();
		}

		void UpdateText()
		{
			Control.Text = Element.Text ?? "";
		}

		void UpdateTextColor()
		{
			Control.Foreground = Element.TextColor.ToBrush();
		}
	}
}