namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.TabbedPage;

	public static class TabbedPage
	{
		public static readonly BindableProperty UnselectedTintColorProperty =
			BindableProperty.Create("UnselectedTintColor", typeof(Color),
			typeof(TabbedPage), Color.Default);

		public static Color GetUnselectedTintColor(BindableObject element)
		{
			return (Color)element.GetValue(UnselectedTintColorProperty);
		}

		public static void SetUnselectedTintColor(BindableObject element, Color value)
		{
			element.SetValue(UnselectedTintColorProperty, value);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetUnselectedTintColor(this IPlatformElementConfiguration<iOS, FormsElement> config, Color value)
		{
			SetUnselectedTintColor(config.Element, value);
			return config;
		}

		public static Color GetUnselectedTintColor(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetUnselectedTintColor(config.Element);
		}

		public static readonly BindableProperty SelectedTintColorProperty =
			BindableProperty.Create("SelectedTintColor", typeof(Color),
			typeof(TabbedPage), Color.Default);

		public static Color GetSelectedTintColor(BindableObject element)
		{
			return (Color)element.GetValue(SelectedTintColorProperty);
		}

		public static void SetSelectedTintColor(BindableObject element, Color value)
		{
			element.SetValue(SelectedTintColorProperty, value);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetSelectedTintColor(this IPlatformElementConfiguration<iOS, FormsElement> config, Color value)
		{
			SetSelectedTintColor(config.Element, value);
			return config;
		}

		public static Color GetSelectedTintColor(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetSelectedTintColor(config.Element);
		}
	}
}