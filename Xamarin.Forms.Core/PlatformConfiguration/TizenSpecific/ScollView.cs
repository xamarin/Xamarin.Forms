namespace Xamarin.Forms.PlatformConfiguration.TizenSpecific
{
	using FormsElement = Forms.ScrollView;

	public static class ScrollView
	{
		public static readonly BindableProperty BarColorProperty = BindableProperty.Create("BarColor", typeof(Color), typeof(FormsElement), Color.Default);

		public static Color GetBarColor(BindableObject element)
		{
			return (Color)element.GetValue(BarColorProperty);
		}

		public static void SetBarColor(BindableObject element, Color color)
		{
			element.SetValue(BarColorProperty, color);
		}

		public static Color GetBarColor(this IPlatformElementConfiguration<Tizen, FormsElement> config)
		{
			return GetBarColor(config.Element);
		}

		public static IPlatformElementConfiguration<Tizen, FormsElement> SetBarColor(this IPlatformElementConfiguration<Tizen, FormsElement> config, Color color)
		{
			SetBarColor(config.Element, color);
			return config;
		}
	}
}
