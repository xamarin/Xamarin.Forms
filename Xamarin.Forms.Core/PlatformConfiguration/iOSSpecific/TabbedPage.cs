using System.Collections.Generic;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.TabbedPage;

	public static class TabbedPage
	{
		public static readonly BindableProperty TabSwipesProperty = BindableProperty.Create(nameof(TabSwipes), typeof(Dictionary<int, TabSwipe>), typeof(TabbedPage));

		public static Dictionary<int, TabSwipe> GetTabSwipes(BindableObject element)
		{
			return (Dictionary<int, TabSwipe>)element.GetValue(TabSwipesProperty);
		}

		public static void SetTabSwipes(BindableObject element, Dictionary<int, TabSwipe> value)
		{
			element.SetValue(TabSwipesProperty, value);
		}

		public static Dictionary<int, TabSwipe> TabSwipes(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetTabSwipes(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetTabSwipes(this IPlatformElementConfiguration<iOS, FormsElement> config, Dictionary<int, TabSwipe> value)
		{
			SetTabSwipes(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> EnableTabSwipes(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			Dictionary<int, TabSwipe> tabSwipes = GetTabSwipes(config.Element);
			if (tabSwipes == null || tabSwipes.Count == 0)
			{
				if (tabSwipes == null)
				{
					tabSwipes = new Dictionary<int, TabSwipe>();
				}

				for (var i = 0; i < config.Element.Children.Count; i++)
				{
					tabSwipes.Add(i, new TabSwipe());
				}
			}

			SetTabSwipes(config.Element, tabSwipes);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> DisableTabSwipes(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetTabSwipes(config.Element, null);
			return config;
		}
	}
}