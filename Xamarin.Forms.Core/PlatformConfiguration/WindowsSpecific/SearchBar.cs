using System;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.PlatformConfiguration.WindowsSpecific
{
	using FormsElement = Forms.SearchBar;

	public static class SearchBar
	{
		public static readonly BindableProperty SuggestionsProperty =
			BindableProperty.CreateAttached("Suggestions", typeof(ObservableCollection<string>),
				typeof(FormsElement), new ObservableCollection<string>());

		public static ObservableCollection<string> GetSuggestions(BindableObject element)
		{
			return (ObservableCollection<string>)element.GetValue(SuggestionsProperty);
		}

		public static void SetSuggestions(BindableObject element, ObservableCollection<string> collection)
		{
			element.SetValue(SuggestionsProperty, collection);
		}

		public static ObservableCollection<string> Suggestions(this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			return (ObservableCollection<string>)config.Element.GetValue(SuggestionsProperty);
		}

		public static readonly BindableProperty TextChangedActionProperty =
			BindableProperty.CreateAttached("TextChangedAction", typeof(Action),
				typeof(FormsElement), null);

		public static Action GetTextChangedAction(BindableObject element)
		{
			return (Action)element.GetValue(TextChangedActionProperty);
		}

		public static void SetTextChangedAction(BindableObject element, Action value)
		{
			element.SetValue(TextChangedActionProperty, value);
		}

		public static Action GetTextChangedAction(this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			return (Action)config.Element.GetValue(TextChangedActionProperty);
		}

		public static IPlatformElementConfiguration<Windows, FormsElement> SetTextChangedAction(
			this IPlatformElementConfiguration<Windows, FormsElement> config, Action value)
		{
			config.Element.SetValue(TextChangedActionProperty, value);
			return config;
		}

		public static readonly BindableProperty AutoMaximizeSuggestionAreaProperty =
			BindableProperty.CreateAttached("AutoMaximizeSuggestionArea", typeof(bool),
				typeof(FormsElement), false);

		public static bool GetAutoMaximizeSuggestionArea(BindableObject element)
		{
			return (bool)element.GetValue(AutoMaximizeSuggestionAreaProperty);
		}

		public static void SetAutoMaximizeSuggestionArea(BindableObject element, bool value)
		{
			element.SetValue(AutoMaximizeSuggestionAreaProperty, value);
		}

		public static bool AutoMaximizeSuggestionArea(this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			return (bool)config.Element.GetValue(AutoMaximizeSuggestionAreaProperty);
		}

		public static IPlatformElementConfiguration<Windows, FormsElement> SetAutoMaximizeSuggestionArea(
			this IPlatformElementConfiguration<Windows, FormsElement> config, bool value)
		{
			config.Element.SetValue(AutoMaximizeSuggestionAreaProperty, value);
			return config;
		}
	}
}
