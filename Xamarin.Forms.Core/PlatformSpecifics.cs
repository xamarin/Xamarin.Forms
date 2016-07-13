namespace Xamarin.Forms
{
	public interface IElementConfiguration<out T> where T : Element
	{
		T Element { get; }
	}

	#region MDP

	public interface IMasterDetailPagePlatformConfiguration
	{
		MasterDetailPageAndroidConfiguration OnAndroid();
		MasterDetailPageiOSConfiguration OniOS();
		MasterDetailPageWindowsConfiguration  OnWindows();
	}

	#region Windows

	public class MasterDetailPageWindowsConfiguration : IElementConfiguration<MasterDetailPage>
	{
		public MasterDetailPageWindowsConfiguration(MasterDetailPage element)
		{
			Element = element;
		}

		public MasterDetailPage Element { get; }

		public CollapseStyle CollapseStyle
		{
			get { return (CollapseStyle)Element.GetValue(MasterDetailPageWindowsSpecifics.CollapseStyleProperty); }
			set { Element.SetValue(MasterDetailPageWindowsSpecifics.CollapseStyleProperty, value); }
		}
	}

	public static class MasterDetailPageWindowsSpecifics
	{
		public static readonly BindableProperty CollapseStyleProperty = BindableProperty.Create("CollapseStyle",
			typeof(CollapseStyle),
			typeof(MasterDetailPage), CollapseStyle.None);

		public static CollapseStyle GetCollapseStyle(this MasterDetailPage mdp)
		{
			return (CollapseStyle)mdp.GetValue(CollapseStyleProperty);
		}

		public static void SetCollapseStyle(this MasterDetailPage mdp, CollapseStyle value)
		{
			mdp.SetValue(CollapseStyleProperty, value);
		}
	}

	public static class MasterDetailPageWindowsConfigurationExtensions
	{
		public static MasterDetailPageWindowsConfiguration UsePartialCollapse(
			this MasterDetailPageWindowsConfiguration config)
		{
			config.CollapseStyle = CollapseStyle.Partial;
			return config;
		}
	}

	#endregion

	#region Android

	public class MasterDetailPageAndroidConfiguration : IElementConfiguration<MasterDetailPage>
	{
		public MasterDetailPageAndroidConfiguration(MasterDetailPage element)
		{
			Element = element;
		}

		public MasterDetailPage Element { get; }

		public int SomeAndroidThing
		{
			get { return (int)Element.GetValue(MasterDetailPageAndroidSpecifics.SomeAndroidThingProperty); }
			set { Element.SetValue(MasterDetailPageAndroidSpecifics.SomeAndroidThingProperty, value); }
		}

		public int SomeOtherAndroidThing
		{
			get { return (int)Element.GetValue(MasterDetailPageAndroidSpecifics.SomeOtherAndroidThingProperty); }
			set { Element.SetValue(MasterDetailPageAndroidSpecifics.SomeOtherAndroidThingProperty, value); }
		}
	}

	public static class MasterDetailPageAndroidSpecifics
	{
		#region Properties

		public static readonly BindableProperty SomeAndroidThingProperty = BindableProperty.Create("SomeAndroidThing",
			typeof(int),
			typeof(MasterDetailPage), 1);

		public static void SetSomeAndroidThing(this MasterDetailPage mdp, int value)
		{
			mdp.SetValue(SomeAndroidThingProperty, value);
		}

		public static int GetSomeAndroidThing(this MasterDetailPage mdp)
		{
			return (int)mdp.GetValue(SomeAndroidThingProperty);
		}

		public static readonly BindableProperty SomeOtherAndroidThingProperty =
			BindableProperty.Create("SomeOtherAndroidThing", typeof(int),
				typeof(MasterDetailPage), 1);

		public static void SetSomeOtherAndroidThing(this MasterDetailPage mdp, int value)
		{
			mdp.SetValue(SomeOtherAndroidThingProperty, value);
		}

		public static int GetSomeOtherAndroidThing(this MasterDetailPage mdp)
		{
			return (int)mdp.GetValue(SomeOtherAndroidThingProperty);
		}

		#endregion

		#region Configuration

		public static MasterDetailPageAndroidConfiguration UseTabletDefaults(
			this MasterDetailPageAndroidConfiguration config)
		{
			config.SomeAndroidThing = 10;
			config.SomeOtherAndroidThing = 45;

			return config;
		}

		public static MasterDetailPageAndroidConfiguration UsePhabletDefaults(
			this MasterDetailPageAndroidConfiguration config)
		{
			config.SomeAndroidThing = 8;
			config.SomeOtherAndroidThing = 40;

			return config;
		}

		public static MasterDetailPageAndroidConfiguration UsePhoneDefaults(this MasterDetailPageAndroidConfiguration config)
		{
			config.SomeAndroidThing = 5;
			config.SomeOtherAndroidThing = 30;

			return config;
		}

		public static MasterDetailPageAndroidConfiguration SetThing(this MasterDetailPageAndroidConfiguration config,
			int value)
		{
			config.SomeAndroidThing = value;
			return config;
		}

		#endregion
	}

	#endregion

	#region iOS

	public class MasterDetailPageiOSConfiguration : EmptyElementConfiguration<MasterDetailPage>
	{
		public MasterDetailPageiOSConfiguration (MasterDetailPage element) : base(element)
		{
		}
	}

	#endregion

	#endregion

	#region NavigationPage 

	public interface INavigationPagePlatformConfiguration
	{
		NavigationPageAndroidConfiguration OnAndroid();
		NavigationPageiOSConfiguration OniOS();
		NavigationPageWindowsConfiguration OnWindows();
	}

	#region Windows

	public class NavigationPageWindowsConfiguration : EmptyElementConfiguration<NavigationPage>
	{
		public NavigationPageWindowsConfiguration(NavigationPage element) : base(element)
		{
		}
	}

	#endregion

	#region Android

	public class NavigationPageAndroidConfiguration : EmptyElementConfiguration<NavigationPage>
	{
		public NavigationPageAndroidConfiguration(NavigationPage element) : base(element)
		{
		}
	}

	#endregion

	#region iOS

	public class NavigationPageiOSConfiguration : IElementConfiguration<NavigationPage>
	{
		public NavigationPageiOSConfiguration(NavigationPage element)
		{
			Element = element;
		}

		public NavigationPage Element { get; }

		public bool IsNavigationBarTranslucent
		{
			get { return (bool)Element.GetValue(NavigationPageiOSpecifics.IsNavigationBarTranslucentProperty); }
			set { Element.SetValue(NavigationPageiOSpecifics.IsNavigationBarTranslucentProperty, value); }
		}
	}

	public static class NavigationPageiOSpecifics
	{
		public static readonly BindableProperty IsNavigationBarTranslucentProperty =
			BindableProperty.Create("IsNavigationBarTranslucent", typeof(bool),
				typeof(NavigationPage), false);

		public static bool GetIsNavigationBarTranslucent(this NavigationPage navigationPage)
		{
			return (bool)navigationPage.GetValue(IsNavigationBarTranslucentProperty);
		}

		public static void SetIsNavigationBarTranslucent(this NavigationPage navigationPage, bool value)
		{
			navigationPage.SetValue(IsNavigationBarTranslucentProperty, value);
		}
	}

	#endregion

	#endregion

	public class EmptyElementConfiguration<T> : IElementConfiguration<T> where T : Element
	{
		public EmptyElementConfiguration(T element)
		{
			Element = element;
		}

		public T Element { get; }
	}

	#region Vendor
	
	public static class FakeVendorExtensions
	{
		public static readonly BindableProperty FooProperty = BindableProperty.Create("VendorFoo", typeof(bool), typeof(MasterDetailPageWindowsConfiguration), true);

		public static void SetVendorFoo(this MasterDetailPageWindowsConfiguration mdp, bool value)
		{
			mdp.Element.SetValue(FooProperty, value);
		}

		public static bool GetVendorFoo(this MasterDetailPageWindowsConfiguration mdp)
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
}