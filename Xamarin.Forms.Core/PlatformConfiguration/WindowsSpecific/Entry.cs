namespace Xamarin.Forms.PlatformConfiguration.WindowsSpecific
{
	// TODO hartez 5:38:45 PM Clean this up I don't think we need it for this platform (just copy/paste it to the other two)
	using FormsElement = Forms.Entry;

	public static class Entry
	{
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

		public static bool GetIsLegacyColorModeEnabled(this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			return (bool)config.Element.GetValue(IsLegacyColorModeEnabledProperty);
		}

		public static IPlatformElementConfiguration<Windows, FormsElement> SetIsLegacyColorModeEnabled(
			this IPlatformElementConfiguration<Windows, FormsElement> config, bool value)
		{
			config.Element.SetValue(IsLegacyColorModeEnabledProperty, value);
			return config;
		}

		#endregion
	}
}
