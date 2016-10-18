namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.ListView;

	public static class ListView
	{
		public static readonly BindableProperty BouncesProperty = BindableProperty.Create(nameof(Bounces), typeof(bool), typeof(ListView), true);

		public static bool GetBounces(BindableObject element)
		{
			return (bool)element.GetValue(BouncesProperty);
		}

		public static void SetBounces(BindableObject element, bool value)
		{
			element.SetValue(BouncesProperty, value);
		}

		public static bool Bounces(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetBounces(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetBounces(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetBounces(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> EnableBounces(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetBounces(config.Element, true);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> DisableBounces(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetBounces(config.Element, false);
			return config;
		}
	}
}