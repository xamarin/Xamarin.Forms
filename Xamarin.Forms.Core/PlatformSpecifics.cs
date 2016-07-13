namespace Xamarin.Forms
{
	public interface IElementConfiguration<out T> where T : Element
	{
		T Element { get; }
	}

	#region MDP

	public interface IMasterDetailPagePlatformConfiguration
	{
		IMasterDetailPageAndroidConfiguration OnAndroid();
		IMasterDetailPageiOSConfiguration OniOS();
		IMasterDetailPageWindowsConfiguration OnWindows();
	}

	#region Windows

	public interface IMasterDetailPageWindowsConfiguration : IElementConfiguration<MasterDetailPage>
	{
		CollapseStyle CollapseStyle { get; set; }
	}

	public class MasterDetailPageWindowsConfiguration : IMasterDetailPageWindowsConfiguration
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
		public static IMasterDetailPageWindowsConfiguration UsePartialCollapse(
			this IMasterDetailPageWindowsConfiguration config)
		{
			config.CollapseStyle = CollapseStyle.Partial;
			return config;
		}
	}

	#endregion

	#region Android

	public interface IMasterDetailPageAndroidConfiguration : IElementConfiguration<MasterDetailPage>
	{
		int SomeAndroidThing { get; set; }

		int SomeOtherAndroidThing { get; set; }
	}

	internal class MasterDetailPageAndroidConfiguration : IMasterDetailPageAndroidConfiguration
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

		public static IMasterDetailPageAndroidConfiguration UseTabletDefaults(
			this IMasterDetailPageAndroidConfiguration config)
		{
			config.SomeAndroidThing = 10;
			config.SomeOtherAndroidThing = 45;

			return config;
		}

		public static IMasterDetailPageAndroidConfiguration UsePhabletDefaults(
			this IMasterDetailPageAndroidConfiguration config)
		{
			config.SomeAndroidThing = 8;
			config.SomeOtherAndroidThing = 40;

			return config;
		}

		public static IMasterDetailPageAndroidConfiguration UsePhoneDefaults(this IMasterDetailPageAndroidConfiguration config)
		{
			config.SomeAndroidThing = 5;
			config.SomeOtherAndroidThing = 30;

			return config;
		}

		public static IMasterDetailPageAndroidConfiguration SetThing(this IMasterDetailPageAndroidConfiguration config,
			int value)
		{
			config.SomeAndroidThing = value;
			return config;
		}

		#endregion
	}

	#endregion

	#region iOS

	public interface IMasterDetailPageiOSConfiguration : IElementConfiguration<MasterDetailPage>
	{
	}

	internal class MasterDetailPageiOsConfiguration : EmptyElementConfiguration<MasterDetailPage>,
		IMasterDetailPageiOSConfiguration
	{
		public MasterDetailPageiOsConfiguration(MasterDetailPage element) : base(element)
		{
		}
	}

	#endregion

	#endregion

	#region NavigationPage 

	public interface INavigationPagePlatformConfiguration
	{
		INavigationPageAndroidConfiguration OnAndroid();
		INavigationPageiOSConfiguration OniOS();
		INavigationPageWindowsConfiguration OnWindows();
	}

	#region Windows

	public interface INavigationPageWindowsConfiguration : IElementConfiguration<NavigationPage>
	{
	}

	public class NavigationPageWindowsConfiguration : EmptyElementConfiguration<NavigationPage>,
		INavigationPageWindowsConfiguration
	{
		public NavigationPageWindowsConfiguration(NavigationPage element) : base(element)
		{
		}
	}

	#endregion

	#region Android

	public interface INavigationPageAndroidConfiguration : IElementConfiguration<NavigationPage>
	{
	}

	public class NavigationPageAndroidConfiguration : EmptyElementConfiguration<NavigationPage>,
		INavigationPageAndroidConfiguration
	{
		public NavigationPageAndroidConfiguration(NavigationPage element) : base(element)
		{
		}
	}

	#endregion

	#region iOS

	public interface INavigationPageiOSConfiguration : IElementConfiguration<NavigationPage>
	{
		bool IsNavigationBarTranslucent { get; set; }
	}

	public class NavigationPageiOSConfiguration : INavigationPageiOSConfiguration
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
		public static readonly BindableProperty FooProperty = BindableProperty.Create("Foo", typeof(bool), typeof(MasterDetailPage), true);

		public static void SetFoo(this MasterDetailPage mdp, bool value)
		{
			mdp.SetValue(FooProperty, value);
		}

		public static bool GetFoo(this MasterDetailPage mdp)
		{
			return (bool)mdp.GetValue(FooProperty);
		}

		public static void SetVendorFoo(this IMasterDetailPageWindowsConfiguration config, bool value)
		{
			config.Element.SetFoo(value);
		}
	}

	#endregion
}