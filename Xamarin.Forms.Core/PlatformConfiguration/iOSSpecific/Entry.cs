using System.Collections.Generic;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.Entry;

	public static class Entry
	{
		public static readonly BindableProperty KeyboardToolbarProperty = BindableProperty.Create("KeyboardToolbar", typeof(List<KeyboardToolbarItem>), typeof(Entry));

		public static List<KeyboardToolbarItem> GetKeyboardToolbar(BindableObject element)
		{
			return (List<KeyboardToolbarItem>)element.GetValue(KeyboardToolbarProperty);
		}

		public static void SetKeyboardToolbar(BindableObject element, List<KeyboardToolbarItem> value)
		{
			element.SetValue(KeyboardToolbarProperty, value);
		}

		public static List<KeyboardToolbarItem> KeyboardToolbar(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetKeyboardToolbar(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetKeyboardToolbar(this IPlatformElementConfiguration<iOS, FormsElement> config, List<KeyboardToolbarItem> value)
		{
			SetKeyboardToolbar(config.Element, value);
			return config;
		}
	}
}