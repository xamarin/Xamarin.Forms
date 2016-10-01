﻿namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.TabbedPage;

	public static class TabbedPage
	{
		public static readonly BindableProperty IsSwipePagingEnabledProperty =
			BindableProperty.Create("IsSwipePagingEnabled", typeof(bool),
			typeof(TabbedPage), true);

		public static bool GetIsSwipePagingEnabled(BindableObject element)
		{
			return (bool)element.GetValue(IsSwipePagingEnabledProperty);
		}

		public static void SetIsSwipePagingEnabled(BindableObject element, bool value)
		{
			element.SetValue(IsSwipePagingEnabledProperty, value);
		}

		public static bool IsSwipePagingEnabled(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetIsSwipePagingEnabled(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetIsSwipePagingEnabled(this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
		{
			SetIsSwipePagingEnabled(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<Android, FormsElement> EnableSwipePaging(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			SetIsSwipePagingEnabled(config.Element, true);
			return config;
		}

		public static IPlatformElementConfiguration<Android, FormsElement> DisableSwipePaging(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			SetIsSwipePagingEnabled(config.Element, false);
			return config;
		}
	}
}
