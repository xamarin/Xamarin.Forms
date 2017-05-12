namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.Application;

	public enum WindowSoftInputModeAdjust
	{
		Unspecified,
		Pan,
		Resize
	}

	public static class Application
	{
		public static readonly BindableProperty WindowSoftInputModeAdjustProperty = BindableProperty.Create("WindowSoftInputModeAdjust", typeof(WindowSoftInputModeAdjust), typeof(Application), WindowSoftInputModeAdjust.Pan);

		public static WindowSoftInputModeAdjust GetWindowSoftInputModeAdjust(BindableObject element)
		{
			return (WindowSoftInputModeAdjust)element.GetValue(WindowSoftInputModeAdjustProperty);
		}

		public static void SetWindowSoftInputModeAdjust(BindableObject element, WindowSoftInputModeAdjust value)
		{
			element.SetValue(WindowSoftInputModeAdjustProperty, value);
		}

		public static WindowSoftInputModeAdjust GetWindowSoftInputModeAdjust(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetWindowSoftInputModeAdjust(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> UseWindowSoftInputModeAdjust(this IPlatformElementConfiguration<Android, FormsElement> config, WindowSoftInputModeAdjust value)
		{
			SetWindowSoftInputModeAdjust(config.Element, value);
			return config;
		}

		public static readonly BindableProperty ShouldSetWindowSoftInputModeAtStartupProperty = BindableProperty.Create("ShouldSetWindowSoftInputModeAtStartup", typeof(bool), typeof(Application), true);

		public static bool GetShouldSetWindowSoftInputModeAtStartup(BindableObject element)
		{
			return (bool)element.GetValue(ShouldSetWindowSoftInputModeAtStartupProperty);
		}

		public static void SetShouldSetWindowSoftInputModeAtStartup(BindableObject element, bool value)
		{
			element.SetValue(ShouldSetWindowSoftInputModeAtStartupProperty, value);
		}

		public static bool GetShouldSetWindowSoftInputModeAtStartup(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetShouldSetWindowSoftInputModeAtStartup(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetShouldSetWindowSoftInputModeAtStartup(this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
		{
			SetShouldSetWindowSoftInputModeAtStartup(config.Element, value);
			return config;
		}
	}
}
