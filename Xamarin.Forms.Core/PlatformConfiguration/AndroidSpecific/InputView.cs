namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.InputView;

	public static class InputView
	{
		public static readonly BindableProperty DismissKeyboardOnOutsideTapProperty = BindableProperty.Create(nameof(DismissKeyboardOnOutsideTap), typeof(bool), typeof(InputView), true);

		public static bool GetDismissKeyboardOnOutsideTap(BindableObject element)
		{
			return (bool)element.GetValue(DismissKeyboardOnOutsideTapProperty);
		}

		public static void SetDismissKeyboardOnOutsideTap(BindableObject element, bool value)
		{
			element.SetValue(DismissKeyboardOnOutsideTapProperty, value);
		}

		public static bool DismissKeyboardOnOutsideTap(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetDismissKeyboardOnOutsideTap(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetDismissKeyboardOnOutsideTap(this IPlatformElementConfiguration<Xamarin.Forms.PlatformConfiguration.Android, FormsElement> config, bool value)
		{
			SetDismissKeyboardOnOutsideTap(config.Element, value);
			return config;
		}
	}
}
