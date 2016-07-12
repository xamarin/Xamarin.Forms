namespace Xamarin.Forms
{
	public interface IElementConfiguration<out T> where T : Element
	{
		T Element { get; }
	}

	#region MDP

	public interface IMasterDetailPagePlatformConfiguration
	{
		IMasterDetailPageWindowsConfiguration OnWindows();

		IMasterDetailPageAndroidConfiguration OnAndroid();

		IMasterDetailPageiOSConfiguration OniOS();
	}

	public interface IMasterDetailPageWindowsConfiguration : IElementConfiguration<MasterDetailPage>
	{
		CollapseStyle CollapseStyle { get; set; }
	}

	public interface IMasterDetailPageAndroidConfiguration : IElementConfiguration<MasterDetailPage>
	{
		int SomeAndroidThing { get; set; }

		int SomeOtherAndroidThing { get; set; }
	}

	public interface IMasterDetailPageiOSConfiguration
	{
	}

	public class MasterDetailPageWindowsConfiguration : IMasterDetailPageWindowsConfiguration
	{
		public MasterDetailPageWindowsConfiguration(MasterDetailPage element)
		{
			Element = element;
		}

		public CollapseStyle CollapseStyle 
		{
			get { return (CollapseStyle)Element.GetValue(MasterDetailPageWindowsSpecifics.CollapseStyleProperty); }
			set { Element.SetValue(MasterDetailPageWindowsSpecifics.CollapseStyleProperty, value); }
		}

		public MasterDetailPage Element { get; }
	}

	public static class MasterDetailPageWindowsSpecifics
	{
		public static readonly BindableProperty CollapseStyleProperty = BindableProperty.Create("CollapseStyle", typeof(CollapseStyle),
			typeof(MasterDetailPage), CollapseStyle.None);

		public static void SetCollapseStyle(this MasterDetailPage mdp, CollapseStyle value)
		{
			mdp.SetValue(CollapseStyleProperty, value);
		}

		public static CollapseStyle GetCollapseStyle(this MasterDetailPage mdp)
		{
			return (CollapseStyle)mdp.GetValue(CollapseStyleProperty);
		}
	}

	internal class MasterDetailPageAndroidConfiguration : IMasterDetailPageAndroidConfiguration
	{
		public MasterDetailPageAndroidConfiguration(MasterDetailPage element)
		{
			Element = element;
		}

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

		public MasterDetailPage Element { get; }
	}

	internal class MasterDetailPageiOsConfiguration : IMasterDetailPageiOSConfiguration
	{
		public MasterDetailPageiOsConfiguration(MasterDetailPage element)
		{
			Element = element;
		}

		public MasterDetailPage Element { get; }
	}

	public static class MasterDetailPageAndroidSpecifics
	{
		#region Properties

		public static readonly BindableProperty SomeAndroidThingProperty = BindableProperty.Create("SomeAndroidThing", typeof(int),
			typeof(MasterDetailPage), 1);

		public static void SetSomeAndroidThing(this MasterDetailPage mdp, int value)
		{
			mdp.SetValue(SomeAndroidThingProperty, value);
		}

		public static int GetSomeAndroidThing(this MasterDetailPage mdp)
		{
			return (int)mdp.GetValue(SomeAndroidThingProperty);
		}

		public static readonly BindableProperty SomeOtherAndroidThingProperty = BindableProperty.Create("SomeOtherAndroidThing", typeof(int),
			typeof(MasterDetailPage), 1);

		public static void SetSomeOtherAndroidThingThing(this MasterDetailPage mdp, int value)
		{
			mdp.SetValue(SomeOtherAndroidThingProperty, value);
		}

		public static int GetSomeOtherAndroidThing(this MasterDetailPage mdp)
		{
			return (int)mdp.GetValue(SomeOtherAndroidThingProperty);
		}


		#endregion

		#region Configuration

		public static IMasterDetailPageAndroidConfiguration UseTabletDefaults(this IMasterDetailPageAndroidConfiguration config)
		{
			config.SomeAndroidThing = 10;
			config.SomeOtherAndroidThing = 45;

			return config;
		}

		public static IMasterDetailPageAndroidConfiguration UsePhabletDefaults(this IMasterDetailPageAndroidConfiguration config)
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

		public static IMasterDetailPageAndroidConfiguration SetThing(this IMasterDetailPageAndroidConfiguration config, int value)
		{
			config.SomeAndroidThing = value;
			return config;
		}

		#endregion
	}

	public static class MasterDetailPageWindowsConfigurationExtensions
	{
		public static IMasterDetailPageWindowsConfiguration UsePartialCollapse(this IMasterDetailPageWindowsConfiguration config)
		{
			config.CollapseStyle = CollapseStyle.Partial;
			return config;
		}
	}

	#endregion

	#region Translucent

	public interface INavigationPagePlatformConfiguration
	{
		INavigationPageWindowsConfiguration OnWindows();

		INavigationPageAndroidConfiguration OnAndroid();

		INavigationPageiOSConfiguration OniOS();
	}

	public interface INavigationPageiOSConfiguration : IElementConfiguration<NavigationPage>
	{
		bool IsNavigationBarTranslucent { get; set; }
	}

	public interface INavigationPageAndroidConfiguration : IElementConfiguration<NavigationPage>
	{
	}

	public interface INavigationPageWindowsConfiguration : IElementConfiguration<NavigationPage>
	{
	}

	public class NavigationPageiOSConfiguration : INavigationPageiOSConfiguration
	{
		public NavigationPageiOSConfiguration (NavigationPage element)
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

	public class NavigationPageAndroidConfiguration : INavigationPageAndroidConfiguration
	{
		public NavigationPageAndroidConfiguration (NavigationPage element)
		{
			Element = element;
		}

		public NavigationPage Element { get; }
	}


	public class NavigationPageWindowsConfiguration : INavigationPageWindowsConfiguration
	{
		public NavigationPageWindowsConfiguration(NavigationPage element)
		{
			Element = element;
		}

		public NavigationPage Element { get; }
	}


	public static class NavigationPageiOSpecifics
	{
		public static readonly BindableProperty IsNavigationBarTranslucentProperty = BindableProperty.Create("IsNavigationBarTranslucent", typeof(bool),
			typeof(NavigationPage), false);

		public static void SetIsNavigationBarTranslucent(this NavigationPage navigationPage, bool value)
		{
			navigationPage.SetValue(IsNavigationBarTranslucentProperty, value);
		}

		public static bool GetIsNavigationBarTranslucent(this NavigationPage navigationPage)
		{
			return (bool)navigationPage.GetValue(IsNavigationBarTranslucentProperty);
		}
	}

	#endregion
}