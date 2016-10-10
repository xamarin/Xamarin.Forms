using System.Collections.Generic;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.TabbedPage;

	public static class TabbedPage
	{
		public static readonly BindableProperty TabSlidesProperty = BindableProperty.Create(nameof(TabSlides), typeof(Dictionary<int, TabSlide>), typeof(TabbedPage));

		public static Dictionary<int, TabSlide> GetTabSlides(BindableObject element)
		{
			return (Dictionary<int, TabSlide>)element.GetValue(TabSlidesProperty);
		}

		public static void SetTabSlides(BindableObject element, Dictionary<int, TabSlide> value)
		{
			element.SetValue(TabSlidesProperty, value);
		}

		public static Dictionary<int, TabSlide> TabSlides(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetTabSlides(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetTabSlides(this IPlatformElementConfiguration<iOS, FormsElement> config, Dictionary<int, TabSlide> value)
		{
			SetTabSlides(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> EnableTabSlides(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			Dictionary<int, TabSlide> tabSlides = GetTabSlides(config.Element);
			if (tabSlides == null || tabSlides.Count == 0)
			{
				if (tabSlides == null)
				{
					tabSlides = new Dictionary<int, TabSlide>();
				}

				for (var i = 0; i < config.Element.Children.Count; i++)
				{
					tabSlides.Add(i, new TabSlide());
				}
			}

			SetTabSlides(config.Element, tabSlides);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> DisableTabSlides(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetTabSlides(config.Element, null);
			return config;
		}
	}
}