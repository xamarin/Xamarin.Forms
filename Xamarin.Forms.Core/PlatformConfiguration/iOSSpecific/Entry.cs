namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.Entry;

	public static class Entry
	{
		public static readonly BindableProperty HasDoneButtonProperty = BindableProperty.Create("HasDoneButton", typeof(bool), typeof(Entry), false);

		public static bool GetHasDoneButton(BindableObject element)
		{
			return (bool)element.GetValue(HasDoneButtonProperty);
		}

		public static void SetHasDoneButton(BindableObject element, bool value)
		{
			element.SetValue(HasDoneButtonProperty, value);
		}

		public static bool HasDoneButton(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetHasDoneButton(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetHasDoneButton(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetHasDoneButton(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> EnableDoneButton(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetHasDoneButton(config.Element, true);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> DisableDoneButton(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetHasDoneButton(config.Element, false);
			return config;
		}
	}
}