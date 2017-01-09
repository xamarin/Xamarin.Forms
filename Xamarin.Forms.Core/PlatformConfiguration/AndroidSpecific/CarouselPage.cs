namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.CarouselPage;

	public static class CarouselPage
    {
		public static readonly BindableProperty IsSwipePagingEnabledProperty =
			BindableProperty.Create("IsSwipePagingEnabled", typeof(bool),
			typeof(CarouselPage), true);

		public static readonly BindableProperty OffscreenPageLimitProperty =
			BindableProperty.Create("OffscreenPageLimit", typeof(int),
			typeof(CarouselPage), 3, validateValue: (binding, value) => (int)value >= 0);

		public static int GetOffscreenPageLimit(BindableObject element)
		{
			return (int)element.GetValue(OffscreenPageLimitProperty);
		}

		public static void SetOffscreenPageLimit(BindableObject element, int value)
		{
			element.SetValue(OffscreenPageLimitProperty, value);
		}

		public static int OffscreenPageLimit(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetOffscreenPageLimit(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetOffscreenPageLimit(this IPlatformElementConfiguration<Android, FormsElement> config, int value)
		{
			SetOffscreenPageLimit(config.Element, value);
			return config;
		}
	}
}
