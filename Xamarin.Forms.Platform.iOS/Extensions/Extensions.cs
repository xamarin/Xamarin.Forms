using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public static class Extensions
	{
		public static void ApplyKeyboard(this IUITextInput textInput, Keyboard keyboard)
		{
			textInput.AutocapitalizationType = UITextAutocapitalizationType.None;
			textInput.AutocorrectionType = UITextAutocorrectionType.No;
			textInput.SpellCheckingType = UITextSpellCheckingType.No;

			if (keyboard == Keyboard.Default)
			{
				textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
				textInput.AutocorrectionType = UITextAutocorrectionType.Default;
				textInput.SpellCheckingType = UITextSpellCheckingType.Default;
				textInput.KeyboardType = UIKeyboardType.Default;
			}
			else if (keyboard == Keyboard.Chat)
			{
				textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
				textInput.AutocorrectionType = UITextAutocorrectionType.Yes;
			}
			else if (keyboard == Keyboard.Email)
				textInput.KeyboardType = UIKeyboardType.EmailAddress;
			else if (keyboard == Keyboard.Numeric)
				textInput.KeyboardType = UIKeyboardType.DecimalPad;
			else if (keyboard == Keyboard.Telephone)
				textInput.KeyboardType = UIKeyboardType.PhonePad;
			else if (keyboard == Keyboard.Text)
			{
				textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
				textInput.AutocorrectionType = UITextAutocorrectionType.Yes;
				textInput.SpellCheckingType = UITextSpellCheckingType.Yes;
			}
			else if (keyboard == Keyboard.Url)
				textInput.KeyboardType = UIKeyboardType.Url;
			else if (keyboard is CustomKeyboard)
			{
				var custom = (CustomKeyboard)keyboard;
				var capitalizedSentenceEnabled = (custom.Flags & KeyboardFlags.CapitalizeSentence) == KeyboardFlags.CapitalizeSentence;
				var spellcheckEnabled = (custom.Flags & KeyboardFlags.Spellcheck) == KeyboardFlags.Spellcheck;
				var suggestionsEnabled = (custom.Flags & KeyboardFlags.Suggestions) == KeyboardFlags.Suggestions;

				textInput.AutocapitalizationType = capitalizedSentenceEnabled ? UITextAutocapitalizationType.Sentences : UITextAutocapitalizationType.None;
				textInput.AutocorrectionType = suggestionsEnabled ? UITextAutocorrectionType.Yes : UITextAutocorrectionType.No;
				textInput.SpellCheckingType = spellcheckEnabled ? UITextSpellCheckingType.Yes : UITextSpellCheckingType.No;
			}
		}

		internal static DeviceOrientation ToDevicenOrientation(this UIDeviceOrientation orientation)
		{
			switch (orientation)
			{
				case UIDeviceOrientation.Portrait:
				case UIDeviceOrientation.PortraitUpsideDown:
					return DeviceOrientation.Portrait;
				case UIDeviceOrientation.LandscapeLeft:
				case UIDeviceOrientation.LandscapeRight:
					return DeviceOrientation.Landscape;
				default:
					return DeviceOrientation.Unknown;
			}
		}

		internal static ScreenOrientation ToScreenOrientation(this UIInterfaceOrientation orientation)
		{
			switch (orientation)
			{
				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
					return ScreenOrientation.Portrait;
				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
					return ScreenOrientation.Landscape;
				default:
					return ScreenOrientation.Other;
			}
		}
	}
}