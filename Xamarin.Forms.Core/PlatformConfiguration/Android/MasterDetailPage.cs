
namespace Xamarin.Forms.PlatformConfiguration.Android
{
	using FormsElement = Xamarin.Forms.MasterDetailPage;

	public static class MasterDetailPage
	{
		public static readonly BindableProperty SomeAndroidThingProperty =
			BindableProperty.Create("SomeAndroidThing", typeof(int),
			typeof(MasterDetailPage), 1);

		public static readonly BindableProperty SomeOtherAndroidThingProperty =
			BindableProperty.Create("SomeOtherAndroidThing", typeof(int),
			typeof(MasterDetailPage), 1);

		public static int GetSomeAndroidThing(BindableObject element)
		{
			return (int)element.GetValue(SomeAndroidThingProperty);
		}

		public static void SetSomeAndroidThing(BindableObject element, int value)
		{
			element.SetValue(SomeAndroidThingProperty, value);
		}

		public static int GetSomeOtherAndroidThing(BindableObject element)
		{
			return (int)element.GetValue(SomeOtherAndroidThingProperty);
		}

		public static void SetSomeOtherAndroidThing(BindableObject element, int value)
		{
			element.SetValue(SomeOtherAndroidThingProperty, value);
		}

		public static int GetSomeAndroidThing(this IPlatformElementConfiguration<AndroidPlatform, FormsElement> config)
		{
			return (int)config.Element.GetValue(SomeAndroidThingProperty);
		}

		public static IPlatformElementConfiguration<AndroidPlatform, FormsElement> SetSomeAndroidThing(this IPlatformElementConfiguration<AndroidPlatform, FormsElement> config,
			int value)
		{
			config.Element.SetValue(SomeAndroidThingProperty, value);
			return config;
		}

		public static int GetSomeOtherAndroidThing(this IPlatformElementConfiguration<AndroidPlatform, FormsElement> config)
		{
			return (int)config.Element.GetValue(SomeOtherAndroidThingProperty);
		}

		public static IPlatformElementConfiguration<AndroidPlatform, FormsElement> SetSomeOtherAndroidThing(this IPlatformElementConfiguration<AndroidPlatform, FormsElement> config, int value)
		{
			config.Element.SetValue(SomeOtherAndroidThingProperty, value);
			return config;
		}

		public static IPlatformElementConfiguration<AndroidPlatform, FormsElement> UseTabletDefaults(this IPlatformElementConfiguration<AndroidPlatform, FormsElement> config)
		{
			config.SetSomeAndroidThing(10);
			config.SetSomeOtherAndroidThing(45);
			return config;
		}

		public static IPlatformElementConfiguration<AndroidPlatform, FormsElement> UsePhabletDefaults(this IPlatformElementConfiguration<AndroidPlatform, FormsElement> config)
		{
			config.SetSomeAndroidThing(8);
			config.SetSomeOtherAndroidThing(40);
			return config;
		}
	}
}
