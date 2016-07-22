using Xamarin.Forms;

namespace ImAVendor.Forms.PlatformConfiguration.iOS
{
	using Xamarin.Forms.PlatformConfiguration;
	using Xamarin.Forms.PlatformConfiguration.iOS;
	using FormsElement = Xamarin.Forms.MasterDetailPage;

	public static class MasterDetailPage
	{
		public static readonly BindableProperty FooProperty =
			BindableProperty.Create("VendorFoo", typeof(bool),
			typeof(MasterDetailPage), true);

		public static void SetVendorFoo(BindableObject element, bool value)
		{
			element.SetValue(FooProperty, value);
		}

		public static bool GetVendorFoo(BindableObject element)
		{
			return (bool)element.GetValue(FooProperty);
		}

		public static IPlatformElementConfiguration<iOSPlatform, FormsElement> SetVendorFoo(this IPlatformElementConfiguration<iOSPlatform, FormsElement> config, bool value)
		{
			SetVendorFoo(config.Element, value);
			return config;
		}

		public static bool GetVendorFoo(this IPlatformElementConfiguration<iOSPlatform, FormsElement> mdp)
		{
			return GetVendorFoo(mdp.Element);
		}
	}
}

namespace ImAVendor.Forms.PlatformConfiguration.iOS
{
	using Xamarin.Forms.PlatformConfiguration;
	using Xamarin.Forms.PlatformConfiguration.iOS;
	using FormsElement = Xamarin.Forms.NavigationPage;

	public static class NavigationPage
	{
		const string NavBarTranslucentEffectName = "XamControl.NavigationPageTranslucentEffect";

		public static readonly BindableProperty IsNavigationBarTranslucentProperty =
			BindableProperty.CreateAttached("IsNavigationBarTranslucent", typeof(bool),
			typeof(NavigationPage), false, propertyChanging: IsNavigationBarTranslucentPropertyChanging);

		public static bool GetIsNavigationBarTranslucent(BindableObject element)
		{
			return (bool)element.GetValue(IsNavigationBarTranslucentProperty);
		}

		public static void SetIsNavigationBarTranslucent(BindableObject element, bool value)
		{
			element.SetValue(IsNavigationBarTranslucentProperty, value);
		}

		public static bool IsNavigationBarTranslucentVendor(this IPlatformElementConfiguration<iOSPlatform, FormsElement> config)
		{
			return GetIsNavigationBarTranslucent(config.Element);
		}

		public static IPlatformElementConfiguration<iOSPlatform, FormsElement> EnableTranslucentNavigationBarVendor(this IPlatformElementConfiguration<iOSPlatform, FormsElement> config, bool value)
		{
			SetIsNavigationBarTranslucent(config.Element, value);
			return config;
		}

		static void IsNavigationBarTranslucentPropertyChanging(BindableObject bindable, object oldValue, object newValue)
		{
			AttachEffect(bindable as FormsElement);
		}

		static void AttachEffect(FormsElement element)
		{
			IElementController controller = element;
			if (controller == null || controller.EffectIsAttached(NavBarTranslucentEffectName))
				return;

			element.Effects.Add(Effect.Resolve(NavBarTranslucentEffectName));
		}
	}
}