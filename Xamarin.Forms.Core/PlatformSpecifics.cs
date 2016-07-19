using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.PlatformConfiguration
{
	public interface IConfigElement<out T> where T : Element
	{
		T Element { get; }
	}

	public interface IPlatformElementConfiguration<out TPlatform, out TElement> : IConfigElement<TElement>
		where TPlatform : IConfigPlatform
 		where TElement : Element
	{
	}

	public interface IElementConfiguration<out TElement> where TElement : Element
	{
		IPlatformElementConfiguration<T, TElement> On<T>() where T : IConfigPlatform;
	}

	public interface IConfigPlatform { }

	public class Configuration<TPlatform, TElement> : IPlatformElementConfiguration<TPlatform, TElement>
		where TPlatform : IConfigPlatform
		where TElement : Element

	{
		public Configuration(TElement element)
		{
			Element = element;
		}

		public TElement Element { get; }

		public static Configuration<TPlatform, TElement> Create(TElement element)
		{
			return new Configuration<TPlatform, TElement>(element);
		}
	}

	/// <summary>
	/// Helper that handles storing and lookup of platform specifics implementations
	/// </summary>
	/// <typeparam name="TElement">The Element type</typeparam>
	internal class PlatformConfigurationRegistry<TElement> : IElementConfiguration<TElement>
		where TElement : Element
	{
		readonly TElement _element;
		readonly Dictionary<Type, object> _platformSpecifics = new Dictionary<Type, object>();

		internal PlatformConfigurationRegistry(TElement element)
		{
			_element = element;
		}

		public IPlatformElementConfiguration<T, TElement> On<T>() where T : IConfigPlatform
		{
			if (_platformSpecifics.ContainsKey(typeof(T)))
			{
				return (IPlatformElementConfiguration<T, TElement>)_platformSpecifics[typeof(T)];
			}

			var emptyConfig = Configuration<T, TElement>.Create(_element);

			_platformSpecifics.Add(typeof(T), emptyConfig);

			return emptyConfig;
		}
	}
}

namespace Xamarin.Forms.PlatformConfiguration.Android
{
	using FormsElement = Xamarin.Forms.MasterDetailPage;

	public interface AndroidPlatform : IConfigPlatform { }

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

// Use the platform specific namespace to hide the platform specific properties from renderers that don't care about them
namespace Xamarin.Forms.PlatformConfiguration.Windows
{
	// Alias the Forms Element to make implementation easier 
	// and to distinguish from the configuration class.
	using FormsElement = Xamarin.Forms.MasterDetailPage;

	public interface WindowsPlatform : IConfigPlatform { }

	// define any required enums, etc.
	public enum CollapseStyle
	{
		None,
		Partial
	}

	// Create static configuration class
	public static class MasterDetailPage
	{
		// Create Attached BindableProperty on the Xamarin.Forms.PlatformConfiguration.Windows.MasterDetailPage class
		public static readonly BindableProperty CollapseStyleProperty =
			BindableProperty.CreateAttached("CollapseStyle", typeof(CollapseStyle),
			typeof(MasterDetailPage), CollapseStyle.None);

		// Implement required Get/Set methods for XAML binding to Attached BindableProperty.
		public static CollapseStyle GetCollapseStyle(BindableObject element)
		{
			return (CollapseStyle)element.GetValue(CollapseStyleProperty);
		}

		public static void SetCollapseStyle(BindableObject element, CollapseStyle value)
		{
			element.SetValue(CollapseStyleProperty, value);
		}

		// Implement extension methods for the discoverable fluent implementation
		// Target the WindowsPlatform and the FormsElement (i.e., Xamarin.Forms.MasterDetailPage)
		public static CollapseStyle GetCollapseStyle(this IPlatformElementConfiguration<WindowsPlatform, FormsElement> config)
		{
			// Recommended: put all logic in the Get/Set methods defined above and simply call those from the
			// extension methods.
			return GetCollapseStyle(config.Element);
		}

		public static IPlatformElementConfiguration<WindowsPlatform, FormsElement> UseCollapseStyle(this IPlatformElementConfiguration<WindowsPlatform, FormsElement> config, CollapseStyle value)
		{
			SetCollapseStyle(config.Element, value);
			return config;

		}

		// Optionally create convenience methods for quick configuration
		public static IPlatformElementConfiguration<WindowsPlatform, FormsElement> UsePartialCollapse(
			this IPlatformElementConfiguration<WindowsPlatform, FormsElement> config)
		{
			SetCollapseStyle(config.Element, CollapseStyle.Partial);
			return config;
		}
	}
}

namespace Xamarin.Forms.PlatformConfiguration.iOS
{
	using FormsElement = Xamarin.Forms.NavigationPage;

	public interface iOSPlatform : IConfigPlatform { }

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