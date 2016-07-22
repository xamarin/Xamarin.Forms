using NUnit.Framework;
using Xamarin.Forms.PlatformConfiguration.Android;
using Xamarin.Forms.PlatformConfiguration.iOS;
using ImAVendor.Forms.PlatformConfiguration.iOS;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class PlatformSpecificsTests
	{
		[Test]
		public void VendorPlatformProperty()
		{
			var x = new MasterDetailPage();

			Assert.IsTrue(x.On<iOSPlatform>().GetVendorFoo());

			x.On<iOSPlatform>().SetVendorFoo(false);

			Assert.IsFalse(x.On<iOSPlatform>().GetVendorFoo());
		}

		[Test]
		public void ConsumeVendorSetting()
		{
			var x = new MasterDetailPage();
			x.On<iOSPlatform>().SetVendorFoo(false);

			Assert.IsFalse(x.On<iOSPlatform>().GetVendorFoo());
		}

		[Test]
		public void Properties()
		{
			var x = new MasterDetailPage();
			x.On<AndroidPlatform>().SetSomeAndroidThing(42);

			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeAndroidThing() == 42);
		}

		[Test]
		public void ConvenienceConfiguration()
		{
			var x = new MasterDetailPage();

			x.On<AndroidPlatform>().UseTabletDefaults();

			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeAndroidThing() == 10);
			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeOtherAndroidThing() == 45);

			x.On<AndroidPlatform>().UsePhabletDefaults();
			
			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeAndroidThing() == 8);
			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeOtherAndroidThing() == 40);
		}

		[Test]
		public void NavigationPageiOSConfiguration()
		{
			var x = new NavigationPage();

			x.On<iOSPlatform>().EnableTranslucentNavigationBar(true);

			Assert.IsTrue(x.On<iOSPlatform>().IsNavigationBarTranslucent()); 
		}
	}
}

namespace ImAVendor.Forms.PlatformConfiguration.iOS
{
	using Xamarin.Forms;
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
	using Xamarin.Forms;
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
