using System;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.Entry;

	public static class Entry
	{
		public static readonly BindableProperty AdjustsFontSizeToFitWidthProperty =
			BindableProperty.Create("AdjustsFontSizeToFitWidth", typeof(bool),
			typeof(Entry), false);

		public static bool GetAdjustsFontSizeToFitWidth(BindableObject element)
		{
			return (bool)element.GetValue(AdjustsFontSizeToFitWidthProperty);
		}

		public static void SetAdjustsFontSizeToFitWidth(BindableObject element, bool value)
		{
			element.SetValue(AdjustsFontSizeToFitWidthProperty, value);
		}

		public static bool AdjustsFontSizeToFitWidth(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetAdjustsFontSizeToFitWidth(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetAdjustsFontSizeToFitWidth(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetAdjustsFontSizeToFitWidth(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> EnableAdjustsFontSizeToFitWidth(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetAdjustsFontSizeToFitWidth(config.Element, true);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> DisableAdjustsFontSizeToFitWidth(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetAdjustsFontSizeToFitWidth(config.Element, false);
			return config;
		}

		#region IsLegacyColorModeEnabled

		public static readonly BindableProperty IsLegacyColorModeEnabledProperty =
			BindableProperty.CreateAttached("IsLegacyColorModeEnabled", typeof(bool),
				typeof(FormsElement), true);

		public static bool GetIsLegacyColorModeEnabled(BindableObject element)
		{
			return (bool)element.GetValue(IsLegacyColorModeEnabledProperty);
		}

		public static void SetIsLegacyColorModeEnabled(BindableObject element, bool value)
		{
			element.SetValue(IsLegacyColorModeEnabledProperty, value);
		}

		public static bool GetIsLegacyColorModeEnabled(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return (bool)config.Element.GetValue(IsLegacyColorModeEnabledProperty);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetIsLegacyColorModeEnabled(
			this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			config.Element.SetValue(IsLegacyColorModeEnabledProperty, value);
			return config;
		}

		#endregion
	}
}
