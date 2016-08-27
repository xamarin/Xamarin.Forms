using Android.Text;

namespace Xamarin.Forms.Platform.Android
{
	public static class KeyboardExtensions
	{
		public static InputTypes ToInputType(this Keyboard self)
		{
			var result = new InputTypes();

			// ClassText:																						!autocaps, spellcheck, suggestions 
			// TextFlagNoSuggestions:																			!autocaps, !spellcheck, !suggestions
			// InputTypes.ClassText | InputTypes.TextFlagCapSentences											 autocaps,	spellcheck,  suggestions
			// InputTypes.ClassText | InputTypes.TextFlagCapSentences | InputTypes.TextFlagNoSuggestions;		 autocaps, !spellcheck, !suggestions

			if (self == Keyboard.Default)
				result = InputTypes.ClassText | InputTypes.TextVariationNormal;
			else if (self == Keyboard.Chat)
				result = InputTypes.ClassText | InputTypes.TextFlagCapSentences | InputTypes.TextFlagNoSuggestions;
			else if (self == Keyboard.Email)
				result = InputTypes.ClassText | InputTypes.TextVariationEmailAddress;
			else if (self == Keyboard.Numeric)
				result = InputTypes.ClassNumber | InputTypes.NumberFlagDecimal;
			else if (self == Keyboard.Telephone)
				result = InputTypes.ClassPhone;
			else if (self == Keyboard.Text)
				result = InputTypes.ClassText | InputTypes.TextFlagCapSentences;
			else if (self == Keyboard.Url)
				result = InputTypes.ClassText | InputTypes.TextVariationUri;
			else if (self is CustomKeyboard)
			{
				var custom = (CustomKeyboard)self;
				bool capitalizedSentenceEnabled = (custom.Flags & KeyboardFlags.CapitalizeSentence) == KeyboardFlags.CapitalizeSentence;
				bool spellcheckEnabled = (custom.Flags & KeyboardFlags.Spellcheck) == KeyboardFlags.Spellcheck;
				bool suggestionsEnabled = (custom.Flags & KeyboardFlags.Suggestions) == KeyboardFlags.Suggestions;

				if (!capitalizedSentenceEnabled && !spellcheckEnabled && !suggestionsEnabled)
					result = InputTypes.ClassText | InputTypes.TextFlagNoSuggestions;

				if (!capitalizedSentenceEnabled && !spellcheckEnabled && suggestionsEnabled)
				{
					// Due to the nature of android, TextFlagAutoCorrect includes Spellcheck
					Log.Warning(null, "On Android, KeyboardFlags.Suggestions enables KeyboardFlags.Spellcheck as well due to a platform limitation.");
					result = InputTypes.ClassText | InputTypes.TextFlagAutoCorrect;
				}

				if (!capitalizedSentenceEnabled && spellcheckEnabled && !suggestionsEnabled)
					result = InputTypes.ClassText | InputTypes.TextFlagAutoComplete;

				if (!capitalizedSentenceEnabled && spellcheckEnabled && suggestionsEnabled)
					result = InputTypes.ClassText | InputTypes.TextFlagAutoCorrect;

				if (capitalizedSentenceEnabled && !spellcheckEnabled && !suggestionsEnabled)
					result = InputTypes.ClassText | InputTypes.TextFlagCapSentences | InputTypes.TextFlagNoSuggestions;

				if (capitalizedSentenceEnabled && !spellcheckEnabled && suggestionsEnabled)
				{
					// Due to the nature of android, TextFlagAutoCorrect includes Spellcheck
					Log.Warning(null, "On Android, KeyboardFlags.Suggestions enables KeyboardFlags.Spellcheck as well due to a platform limitation.");
					result = InputTypes.ClassText | InputTypes.TextFlagCapSentences | InputTypes.TextFlagAutoCorrect;
				}

				if (capitalizedSentenceEnabled && spellcheckEnabled && !suggestionsEnabled)
					result = InputTypes.ClassText | InputTypes.TextFlagCapSentences | InputTypes.TextFlagAutoComplete;

				if (capitalizedSentenceEnabled && spellcheckEnabled && suggestionsEnabled)
					result = InputTypes.ClassText | InputTypes.TextFlagCapSentences | InputTypes.TextFlagAutoCorrect;
			}
			else
			{
				// Should never happens
				result = InputTypes.TextVariationNormal;
			}
			return result;
		}
	}
}