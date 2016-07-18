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
	public interface IConfigAndroid : IConfigPlatform { }

	public static class MasterDetailPageConfig
	{
		#region Properties

		public static readonly BindableProperty SomeAndroidThingProperty = BindableProperty.Create("SomeAndroidThing",
			typeof(int),
			typeof(MasterDetailPageConfig), 1);

		public static readonly BindableProperty SomeOtherAndroidThingProperty =
			BindableProperty.Create("SomeOtherAndroidThing", typeof(int),
				typeof(MasterDetailPageConfig), 1);

		#endregion

		#region Configuration

		public static IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> SetSomeAndroidThing(this IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> config,
			int value)
		{
			config.Element.SetValue(SomeAndroidThingProperty, value);
			return config;
		}

		public static IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> SetSomeOtherAndroidThing(this IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> config,
			int value)
		{
			config.Element.SetValue(SomeOtherAndroidThingProperty, value);
			return config;
		}

		public static int GetSomeAndroidThing(this IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> config)
		{
			return (int)config.Element.GetValue(SomeAndroidThingProperty);
		}

		public static int GetSomeOtherAndroidThing(this IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> config)
		{
			return (int)config.Element.GetValue(SomeOtherAndroidThingProperty);
		}

		public static IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> UseTabletDefaults(this IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> config)
		{
			config.SetSomeAndroidThing(10);
			config.SetSomeOtherAndroidThing(45);
			return config;
		}

		public static IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> UsePhabletDefaults(this IPlatformElementConfiguration<IConfigAndroid, MasterDetailPage> config)
		{
			config.SetSomeAndroidThing(8);
			config.SetSomeOtherAndroidThing(40);
			return config;
		}

		#endregion
	}
}

namespace Xamarin.Forms.PlatformConfiguration.Windows
{
	public interface IConfigWindows : IConfigPlatform { }

	public enum CollapseStyle
	{
		None,
		Partial
	}
	public static class MasterDetailPageConfig
	{
		public static readonly BindableProperty CollapseStyleProperty = BindableProperty.Create("CollapseStyle",
			typeof(CollapseStyle),
			typeof(MasterDetailPageConfig), CollapseStyle.None);

		public static CollapseStyle GetCollapseStyle(this IPlatformElementConfiguration<IConfigWindows, MasterDetailPage> config)
		{
			return (CollapseStyle)config.Element.GetValue(CollapseStyleProperty);
		}

		public static void SetCollapseStyle(this IPlatformElementConfiguration<IConfigWindows, MasterDetailPage> config, CollapseStyle value)
		{
			config.Element.SetValue(CollapseStyleProperty, value);
		}

		public static IPlatformElementConfiguration<IConfigWindows, MasterDetailPage> UsePartialCollapse(
			this IPlatformElementConfiguration<IConfigWindows, MasterDetailPage> config)
		{
			config.Element.SetValue(CollapseStyleProperty, CollapseStyle.Partial);
			return config;
		}
	}
}

namespace Xamarin.Forms.PlatformConfiguration.iOS
{
	public interface IConfigIOS : IConfigPlatform { }

	public static class NavigationPageConfig
	{
		public static readonly BindableProperty IsNavigationBarTranslucentProperty =
			BindableProperty.Create("IsNavigationBarTranslucent", typeof(bool),
				typeof(NavigationPageConfig), false);

		public static bool GetNavigationBarIsTranslucent(this IPlatformElementConfiguration<IConfigIOS, NavigationPage> config)
		{
			return (bool)config.Element.GetValue(IsNavigationBarTranslucentProperty);
		}
		public static IPlatformElementConfiguration<IConfigIOS, NavigationPage> SetNavigationBarIsTranslucent(this IPlatformElementConfiguration<IConfigIOS, NavigationPage> config, bool value)
		{
			config.Element.SetValue(IsNavigationBarTranslucentProperty, value);
			return config;
		}
	}
}

namespace ImAVendor.Forms.PlatformConfiguration.iOS
{
	using Xamarin.Forms.PlatformConfiguration;
	using Xamarin.Forms.PlatformConfiguration.iOS;

	public static class FakeVendorConfig
	{
		const string NavBarTranslucentEffectName = "XamControl.NavigationPageTranslucentEffect";

		public static readonly BindableProperty FooProperty = BindableProperty.Create("VendorFoo", typeof(bool), typeof(IPlatformElementConfiguration<IConfigIOS, MasterDetailPage>), true);

		public static void SetVendorFoo(this IPlatformElementConfiguration<IConfigIOS, MasterDetailPage> mdp, bool value)
		{
			mdp.Element.SetValue(FooProperty, value);
		}

		public static bool GetVendorFoo(this IPlatformElementConfiguration<IConfigIOS, MasterDetailPage> mdp)
		{
			return (bool)mdp.Element.GetValue(FooProperty);
		}


		public static bool GetIsNavigationBarTranslucent(BindableObject element)
		{
			return (bool)element.GetValue(IsNavigationBarTranslucentProperty);
		}

		public static readonly BindableProperty IsNavigationBarTranslucentProperty =
	BindableProperty.CreateAttached("IsNavigationBarTranslucent", typeof(bool),
		typeof(FakeVendorConfig), false, propertyChanging: IsNavigationBarTranslucentPropertyChanging);

		public static void SetIsNavigationBarTranslucent(BindableObject element, bool value)
		{
			element.SetValue(IsNavigationBarTranslucentProperty, value);
		}

		public static bool GetNavigationBarIsTranslucentVendor(this IPlatformElementConfiguration<IConfigIOS, NavigationPage> config)
		{
			return GetIsNavigationBarTranslucent(config.Element);
		}

		public static IPlatformElementConfiguration<IConfigIOS, NavigationPage> SetNavigationBarIsTranslucentVendor(this IPlatformElementConfiguration<IConfigIOS, NavigationPage> config, bool value)
		{
			SetIsNavigationBarTranslucent(config.Element, value);
			return config;
		}

		static void IsNavigationBarTranslucentPropertyChanging(BindableObject bindable, object oldValue, object newValue)
		{
			AttachEffect(bindable as NavigationPage);
		}

		static void AttachEffect(NavigationPage element)
		{
			IElementController controller = element;
			if (controller == null || controller.EffectIsAttached(NavBarTranslucentEffectName))
				return;

			element.Effects.Add(Effect.Resolve(NavBarTranslucentEffectName));
		}
	}
}