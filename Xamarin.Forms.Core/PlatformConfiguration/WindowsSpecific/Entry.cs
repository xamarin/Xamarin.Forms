using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Core.PlatformConfiguration.WindowsSpecific
{
	using FormsElement = Forms.Entry;

	public static class Entry
	{
		#region DisableLegacyColorMode

		public static readonly BindableProperty DisableLegacyColorModeProperty =
			BindableProperty.CreateAttached("DisableLegacyColorMode", typeof(bool),
				typeof(Entry), false);

		public static bool GetSelectionMode(BindableObject element)
		{
			return (bool)element.GetValue(DisableLegacyColorModeProperty);
		}

		public static void SetSelectionMode(BindableObject element, bool value)
		{
			element.SetValue(DisableLegacyColorModeProperty, value);
		}

		public static bool GetSelectionMode(this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			return (bool)config.Element.GetValue(DisableLegacyColorModeProperty);
		}

		public static IPlatformElementConfiguration<Windows, FormsElement> SetSelectionMode(
			this IPlatformElementConfiguration<Windows, FormsElement> config, bool value)
		{
			config.Element.SetValue(DisableLegacyColorModeProperty, value);
			return config;
		}

		#endregion
	}
}
