namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Xamarin.Forms.TimePicker;

	public static class TimePicker
	{
		public static readonly BindableProperty UpdateModeProperty = BindableProperty.Create(
			nameof(UpdateMode),
			typeof(UpdateMode),
			typeof(TimePicker),
			default(UpdateMode));

		public static UpdateMode GetUpdateMode(BindableObject element)
		{
			return (UpdateMode)element.GetValue(UpdateModeProperty);
		}

		public static void SetUpdateMode(BindableObject element, UpdateMode value)
		{
			element.SetValue(UpdateModeProperty, value);
		}

		public static UpdateMode UpdateMode(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetUpdateMode(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetUpdateMode(this IPlatformElementConfiguration<iOS, FormsElement> config, UpdateMode value)
		{
			SetUpdateMode(config.Element, value);
			return config;
		}

		public static readonly BindableProperty DatePickerStyleProperty = BindableProperty.Create(
			nameof(UIDatePickerStyle),
			typeof(UIDatePickerStyle),
			typeof(TimePicker),
			default(UIDatePickerStyle));

		public static UIDatePickerStyle GetUIPickerStyle(BindableObject element)
		{
			return (UIDatePickerStyle)element.GetValue(DatePickerStyleProperty);
		}

		public static void SetUIPickerStyle(BindableObject element, UIDatePickerStyle value)
		{
			element.SetValue(DatePickerStyleProperty, value);
		}

		public static UIDatePickerStyle UIDatePickerStyle(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetUIPickerStyle(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetUIPickerStyle(this IPlatformElementConfiguration<iOS, FormsElement> config, UIDatePickerStyle value)
		{
			SetUIPickerStyle(config.Element, value);
			return config;
		}
	}
}