namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.ListView;

	public static class ListView
	{
		public static readonly BindableProperty IsBounceEnabledProperty = BindableProperty.Create(nameof(IsBounceEnabled), typeof(bool), typeof(ListView), true);

		public static bool GetIsBounceEnabled(BindableObject element)
		{
			return (bool)element.GetValue(IsBounceEnabledProperty);
		}

		public static void SetIsBounceEnabled(BindableObject element, bool value)
		{
			element.SetValue(IsBounceEnabledProperty, value);
		}

		public static bool IsBounceEnabled(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetIsBounceEnabled(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetIsBounceEnabled(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetIsBounceEnabled(config.Element, value);
			return config;
		}
	}
}