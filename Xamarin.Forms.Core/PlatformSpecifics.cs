using System;
using System.Collections.Generic;

namespace Xamarin.Forms
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

	public interface IConfigIOS : IConfigPlatform { }

	public interface IConfigAndroid : IConfigPlatform { }

	public interface IConfigWindows : IConfigPlatform { }

	#region MDP

	#region Windows

	public static class MasterDetailPageWindowsSpecifics
	{
		public static readonly BindableProperty CollapseStyleProperty = BindableProperty.Create("CollapseStyle",
			typeof(CollapseStyle),
			typeof(IPlatformElementConfiguration<IConfigWindows, MasterDetailPage>), CollapseStyle.None);

		public static CollapseStyle GetCollapseStyle(this IPlatformElementConfiguration<IConfigWindows, MasterDetailPage> config)
		{
			return (CollapseStyle)config.Element.GetValue(CollapseStyleProperty);
		}

		public static void SetCollapseStyle(this IPlatformElementConfiguration<IConfigWindows, MasterDetailPage> config, CollapseStyle value)
		{
			config.Element.SetValue(CollapseStyleProperty, value);
		}
	}

	public static class MasterDetailPageWindowsConfigurationExtensions
	{
		public static IPlatformElementConfiguration<IConfigWindows, MasterDetailPage> UsePartialCollapse(
			this IPlatformElementConfiguration<IConfigWindows, MasterDetailPage> config)
		{
			config.Element.SetValue(MasterDetailPageWindowsSpecifics.CollapseStyleProperty, CollapseStyle.Partial);
			return config;
		}
	}

	#endregion

	#region Android

	public static class MasterDetailPageAndroidSpecifics
	{
		#region Properties
		
		public static readonly BindableProperty SomeAndroidThingProperty = BindableProperty.Create("SomeAndroidThing",
			typeof(int),
			typeof(MasterDetailPage), 1);

		public static readonly BindableProperty SomeOtherAndroidThingProperty =
			BindableProperty.Create("SomeOtherAndroidThing", typeof(int),
				typeof(MasterDetailPage), 1);
		
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

	#endregion

	#region iOS

	public class MasterDetailPageiOSConfiguration : Configuration<IConfigIOS, MasterDetailPage>
	{
		public MasterDetailPageiOSConfiguration (MasterDetailPage element) : base(element)
		{
		}
	}

	#endregion

	#endregion

	#region NavigationPage 

	#region iOS

	public static class NavigationPageiOSpecifics
	{
		public static readonly BindableProperty IsNavigationBarTranslucentProperty =
			BindableProperty.Create("IsNavigationBarTranslucent", typeof(bool),
				typeof(NavigationPage), false);

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

	#endregion

	#endregion

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

	#region Vendor
	
	public static class FakeVendorExtensions
	{
		public static readonly BindableProperty FooProperty = BindableProperty.Create("VendorFoo", typeof(bool), typeof(IPlatformElementConfiguration<IConfigIOS, MasterDetailPage> ), true);

		public static void SetVendorFoo(this IPlatformElementConfiguration<IConfigIOS, MasterDetailPage>  mdp, bool value)
		{
			mdp.Element.SetValue(FooProperty, value);
		}

		public static bool GetVendorFoo(this IPlatformElementConfiguration<IConfigIOS, MasterDetailPage>  mdp)
		{
			return (bool)mdp.Element.GetValue(FooProperty);
		}
	}

	#endregion

	// Should this be in a different namespace? Xamarin.Forms.PlatformSpecific

	public enum CollapseStyle
	{
		None,
		Partial
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