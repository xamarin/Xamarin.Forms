namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.ListView;

	public static class ListView
	{
		public static readonly BindableProperty IsFastScrollEnabledProperty = BindableProperty.Create("IsFastScrollEnabled", typeof(bool), typeof(ListView), false);

		public static bool GetIsFastScrollEnabled(BindableObject element)
		{
			return (bool)element.GetValue(IsFastScrollEnabledProperty);
		}

		public static void SetIsFastScrollEnabled(BindableObject element, bool value)
		{
			element.SetValue(IsFastScrollEnabledProperty, value);
		}

		public static bool IsFastScrollEnabled(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetIsFastScrollEnabled(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetIsFastScrollEnabled(this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
		{
			SetIsFastScrollEnabled(config.Element, value);
			return config;
		}

		public static readonly BindableProperty IsNestedScrollingEnabledProperty = BindableProperty.Create("IsNestedScrollingEnabled", typeof(bool), typeof(ListView), false);

		public static bool GetIsNestedScrollingEnabled(BindableObject element)
		{
			return (bool)element.GetValue(IsNestedScrollingEnabledProperty);
		}

		public static void SetIsNestedScrollingEnabled(BindableObject element, bool value)
		{
			element.SetValue(IsNestedScrollingEnabledProperty, value);
		}

		public static bool IsNestedScrollingEnabled(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetIsNestedScrollingEnabled(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetIsNestedScrollingEnabled(this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
		{
			SetIsNestedScrollingEnabled(config.Element, value);
			return config;
		}
	}
}
