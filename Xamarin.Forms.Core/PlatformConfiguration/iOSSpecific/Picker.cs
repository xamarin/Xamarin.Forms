namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.Picker;

	public static class Picker
	{
		public static readonly BindableProperty ShouldChangeSelectedIndexWhenDoneProperty = BindableProperty.Create(nameof(ShouldChangeSelectedIndexWhenDone), typeof(bool), typeof(Picker), false);

		public static bool GetShouldChangeSelectedIndexWhenDone(BindableObject element)
		{
			return (bool)element.GetValue(ShouldChangeSelectedIndexWhenDoneProperty);
		}

		public static void SetShouldChangeSelectedIndexWhenDone(BindableObject element, bool value)
		{
			element.SetValue(ShouldChangeSelectedIndexWhenDoneProperty, value);
		}

		public static bool ShouldChangeSelectedIndexWhenDone(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetShouldChangeSelectedIndexWhenDone(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetShouldChangeSelectedIndexWhenDone(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetShouldChangeSelectedIndexWhenDone(config.Element, value);
			return config;
		}
	}
}