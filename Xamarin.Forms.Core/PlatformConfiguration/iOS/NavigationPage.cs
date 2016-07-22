
namespace Xamarin.Forms.PlatformConfiguration.iOS
{
	using FormsElement = Xamarin.Forms.NavigationPage;

	public static class NavigationPage
	{
		public static readonly BindableProperty IsNavigationBarTranslucentProperty =
			BindableProperty.Create("IsNavigationBarTranslucent", typeof(bool),
			typeof(NavigationPage), false);

		public static bool GetIsNavigationBarTranslucent(BindableObject element)
		{
			return (bool)element.GetValue(IsNavigationBarTranslucentProperty);
		}

		public static void SetIsNavigationBarTranslucent(BindableObject element, bool value)
		{
			element.SetValue(IsNavigationBarTranslucentProperty, value);
		}

		public static bool IsNavigationBarTranslucent(this IPlatformElementConfiguration<iOSPlatform, FormsElement> config)
		{
			return GetIsNavigationBarTranslucent(config.Element);
		}

		public static IPlatformElementConfiguration<iOSPlatform, FormsElement> EnableTranslucentNavigationBar(this IPlatformElementConfiguration<iOSPlatform, FormsElement> config, bool value)
		{
			SetIsNavigationBarTranslucent(config.Element, value);
			return config;
		}
	}
}
