namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
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

	    public static bool GetIsLegacyColorModeEnabled(this IPlatformElementConfiguration<Android, FormsElement> config)
	    {
		    return (bool)config.Element.GetValue(IsLegacyColorModeEnabledProperty);
	    }

	    public static IPlatformElementConfiguration<Android, FormsElement> SetIsLegacyColorModeEnabled(
		    this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
	    {
		    config.Element.SetValue(IsLegacyColorModeEnabledProperty, value);
		    return config;
	    }

	    #endregion
	}
}
