using System.Collections.Generic;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.TabbedPage;

	public static class TabbedPage
	{
		public static readonly BindableProperty TabBarItemsProperty = BindableProperty.Create(nameof(TabBarItems), typeof(Dictionary<int, TabBarItem>), typeof(TabbedPage));

		public static Dictionary<int, TabBarItem> GetTabBarItems(BindableObject element)
		{
			return (Dictionary<int, TabBarItem>)element.GetValue(TabBarItemsProperty);
		}

		public static void SetTabBarItems(BindableObject element, Dictionary<int, TabBarItem> value)
		{
			element.SetValue(TabBarItemsProperty, value);
		}

		public static Dictionary<int, TabBarItem> TabBarItems(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetTabBarItems(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetTabBarItems(this IPlatformElementConfiguration<iOS, FormsElement> config, Dictionary<int, TabBarItem> value)
		{
			SetTabBarItems(config.Element, value);
			return config;
		}
	}
}