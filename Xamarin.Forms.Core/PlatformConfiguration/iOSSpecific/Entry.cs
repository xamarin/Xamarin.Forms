using System;
using System.Collections.Generic;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.Entry;

	public static class Entry
	{
		public static readonly BindableProperty KeyboardToolbarProperty = BindableProperty.Create("KeyboardToolbar", typeof(List<Tuple<UIBarButtonSystemItem, Action>>), typeof(Entry));

		public static List<Tuple<UIBarButtonSystemItem, Action>> GetKeyboardToolbar(BindableObject element)
		{
			return (List<Tuple<UIBarButtonSystemItem, Action>>)element.GetValue(KeyboardToolbarProperty);
		}

		public static void SetKeyboardToolbar(BindableObject element, List<Tuple<UIBarButtonSystemItem, Action>> value)
		{
			element.SetValue(KeyboardToolbarProperty, value);
		}

		public static List<Tuple<UIBarButtonSystemItem, Action>> KeyboardToolbar(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetKeyboardToolbar(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetKeyboardToolbar(this IPlatformElementConfiguration<iOS, FormsElement> config, List<Tuple<UIBarButtonSystemItem, Action>> value)
		{
			SetKeyboardToolbar(config.Element, value);
			return config;
		}
	}
}