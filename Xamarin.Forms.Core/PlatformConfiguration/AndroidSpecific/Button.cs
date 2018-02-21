namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.Button;

	public static class Button
	{
		public static readonly BindableProperty UseNativePaddingProperty = BindableProperty.Create("UseNativePadding", typeof(bool), typeof(Button), true);

		public static bool GetUseNativePadding(BindableObject element)
		{
			return (bool)element.GetValue(UseNativePaddingProperty);
		}

		public static void SetUseNativePadding(BindableObject element, bool value)
		{
			element.SetValue(UseNativePaddingProperty, value);
		}

		public static bool UseNativePadding(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetUseNativePadding(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetUseNativePadding(this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
		{
			SetUseNativePadding(config.Element, value);
			return config;
		}
	}
}
